using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("JLU", "查空教室", "没地方自习？试试这个吧。", EntranceType.InfoEntrance)]
    internal class EmptyRoom : BaseController, IInfoEntrance
    {
        public Bootstrap HtmlDocument { get; set; }

        const string serviceResourceUrl = "service/res.do";
        private string Cs = "";
        private string Bid = "";
        private string Today = "";

        private string PostValue => $"{{\"tag\":\"roomIdle@roomUsage\",\"branch\":\"default\",\"params\":{{\"termId\":`term`,\"bid\":\"{Bid}\",\"rname\":\"\",\"dateActual\":{{}},\"cs\":{Cs},\"d_actual\":\"{Today}T00:00:00+08:00\"}}}}";
        
        public EmptyRoom()
        {
            var campusSelect = new Select("campus", AlreadyKnownThings.Campus)
            {
                OnChanged = "getList()"
            };
            
            // Building list
            var sb = new StringBuilder();
            sb.Append("<select class=\"form-control\" id=\"buildings\">");

            foreach (var building in AlreadyKnownThings.Buildings)
            {
                sb.Append($"<option value=\"{building.Id}\"");
                if (building.Campus != null) sb.Append($" data-campus=\"{building.Campus}\"");
                sb.Append($">{building.Name}</option>");
            }
            
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
                    new MasterDetail(new Form
                    {
                        campusSelect.WrapFormGroup(),
                        buildings.WrapFormGroup(),
                        startclass.WrapFormGroup(),
                        endclass.WrapFormGroup(),
                    }) {
                        "<table class=\"table\" id=\"rooms\">" +
                        "<tr><th>教室名称</th><th>容量</th><th>说明</th></tr>" +
                        "</table>".ToRawHtml()
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

        private async Task Execute()
        {
            if (IsBusy) return; IsBusy = true;

            try
            {
                var LastReport = await Core.App.Service.Post(serviceResourceUrl, PostValue);
                Evaluate($"callback({LastReport})");
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.Timeout) throw;
                await this.ShowTimeoutMessage();
            }

            IsBusy = false;
        }

        public override async Task Receive(string data)
        {
            if(!data.StartsWith("time"))
            {
                await RequestMessageAsync("查空教室", "请选择合法数据！", "知道了");
            }
            else
            {
                var Res = data.Split(' ');
                Today = Res[1];
                Bid = Res[3];
                Cs = Res[5];
                await Execute();
            }
        }
    }
}
