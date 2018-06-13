using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Text;
using static HandSchool.Internal.Helper;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.JLU.InfoQuery
{
    class EmptyRoom : IInfoEntrance
    {
        public string Description => "没地方自习?试试这个吧";

        private RootObject<List<RoomInfo>> obj;
        public List<string> TableHeader { get; set; }
        public Bootstrap HtmlDocument { get; set; }
        public IViewResponse Binding { get; set; }
        public Action<string> Evaluate { get; set; }
        public List<InfoEntranceMenu> Menu { get; set; } = new List<InfoEntranceMenu>();

        public string Name => "查空教室";
        public string ScriptFileUri => "service/res.do";
        public bool IsPost => true;
        public string PostValue => $"{{\"tag\":\"roomIdle@roomUsage\",\"branch\":\"default\",\"params\":{{\"termId\":{TermId},\"bid\":\"{Bid}\",\"rname\":\"\",\"dateActual\":{{}},\"cs\":{Cs},\"d_actual\":\"{Today}T00:00:00+08:00\"}}}}";//today:2018-06-12 termid,bid,cs
        public string StorageFile => "No storage";
        public string Cs = "";
        public string Bid = "";
        public string Today = "";
        public string TermId = "";
        public string LastReport{get;set;}

        public EmptyRoom()
        {
            TermId = Core.App.Service.AttachInfomation["term"];
            var sb = new StringBuilder();
            sb.Append("<select class=\"form-control\" id=\"campus\" onchange=\"getList()\">");
            foreach (string key in AlreadyKnownThings.Campus.Keys)
            {
                sb.Append($"<option value=\"{key}\">{AlreadyKnownThings.Campus[key]}</option>");
            }
            sb.Append("</select>");
            var divisions = new RawHtml { Raw = sb.ToString() };
            sb.Clear();

            sb.Append("<select class=\"form-control\" id=\"buildings\">");
            AlreadyKnownThings.Buildings.ForEach((o) => sb.Append(o.ToString("option")));
            sb.Append("</select>");

            var buildings = new RawHtml { Raw = sb.ToString() };
            sb.Clear();
            
            sb.Append("<select class=\"form-control\" onchange=\"changeClassList()\" id=\"startclass\">");
            for(int i=1;i<=11;i++)
            {
                sb.Append($"<option class=\"startclass\" value=\"{i}\" on>从第{i}节</option>");
            }
            sb.Append("</select>");
            var startclass = new RawHtml { Raw = sb.ToString() };
            sb.Clear();
            sb.Append("<select  class=\"form-control\" id=\"endclass\">");
            for (int i = 1; i <= 11; i++)
            {
                sb.Append($"<option class=\"endclass\"  value=\"{i}\">到第{i}节</option>");
            }
            sb.Append("</select>");
            var endclass = new RawHtml { Raw = sb.ToString() };
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
                                new FormGroup { Children = { divisions } },
                                new FormGroup { Children = { buildings } },
                                new FormGroup { Children = { startclass } },
                                new FormGroup { Children = { endclass } },
                            }
                        },
                        Children =
                        {
                                 (RawHtml) "<table class=\"table\" id=\"rooms\"><tr><th>教室名称</th><th>容量</th>+<th>说明</th></tr></table>"
                        }
                    }
                },
                JavaScript =
                        {
                            "function getList(){var buildings=$(\"#buildings\").val();var campus=$(\"#campus\").val();var a=$(\"option[data-campus='*']\");var b=$(\"option\");$(\"#buildings\").children().hide();$(\"option[data-campus='\"+campus+\"']\").show();$(\"#buildings\").val($(\"option[data-campus='\"+campus+\"']:visible:first\")[0].value)}function changeClassList(){var start=$(\"#startclass\").val();$(\".endclass\").show();$(\".endclass:lt(\"+start+\")\").hide();var a=$(\"#endclass\");$(\"#endclass\").val(start)}function p(s){return s<10?\"0\"+s:s}function getdata(){var myDate=new Date();var year=myDate.getFullYear();var month=myDate.getMonth()+1;var date=myDate.getDate();var time=year+\"-\"+p(month)+\"-\"+p(date);var bid=$(\"#buildings\").val();var start=$(\"#startclass\").val();var cs=0;var end=$(\"#endclass\").val();for(var i=start;i<=end;i++){cs+=Math.pow(i,2)}alert(\"time \"+time+\" bid \"+bid+\" cs \"+cs);invokeCSharpAction(\"time \"+time+\" bid \"+bid+\" cs \"+cs)}function callback(resp){$(\".item\").remove();for(var p=0;p<resp.value.length;p++){$(\"#rooms\").append('<tr class=\\\"item\\\" id=\"'+resp.value[p].roomId+'\"><td>'+resp.value[p].fullName.split(\"#\")[1]+\"</td><td>\"+resp.value[p].volume+\"</td><td>\"+(resp.value[p].notes==null?\"\":resp.value[p].notes)+\"</td>\"+\"</tr>\")}};"
                        }
            };
            Menu.Add(new InfoEntranceMenu("查询", new Command(() => Evaluate("getdata()")), "\uE721"));
        }
        public async Task Execute()
        {
            LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
            var RoomList = JSON<RootObject<RoomInfo>>(LastReport);
            Evaluate($"callback({LastReport})");
            
        }

        public void Parse()
        {
            throw new NotImplementedException();
        }

        public async void Receive(string data)
        {
            if(!data.StartsWith("time"))
            {
                await Binding.ShowMessage("查空教室", "请选择合法数据!", "知道了");
                return;
            }
            else
            {
                var Res=data.Split(' ');
                Today = Res[1];
                Bid = Res[3];
                Cs = Res[5];
                Binding.SetIsBusy(true, "信息查询中……");
                await Execute();
                Binding.SetIsBusy(false);

            }

        }
    }
}
