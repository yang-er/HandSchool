using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("学院介绍查询", "查询各个学院的详细信息。", EntranceType.InfoEntrance)]
    class CollegeIntroduce : IInfoEntrance
    {
        private RootObject<CollegeInfo> obj;
        private int schId = 101;
        
        public Bootstrap HtmlDocument { get; set; }
        public string LastReport { get; private set; }
        public string StorageFile => "No storage";
        public bool IsPost => true;
        public string ScriptFileUri => "service/res.do";
        public string PostValue => $"{{\"tag\":\"school@schoolSearch\",\"branch\":\"byId\",\"params\":{{\"schId\":\"{schId}\"}}}}";
        public List<InfoEntranceMenu> Menu { get; set; } = new List<InfoEntranceMenu>();

        public IViewResponse Binding { get; set; }
        public Action<string> Evaluate { get; set; }
        
        public CollegeIntroduce()
        {
            var sb = new StringBuilder();
            sb.Append("<select class=\"form-control\" id=\"division\" onchange=\"getList()\"><option value=\"*\">全部学部</option>");
            foreach (string key in AlreadyKnownThings.Division.Keys)
                sb.Append($"<option value=\"{key}\">{AlreadyKnownThings.Division[key]}</option>");
            sb.Append("</select>");
            var divisions = new RawHtml { Raw = sb.ToString() };
            sb.Clear();
            sb.Append("<select class=\"form-control\" id=\"campus\" onchange=\"getList()\"><option value=\"*\">任意校区</option>");
            foreach (string key in AlreadyKnownThings.Campus.Keys)
                sb.Append($"<option value=\"{key}\">{AlreadyKnownThings.Campus[key]}</option>");
            sb.Append("</select>");
            var campus = new RawHtml { Raw = sb.ToString() };
            sb.Clear();
            sb.Append("<select class=\"form-control\" id=\"schId\">");
            AlreadyKnownThings.Colleges.ForEach((o) => sb.Append(o.ToString("option")));
            sb.Append("</select>");
            var sch = new RawHtml { Raw = sb.ToString() };
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
                                new FormGroup { Children = { campus } },
                                new FormGroup { Children = { sch } }
                            }
                        },
                        Children =
                        {
                            new RawHtml { Raw = "<h4>名称</h4><p id=\"schoolName\">软件学院</p><h4>英文名称</h4><p id=\"englishName\">College of Software</p><h4>外部编号</h4><p id=\"extSchNo\">54</p><h4>校区</h4><p id=\"Icampus\">前卫校区</p><h4>学部</h4><p id=\"Idivision\">信息科学学部</p><h4>负责人</h4><p id=\"staff\">未设置</p><h4>联系电话</h4><p id=\"telephone\">学校很懒，什么也没有留下……</p><h4>院系主页</h4><p id=\"website\">学校很懒，什么也没有留下……</p><h4>院系介绍</h4><p id=\"introduction\">学校很懒，什么也没有留下……</p>" }
                        }
                    }
                },
                JavaScript =
                {
                    "function getList() { var campus = $('#campus').val(); var selector = '#schId option' + (campus == '*' ? '' : '[data-campus=\"' + campus + '\"]'); var division = $('#division').val(); selector += (division == '*' ? '' : '[data-part=\"' + division + '\"]'); $('#schId > option').wrap('<span>').hide(); $(selector).unwrap().show(); }; $('#schId').val('101'); function getSchId() { invokeCSharpAction('schId=' + $('#schId').val()); }; "
                }
            };
            Menu.Add(new InfoEntranceMenu("查询", new Command(() => Evaluate("getSchId()")), "\uE721"));
        }

        public async void Receive(string data)
        {
            System.Diagnostics.Debug.WriteLine(data);
            if (data.StartsWith("schId"))
            {
                if (data == "schId=null")
                {
                    await Binding.ShowMessage("信息查询", "未指定查询学院。", "知道了");
                    return;
                }

                schId = int.Parse(data.Split('=')[1]);
                Binding.SetIsBusy(true, "信息查询中……");
                await Execute();
                Binding.SetIsBusy(false);
            }
            else
            {
                await Binding.ShowMessage("信息查询", "未定义操作。", "知道了");
                await Binding.ShowMessage("信息查询", "未知响应：" + data);
            }
        }
        
        public async Task Execute()
        {
            LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
            obj = LastReport.ParseJSON<RootObject<CollegeInfo>>();
            var jsBuilder = new StringBuilder();
            if (obj.value[0].schoolName == null) obj.value[0].schoolName = "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#schoolName').text('" + obj.value[0].schoolName + "');");
            if (obj.value[0].englishName == null) obj.value[0].englishName = "School is lazy, left nothing...";
            jsBuilder.Append("$('#englishName').text('" + obj.value[0].englishName + "');");
            if (obj.value[0].extSchNo == null) obj.value[0].extSchNo = "??";
            jsBuilder.Append("$('#extSchNo').text('" + obj.value[0].extSchNo + "');");
            if (obj.value[0].campus == null) obj.value[0].campus = "未知"; else obj.value[0].campus = AlreadyKnownThings.Campus[obj.value[0].campus];
            jsBuilder.Append("$('#Icampus').text('" + obj.value[0].campus + "');");
            if (obj.value[0].division == null) obj.value[0].division = "未知"; else obj.value[0].division = AlreadyKnownThings.Division[obj.value[0].division];
            jsBuilder.Append("$('#Idivision').text('" + obj.value[0].division + "');");
            if (obj.value[0].staff == null) obj.value[0].staff = new Staff { name = "未设置" };
            jsBuilder.Append("$('#staff').text('" + obj.value[0].staff.name + "');");
            if (obj.value[0].telephone == null) obj.value[0].telephone = "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#telephone').text('" + obj.value[0].telephone + "');");
            if (obj.value[0].website == null) obj.value[0].website = "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#website').text('" + obj.value[0].website + "');");
            if (obj.value[0].introduction == null) obj.value[0].introduction = "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#introduction').text('" + obj.value[0].introduction + "');");
            Evaluate(jsBuilder.ToString());
        }
        public void Parse() { }
    }
}
