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
    public class TeacherEvaVM : BaseViewModel
    {
        public const string TemplateVersion = "180";
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
                        e.evalActTime?.evalGuideline?.evalGuidelineId?.Equals(TemplateVersion.ToString()) == true));
                Core.Platform.EnsureOnMainThread(() =>
                {
                    EvaItems.Clear();
                    items?.ForEach(v => EvaItems.Add(v));
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
                _classMates ??= await GetClassMates();
                if (evaItem?.InnerInfo is null) return TaskResp.False;
                var getPuzzle = "{\"evalItemId\":\"" + evaItem.InnerInfo.evalItemId + "\"}";
                var res = await Core.App.Service.Post(PuzzleUrl, getPuzzle);
                var jo = JsonConvert.DeserializeObject<JObject>(res)?["items"]?[0]?["puzzle"]?.ToString()
                    .Replace("_", ".");
                if (string.IsNullOrWhiteSpace(jo)) return TaskResp.False;
                if (jo.Contains("."))
                {
                    foreach (var classMate in _classMates)
                    {
                        var m = Regex.Match(classMate, jo);
                        if (m.Success && m.Length == jo.Length)
                        {

                            var index = jo.IndexOf(".", StringComparison.Ordinal);
                            var ans = m.Value[index];
                            var postValue = "{\"evalItemId\":\"" + evaItem.InnerInfo.evalItemId +
                                            "\",\"answers\":{\"p01\":\"A\",\"p02\":\"A\",\"p03\":\"A\",\"p04\":\"A\",\"p05\":\"A\",\"p06\":\"A\",\"p07\":\"A\",\"p08\":\"A\",\"p09\":\"A\",\"p10\":\"A\",\"sat11\":\"A\",\"sat12\":\"A\",\"sat13\":\"A\",\"puzzle_answer\":\"" +
                                            ans + "\"}}";
                            var evaRes = await Core.App.Service.Post(EvaUrl, postValue);
                            var resp = JsonConvert.DeserializeObject<JObject>(evaRes)?["status"]?.ToString();
                            return resp == "0" ? TaskResp.True : TaskResp.False;
                        }
                    }
                }
                return TaskResp.False;
            }
            catch (Exception e)
            {
                return TaskResp.False;
            }
        }

        public bool IsEmpty => EvaItems.Count == 0;
        public async Task EvalAll()
        {
            if(IsBusy) return;
            if(EvaItems.Count == 0 || EvaItems.All(e => !e.IsEnable || e.IsEvaluated))
            {
                await RequestMessageAsync("提示", "未发现可评老师");
                return;
            };
            var count = 0;
            try
            {
                IsBusy = true;
                foreach (var e in EvaItems)
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
