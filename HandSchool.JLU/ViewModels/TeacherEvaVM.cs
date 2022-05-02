using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using HandSchool.JLU.Models;
using HandSchool.Models;
using Xamarin.Forms.Internals;

namespace HandSchool.JLU.ViewModels
{ 
    public class TeacherEvaVM : NoticeCheckViewModel
    {
        public static readonly IReadOnlyCollection<string> SupportTemplateVersions = new HashSet<string> { "180", "190" };
        public static TeacherEvaVM Instance => lazy.Value;
        private static Lazy<TeacherEvaVM> lazy = new Lazy<TeacherEvaVM>(() => new TeacherEvaVM());
        private const string GetStuInfoUrl = "action/getCurrentUserInfo.do";
        private const string UIMSRes = "service/res.do";
        private const string EvaItemUrl = "{\"tag\":\"student@evalItem\",\"branch\":\"self\",\"params\":{}}";
        private const string EvaUrl = "action/eval/eval-with-answer.do";
        private const string PuzzleUrl = "action/eval/fetch-eval-item.do";

        public System.Collections.ObjectModel.ObservableCollection<EvaItemShell> EvaItems { get; } 
            = new System.Collections.ObjectModel.ObservableCollection<EvaItemShell>();
        public ICommand EvaCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public TeacherEvaVM()
        {
            RefreshCommand = new CommandAction(GetEvaItems);
            EvaCommand = new CommandAction(EvalAll);
        }
        private List<string> _classMates;
        private async Task<List<string>> GetClassMates()
        {
            var infoJson = await Core.App.Service.Post(GetStuInfoUrl, null);
            if (string.IsNullOrWhiteSpace(infoJson)) return null;
            var info = JsonConvert.DeserializeObject<JObject>(infoJson)["defRes"];
            var schId = info["school"].ToString();
            var dept = info["department"].ToString();
            var adcId = info["adcId"].ToString();

            var getCMs = "{\"tag\":\"student_sch_dept\",\"branch\":\"default\",\"params\": {\"schId\":\"" + schId + "\",\"deptId\":\"" + dept + "\",\"adcId\":\"" + adcId + "\"}}";
            var cmJson = await Core.App.Service.Post(UIMSRes, getCMs);
            if (string.IsNullOrWhiteSpace(cmJson)) return null;
            var res = JsonConvert.DeserializeObject<JObject>(cmJson)?["value"]?.ToObject<List<StudentName>>();
            return res.Select(x => x.name)?.ToList();
        }

        public async Task GetEvaItems()
        {
            if (IsBusy) return;
            IsBusy = true;
            if (!await CheckEnvAndNotice("GetEvaItems")) return;
            try
            {
                var str = await Core.App.Service.Post(UIMSRes, EvaItemUrl);
                if (string.IsNullOrWhiteSpace(str))
                {
                    IsBusy = false;
                    return;
                }

                var items = JsonConvert.DeserializeObject<JObject>(str)?["value"]?.ToObject<List<StudEval>>()
                    ?.Select(e => new EvaItemShell(e,
                        SupportTemplateVersions.Contains(e.evalActTime?.evalGuideline?.evalGuidelineId)));
                Core.Platform.EnsureOnMainThread(() =>
                {
                    EvaItems.Clear();
                    items?.ForEach(EvaItems.Add);
                });
            }
            catch (Exception ex)
            {
                await NoticeError("出错了：" + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }

        }

        public async Task<TaskResp> EvalOne(EvaItemShell evaItem)
        {
            try
            {
                switch (evaItem.EvalGuidelineId)
                {
                    case "180":
                    {
                        _classMates ??= await GetClassMates();
                        if (evaItem?.InnerInfo is null) return TaskResp.False;
                        var getPuzzle = "{\"evalItemId\":\"" + evaItem.InnerInfo.evalItemId + "\"}";
                        var res = await Core.App.Service.Post(PuzzleUrl, getPuzzle);
                        var jo = JsonConvert.DeserializeObject<JObject>(res)?["items"]?[0]?["puzzle"]?.ToString()
                            .Replace("_", ".");
                        if (string.IsNullOrWhiteSpace(jo)) return TaskResp.False;
                        if (!jo.Contains(".")) return false;
                        foreach (var classMate in _classMates)
                        {
                            var m = Regex.Match(classMate, jo);
                            if (!m.Success || m.Length != jo.Length) continue;
                            var index = jo.IndexOf(".", StringComparison.Ordinal);
                            var ans = m.Value[index];
                            var postValue = "{\"evalItemId\":\"" + evaItem.InnerInfo.evalItemId +
                                            "\",\"answers\":{\"p01\":\"A\",\"p02\":\"A\",\"p03\":\"A\",\"p04\":\"A\",\"p05\":\"A\",\"p06\":\"A\",\"p07\":\"A\",\"p08\":\"A\",\"p09\":\"A\",\"p10\":\"A\",\"sat11\":\"A\",\"sat12\":\"A\",\"sat13\":\"A\",\"puzzle_answer\":\"" +
                                            ans + "\"}}";
                            var evaRes = await Core.App.Service.Post(EvaUrl, postValue);
                            var resp = JsonConvert.DeserializeObject<JObject>(evaRes)?["status"]?.ToString();
                            return resp == "0" ? TaskResp.True : TaskResp.False;
                        }

                        return false;
                    }
                    case "190":
                    {
                        var postValue = "{\"evalItemId\":\"" + evaItem.InnerInfo.evalItemId +
                                        "\",\"answers\":{\"p01\":\"A\",\"p02\":\"A\",\"p03\":\"A\",\"p04\":\"A\",\"p05\":\"A\",\"p06\":\"A\",\"p07\":\"A\",\"p08\":\"A\",\"p09\":\"A\",\"p10\":\"A\",\"sat11\":\"A\",\"sat12\":\"A\",\"sat13\":\"A\"}}";
                        var evaRes = await Core.App.Service.Post(EvaUrl, postValue);
                        var resp = JsonConvert.DeserializeObject<JObject>(evaRes)?["status"]?.ToString();
                        return resp == "0";
                    }
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool IsEmpty => EvaItems.Count == 0;
        public async Task EvalAll()
        {
            if(IsBusy) return;
            var queue = EvaItems.Where(i => !i.IsEvaluated && i.IsEnable).ToArray();
            if (queue.Length == 0)
            {
                await RequestMessageAsync("提示", "未发现可评老师");
                return;
            };
            var count = 0;
            if (!await CheckEnvAndNotice("EvalAll")) return;
            try
            {
                IsBusy = true;
                foreach (var e in queue)
                {
                    if ((await EvalOne(e)).IsSuccess)
                    {
                        count++;
                    }
                }

                IsBusy = false;
                await RequestMessageAsync("成功", "给了" + count + "名老师好评");
            }
            catch (Exception ex)
            {
                await NoticeError("出错了：" + ex.Message + "\n" + "给了" + count + "名老师好评");
            }
            await GetEvaItems();
        }
    } 
}
