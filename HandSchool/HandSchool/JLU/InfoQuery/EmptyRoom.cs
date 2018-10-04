using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("查空教室", "没地方自习？试试这个吧。", EntranceType.InfoEntrance)]
    class EmptyRoom : BaseController, IInfoEntrance
    {
        public Bootstrap HtmlDocument { get; set; }

        private string ScriptFileUri => "service/res.do";
        private string PostValue => $"{{\"tag\":\"roomIdle@roomUsage\",\"branch\":\"default\",\"params\":{{\"termId\":{TermId},\"bid\":\"{Bid}\",\"rname\":\"\",\"dateActual\":{{}},\"cs\":{Cs},\"d_actual\":\"{Today}T00:00:00+08:00\"}}}}";

        private string Cs = "";
        private string Bid = "";
        private string Today = "";
        private string TermId = "";
        private string LastReport { get; set; }

        public EmptyRoom()
        {
            TermId = Core.App.Service.AttachInfomation["term"];
            var sb = new StringBuilder();

            // Campus list
            sb.Append("<select class=\"form-control\" id=\"campus\" onchange=\"getList()\">");
            foreach (string key in AlreadyKnownThings.Campus.Keys)
                sb.Append($"<option value=\"{key}\">{AlreadyKnownThings.Campus[key]}</option>");
            sb.Append("</select>");
            var divisions = sb.ToRawHtml();
            sb.Clear();

            // Building list
            sb.Append("<select class=\"form-control\" id=\"buildings\">");
            AlreadyKnownThings.Buildings.ForEach((o) => sb.Append(o.ToString("option")));
            sb.Append("</select>");
            var buildings = sb.ToRawHtml();
            sb.Clear();

            // Start Class
            sb.Append("<select class=\"form-control\" onchange=\"changeClassList()\" id=\"startclass\">");
            for (int i = 1; i <= 11; i++)
                sb.Append($"<option class=\"startclass\" value=\"{i}\" on>从第{i}节</option>");
            sb.Append("</select>");
            var startclass = sb.ToRawHtml();
            sb.Clear();

            // End class
            sb.Append("<select  class=\"form-control\" id=\"endclass\">");
            for (int i = 1; i <= 11; i++)
                sb.Append($"<option class=\"endclass\" value=\"{i}\">到第{i}节</option>");
            sb.Append("</select>");
            var endclass = sb.ToRawHtml();
            sb.Clear();

            // Html document
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
                                divisions.WrapFormGroup(),
                                buildings.WrapFormGroup(),
                                startclass.WrapFormGroup(),
                                endclass.WrapFormGroup(),
                            }
                        },
                        Children =
                        {
                            "<table class=\"table\" id=\"rooms\">" +
                            "<tr><th>教室名称</th><th>容量</th><th>说明</th></tr>" +
                            "</table>".ToRawHtml()
                        }
                    }
                },
                JavaScript =
                {
                    "function getList() { var buildings = $(\"#buildings\").val(); var campus = $(\"#campus\").val(); $(\"#buildings\").children().hide(); $(\"option[data-campus='\"+campus+\"']\").show(); $(\"#buildings\").val($(\"option[data-campus='\"+campus+\"']:visible:first\")[0].value); }",
                    "function changeClassList() { var start = $(\"#startclass\").val(); $(\".endclass\").show(); $(\".endclass:lt(\"+start+\")\").hide(); $(\"#endclass\").val(start); }",
                    "function p(s) { return s < 10 ? (\"0\"+s) : (\"\"+s); }",
                    "function getdata() { var myDate = new Date(); var year = myDate.getFullYear(); var month = myDate.getMonth()+1; var date = myDate.getDate(); var time = year+\"-\"+p(month)+\"-\"+p(date); var bid = $(\"#buildings\").val(); var start = $(\"#startclass\").val(); var cs = 0; var end = $(\"#endclass\").val(); for (var i = start; i <= end; i++) { cs += Math.pow(i,2); } invokeCSharpAction(\"time \"+time+\" bid \"+bid+\" cs \"+cs); }",
                    "function callback(resp) { $(\".item\").remove(); for (var p = 0; p < resp.value.length; p++) { $(\"#rooms\").append('<tr class=\"item\" id=\"'+resp.value[p].roomId+'\"><td>'+resp.value[p].fullName.split(\"#\")[1]+\"</td><td>\"+resp.value[p].volume+\"</td><td>\"+(resp.value[p].notes==null?\"\":resp.value[p].notes)+\"</td>\"+\"</tr>\"); } }"
                }
            };

            Menu.Add(new InfoEntranceMenu("查询", new Command(() => Evaluate("getdata()")), "\uE721"));
        }
        
        public override async Task Receive(string data)
        {
            if(!data.StartsWith("time"))
            {
                await ShowMessage("查空教室", "请选择合法数据！", "知道了");
            }
            else
            {
                var Res = data.Split(' ');
                Today = Res[1];
                Bid = Res[3];
                Cs = Res[5];
                SetIsBusy(true, "信息查询中……");

                try
                {
                    var LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
                    Evaluate($"callback({LastReport})");
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.Timeout)
                    {
                        SetIsBusy(false);
                        await ShowMessage("错误", "连接超时，请重试。");
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
        }
    }
}
