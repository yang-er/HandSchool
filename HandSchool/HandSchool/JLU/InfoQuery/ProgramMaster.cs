using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Command = Xamarin.Forms.Command;
using JsonException = Newtonsoft.Json.JsonException;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("专业培养计划", "看看我们要学什么，提前预习偷偷补课。", EntranceType.InfoEntrance)]
    class ProgramMaster : IInfoEntrance
    {
        public Bootstrap HtmlDocument { get; set; }
        public IViewResponse Binding { get; set; }
        public Action<string> Evaluate { get; set; }
        public List<InfoEntranceMenu> Menu { get; set; } = new List<InfoEntranceMenu>();
        public int[] Batches = { 2009, 2013, 2018 };

        private int batch = 2013;
        private int schId = 101;
        private int progId = 489;
        private bool is_busy = false;

        public ProgramMaster()
        {
            var sb = new StringBuilder();

            sb.Append("<select class=\"form-control\" id=\"schId\">");
            foreach (var opt in AlreadyKnownThings.Colleges)
            {
                if (opt.Opt == null) continue;
                sb.Append($"<option value=\"{opt.Id}\">{opt.Opt} - {opt.Name}</option>");
            }
            sb.Append("</select>");
            var sch = (RawHtml)sb.ToString();
            sb.Clear();

            sb.Append("<select class=\"form-control\" id=\"batch\">");
            foreach (var opt in Batches)
            {
                sb.Append($"<option value=\"{opt}\">{opt}版</option>");
            }
            sb.Append("</select>");
            var btch = (RawHtml)sb.ToString();
            sb.Clear();

            sb.Append("<select class=\"form-control\" id=\"progId\">");
            sb.Append("<option value=\"-1\" selected>请选择</option>");
            sb.Append("<option value=\"413\">软件工程(软件学院)2013版培养计划</option>");
            sb.Append("<option value=\"489\">工科试验班(软件工程)2013版培养计划</option>");
            sb.Append("</select>");
            var prog = (RawHtml)sb.ToString();
            sb.Clear();

            HtmlDocument = new Bootstrap
            {
                Children =
                {
                    new MasterDetail
                    {
                        InfoGather = new Form
                        {
                            Children =
                            {
                                new FormGroup { Children = { sch } },
                                new FormGroup { Children = { btch } },
                                new FormGroup { Children = { prog } },
                            }
                        },
                        Children =
                        {
                            (RawHtml) "<table class=\"table table-responsive\"><thead><tr>",
                            (RawHtml) ( "<th scope=\"col\" style=\"min-width:15em\">课程名称</th>" +
                                        "<th scope=\"col\" style=\"min-width:8em\">课程代码</th>" +
                                        "<th scope=\"col\" style=\"min-width:6em\">课程性质</th>" +
                                        "<th scope=\"col\" style=\"min-width:6em\">建议学年</th>" +
                                        "<th scope=\"col\" style=\"min-width:6em\">建议学期</th>" +
                                        "<th scope=\"col\" style=\"min-width:15em\">开课学院</th>" +
                                        "<th scope=\"col\" style=\"min-width:6em\">课程学分</th>" +
                                        "<th scope=\"col\" style=\"min-width:7.5em\">课程总学时</th>" +
                                        "<th scope=\"col\" style=\"min-width:7.5em\">内实践学时</th>"),
                            (RawHtml) "</tr></thead><tbody id=\"progList\"><tr><td colspan=\"9\">请先选择一个教学计划</td></tr></tbody></table>"
                        }
                    }
                },
                JavaScript =
                {
                    "$('#schId').on('change',function(){invokeCSharpAction('schId='+$(this).val())});",
                    "$('#batch').on('change',function(){invokeCSharpAction('batch='+$(this).val())});",
                    "$('#progId').on('change',function(){invokeCSharpAction('progId='+$(this).val())});",
                    "$(function(){$('#schId').val('101');$('#batch').val('2013');});"
                }
            };

            Menu.Add(new InfoEntranceMenu("加载", new Command(SolveProgVal), "\uE721"));
        }

        public string ScriptFileUri => "service/res.do";
        public bool IsPost => false;
        public string PostValue => $"{{\"tag\":\"programDetail@common\",\"branch\":\"default\",\"params\":{{\"progId\":\"{progId}\"}},\"orderBy\":\" advGrade, termSeq, courseInfo.extCourseNo\"}}";
        public string PostList => $"{{\"type\":\"search\",\"branch\":\"default\",\"params\":{{\"schId\":\"{schId}\",\"active\":\"Y\",\"batch\":\"{batch}\"}},\"tag\":\"programMaster@programsQuery\"}}";
        public string StorageFile => "No storage";
        public string LastReport { get; set; }
        
        public Task Execute() { throw new InvalidOperationException(); }
        public void Parse() { throw new InvalidOperationException(); }

        public async void SolveProgId()
        {
            if (is_busy) return;
            Binding.SetIsBusy(is_busy = true, "正在加载教学方案……");
            try
            {
                LastReport = await Core.App.Service.Post(ScriptFileUri, PostList);
                var lists = Helper.JSON<RootObject<ProgItem>>(LastReport);
                var sb = new StringBuilder();
                sb.Append("<option value=\"-1\" selected>请选择</option>");
                foreach (var opt in lists.value)
                {
                    sb.Append($"<option value=\"{opt.progId}\">{opt.title}</option>");
                }
                Evaluate?.Invoke($"$('#progId').html('{sb.ToString()}')");
                sb.Clear();
                Binding.SetIsBusy(is_busy = false);
            }
            catch (JsonException)
            {
                Binding.SetIsBusy(is_busy = false);
                await Binding.ShowMessage("提示", "加载教学方案失败。");
            }
        }

        public async void SolveProgVal()
        {
            if (is_busy) return;

            if (progId == -1)
            {
                await Binding.ShowMessage("提示", "请选择一个培养计划！");
                Evaluate?.Invoke($"$('#progList').html('<tr><td colspan=\"9\">请先选择一个教学计划</td></tr>')");
                return;
            }

            Binding.SetIsBusy(is_busy = true, "正在加载培养方案内容……");
            Evaluate?.Invoke($"$('#progList').html('<tr><td colspan=\"9\">正在加载……</td></tr>')");
            try
            {
                LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
                var lists = Helper.JSON<RootObject<ProgTerm>>(LastReport);
                var sb = new StringBuilder();
                foreach (var opt in lists.value)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{opt.courseInfo.courName}</td>");
                    sb.Append($"<td>{opt.courseInfo.extCourseNo}</td>");
                    sb.Append($"<td>{AlreadyKnownThings.Type5Name(opt.courseInfo.type5)}</td>");
                    sb.Append($"<td>{opt.advGrade}</td>");
                    sb.Append($"<td>{opt.termSeq}</td>");
                    sb.Append($"<td>{opt.courseInfo.school.schoolName}</td>");
                    sb.Append($"<td>{opt.credit}</td>");
                    sb.Append($"<td>{opt.classHour}</td>");
                    sb.Append($"<td>{opt.exprCredit}</td>");
                    sb.Append("</tr>");
                }
                Evaluate?.Invoke($"$('#progList').html('{sb.ToString()}')");
                sb.Clear();
                Binding.SetIsBusy(is_busy = false);
            }
            catch (JsonException)
            {
                Binding.SetIsBusy(is_busy = false);
                Evaluate?.Invoke($"$('#progList').html('<tr><td colspan=\"9\">加载失败</td></tr>')");
                await Binding.ShowMessage("提示", "加载教学方案失败。");
            }
        }

        public void Receive(string data)
        {
            if (data.StartsWith("batch="))
            {
                batch = int.Parse(data.Substring(6));
                SolveProgId();
            }
            else if (data.StartsWith("schId="))
            {
                schId = int.Parse(data.Substring(6));
                SolveProgId();
            }
            else if (data.StartsWith("progId="))
            {
                progId = int.Parse(data.Substring(7));
            }
            else
            {
                Binding.ShowMessage("错误", "未知响应：" + data);
            }
        }
    }
}
