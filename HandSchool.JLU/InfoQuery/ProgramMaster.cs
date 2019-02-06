using HandSchool.Internals;
using HandSchool.Internals.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonException = Newtonsoft.Json.JsonException;

namespace HandSchool.JLU.InfoQuery
{
    /// <summary>
    /// 查询专业培养计划的模块。
    /// </summary>
    /// <inheritdoc cref="BaseController" />
    /// <inheritdoc cref="IInfoEntrance" />
    [Entrance("JLU", "专业培养计划", "看看我们要学什么，提前预习偷偷补课。", EntranceType.InfoEntrance)]
    internal class ProgramMaster : BaseController, IInfoEntrance
    {
        public Bootstrap HtmlDocument { get; set; }
        public int[] Batches = { 2009, 2013, 2018 };

        private int batch = 2013;
        private int schId = 101;
        private int progId = -1;

        public string ScriptFileUri => "service/res.do";
        public string PostValue => $"{{\"tag\":\"programDetail@common\",\"branch\":\"default\",\"params\":{{\"progId\":\"{progId}\"}},\"orderBy\":\" advGrade, termSeq, courseInfo.extCourseNo\"}}";
        public string PostList => $"{{\"type\":\"search\",\"branch\":\"default\",\"params\":{{\"schId\":\"{schId}\",\"active\":\"Y\",\"batch\":\"{batch}\"}},\"tag\":\"programMaster@programsQuery\"}}";

        public ProgramMaster()
        {
            var schIdSelect = new Select
            (
                "schId",
                new EnumerableAdapter
                (
                    from item in AlreadyKnownThings.Colleges
                    where item.Opt != null
                    select new KeyValuePair<string, string>(item.Id, item.Opt + " - " + item.Name)
                )
            );

            var batchSelect = new Select
            (
                "batch",
                new EnumerableAdapter
                (
                    from i in Batches
                    select new KeyValuePair<string, string>($"{i}", $"{i}版")
                )
            );

            var progList = new TableResponsive(bodyId: "progList")
            {
                { "课程名称", 15 },
                { "课程代码", 8 },
                { "课程性质", 6 },
                { "建议学年", 6 },
                { "建议学期", 6 },
                { "开课学院", 15 },
                { "课程学分", 6 },
                { "课程总学时", 7.5m },
                { "内实践学时", 7.5m },
            };

            progList.DefaultContent = "<tr><td colspan=\"9\">请先选择一个教学计划</td></tr>";

            var progIdSelect = new Select("progId")
            {
                { "-1", "请选择" },
                { "413", "软件工程(软件学院)2013版培养计划" },
                { "489", "工科试验班(软件工程)2013版培养计划" },
            };
            
            HtmlDocument = new Bootstrap
            {
                Children =
                {
                    new MasterDetail(new Form
                    {
                        schIdSelect.WrapFormGroup(),
                        batchSelect.WrapFormGroup(),
                        progIdSelect.WrapFormGroup(),
                    }) { progList }
                },
                JavaScript =
                {
                    "$('#schId').on('change',function(){invokeCSharpAction('schId='+$(this).val())});",
                    "$('#batch').on('change',function(){invokeCSharpAction('batch='+$(this).val())});",
                    "$('#progId').on('change',function(){invokeCSharpAction('progId='+$(this).val())});",
                    "$(function(){$('#schId').val('101');$('#batch').val('2013');$('#progId').val('-1')});"
                }
            };

            Menu.Add(new HandSchool.Views.MenuEntry
            {
                Title = "加载",
                UWPIcon = "\uE721",
                Command = new CommandAction(SolveProgVal)
            });
        }

        public async void SolveProgId()
        {
            if (IsBusy) return; IsBusy = true;

            try
            {
                var LastReport = await Core.App.Service.Post(ScriptFileUri, PostList);
                var lists = LastReport.ParseJSON<RootObject<ProgItem>>();
                var sb = new StringBuilder();
                sb.Append("<option value=\"-1\" selected>请选择</option>");

                foreach (var opt in lists.value)
                {
                    sb.Append($"<option value=\"{opt.progId}\">{opt.title}</option>");
                }

                Evaluate?.Invoke($"$('#progId').html('{sb}')");
                sb.Clear();
                IsBusy = false;
            }
            catch (JsonException)
            {
                IsBusy = false;
                await RequestMessageAsync("提示", "加载教学方案失败。");
            }
            catch (WebsException ex)
            {
                await RequestMessageAsync("错误", ex.Status.ToDescription() + "。");
                IsBusy = false;
            }
        }

        public async Task SolveProgVal()
        {
            if (IsBusy) return;

            if (progId == -1)
            {
                await RequestMessageAsync("提示", "请选择一个培养计划！");
                Evaluate?.Invoke($"$('#progList').html('<tr><td colspan=\"9\">请先选择一个教学计划</td></tr>')");
                return;
            }

            IsBusy = true;
            Evaluate?.Invoke($"$('#progList').html('<tr><td colspan=\"9\">正在加载……</td></tr>')");

            try
            {
                var LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
                var lists = LastReport.ParseJSON<RootObject<ProgTerm>>();
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

                Evaluate?.Invoke($"$('#progList').html('{sb}')");
                sb.Clear();
                IsBusy = false;
            }
            catch (JsonException)
            {
                IsBusy = false;
                Evaluate?.Invoke("$('#progList').html('<tr><td colspan=\"9\">加载失败</td></tr>')");
                await RequestMessageAsync("提示", "加载教学方案失败。");
            }
            catch (WebsException ex)
            {
                IsBusy = false;
                await RequestMessageAsync("错误", ex.Status.ToDescription() + "。");
            }
        }

        public override async Task Receive(string data)
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
                await RequestMessageAsync("错误", "未知响应：" + data);
            }
        }
    }
}
