using HandSchool.Internals;
using HandSchool.Internals.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Services;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("JLU", "远程教学信息查询", "查看课程QQ群、上课平台~", EntranceType.InfoEntrance)]
    internal class RemoteSchedule : BaseController, IInfoEntrance
    {
        const string ScriptFileUri = "service/res.do";
        const string Query = "{\"tag\":\"tcmRemote@tcs\",\"branch\":\"default\",\"params\":{\"termId\":`term`,\"studId\":`studId`}}";
        
        public RemoteSchedule()
        {
            var progList = new TableResponsive(bodyId: "progList")
            {
                { "课程名称", 15 },
                { "教学方式", 8 },
                { "教学平台", 6 },
                { "联系方式（默认QQ群）", 15 },
            };

            progList.DefaultContent = "<tr><td colspan=\"5\">加载中……</td></tr>";

            HtmlDocument = new Bootstrap
            {
                Children =
                {
                    progList
                },
                JavaScript =
                {
                    "$(function(){invokeCSharpAction('show')})",
                }
            };
        }

        public Bootstrap HtmlDocument { get; set; }
        
        private async Task SolveInfo()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var LastReport = await Core.App.Service.Post(ScriptFileUri, Query);
                var remoteTcm = LastReport.ParseJSON<RootObject<RemoteTcm>>();
                var sb = new StringBuilder();

                foreach (var opt in remoteTcm.value)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{opt.teachClassMaster.lessonSegment.fullName}</td>");
                    if (opt.teachClassMaster.tcmRemote == null)
                        sb.Append("<td></td><td></td><td></td>");
                    else
                        sb.Append($"<td>{opt.teachClassMaster.tcmRemote.teachMethod}</td>")
                            .Append($"<td>{opt.teachClassMaster.tcmRemote.platform}</td>")
                            .Append($"<td>{opt.teachClassMaster.tcmRemote.contactGroup}</td>");
                    sb.Append("</tr>");
                }

                Evaluate?.Invoke($"$('#progList').html('{sb}')");
                sb.Clear();
            }
            catch (JsonException)
            {
                await RequestMessageAsync("提示", "加载信息失败，解析数据出现错误。");
            }
            catch (WebsException ex)
            {
                await RequestMessageAsync("错误", ex.Status.ToDescription() + "。");
            }

            IsBusy = false;
        }
        
        public override async Task Receive(string data)
        {
            if (data == "show")
            {
                await SolveInfo();
            }
            else
            {
                await RequestMessageAsync("错误", "请报告开发者，参数未知：" + data);
            }
        }
    }
}
