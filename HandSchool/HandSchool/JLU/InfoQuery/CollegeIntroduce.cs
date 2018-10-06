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
    [Entrance("学院介绍查询", "查询各个学院的详细信息。", EntranceType.InfoEntrance)]
    class CollegeIntroduce : BaseController, IInfoEntrance
    {
        private int schId = 101;
        
        public Bootstrap HtmlDocument { get; set; }
        public string ScriptFileUri => "service/res.do";
        public string PostValue => $"{{\"tag\":\"school@schoolSearch\",\"branch\":\"byId\",\"params\":{{\"schId\":\"{schId}\"}}}}";
        
        public CollegeIntroduce()
        {
            var sb = new StringBuilder();

            sb.Append("<select class=\"form-control\" id=\"division\" onchange=\"getList()\"><option value=\"*\">全部学部</option>");
            foreach (string key in AlreadyKnownThings.Division.Keys)
                sb.Append($"<option value=\"{key}\">{AlreadyKnownThings.Division[key]}</option>");
            sb.Append("</select>");
            var divisions = sb.ToRawHtml();
            sb.Clear();

            sb.Append("<select class=\"form-control\" id=\"campus\" onchange=\"getList()\"><option value=\"*\">任意校区</option>");
            foreach (string key in AlreadyKnownThings.Campus.Keys)
                sb.Append($"<option value=\"{key}\">{AlreadyKnownThings.Campus[key]}</option>");
            sb.Append("</select>");
            var campus = sb.ToRawHtml();
            sb.Clear();

            sb.Append("<select class=\"form-control\" id=\"schId\">");
            AlreadyKnownThings.Colleges.ForEach((o) => sb.Append(o.ToString("option")));
            sb.Append("</select>");
            var sch = sb.ToRawHtml();
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
                                divisions.WrapFormGroup(),
                                campus.WrapFormGroup(),
                                sch.WrapFormGroup()
                            }
                        },
                        Children =
                        {
                            "<h4>名称</h4><p id=\"schoolName\">软件学院</p>" +
                            "<h4>英文名称</h4><p id=\"englishName\">College of Software</p>" +
                            "<h4>外部编号</h4><p id=\"extSchNo\">54</p>" +
                            "<h4>校区</h4><p id=\"Icampus\">前卫校区</p>" +
                            "<h4>学部</h4><p id=\"Idivision\">信息科学学部</p>" +
                            "<h4>负责人</h4><p id=\"staff\">未设置</p>" +
                            "<h4>联系电话</h4><p id=\"telephone\">学校很懒，什么也没有留下……</p>" +
                            "<h4>院系主页</h4><p id=\"website\">学校很懒，什么也没有留下……</p>" +
                            "<h4>院系介绍</h4><p id=\"introduction\">学校很懒，什么也没有留下……</p>".ToRawHtml()
                        }
                    }
                },
                JavaScript =
                {
                    "function getList() { var campus = $('#campus').val(); var selector = '#schId option' + (campus == '*' ? '' : '[data-campus=\"' + campus + '\"]'); var division = $('#division').val(); selector += (division == '*' ? '' : '[data-part=\"' + division + '\"]'); $('#schId > option').wrap('<span>').hide(); $(selector).unwrap().show(); }",
                    "function getSchId() { invokeCSharpAction('schId=' + $('#schId').val()); }",
                    "$('#schId').val('101')"
                }
            };

            Menu.Add(new InfoEntranceMenu("查询", new Command(() => Evaluate("getSchId()")), "\uE721"));
        }

        public override async Task Receive(string data)
        {
            if (data.StartsWith("schId"))
            {
                if (data == "schId=null")
                {
                    await ShowMessage("信息查询", "未指定查询学院。", "知道了");
                    return;
                }

                schId = int.Parse(data.Split('=')[1]);
                await Execute();
            }
            else
            {
                await ShowMessage("信息查询", "未定义操作。", "知道了");
                await ShowMessage("信息查询", "未知响应：" + data);
            }
        }
        
        private void CreateInfo(StringBuilder jsBuilder, CollegeInfo info)
        {
            if (info.schoolName == null) info.schoolName = "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#schoolName').text('" + info.schoolName + "');");

            if (info.englishName == null) info.englishName = "School is lazy, left nothing...";
            jsBuilder.Append("$('#englishName').text('" + info.englishName + "');");

            if (info.extSchNo == null) info.extSchNo = "??";
            jsBuilder.Append("$('#extSchNo').text('" + info.extSchNo + "');");

            if (info.campus == null) info.campus = "未知";
            else info.campus = AlreadyKnownThings.Campus[info.campus];
            jsBuilder.Append("$('#Icampus').text('" + info.campus + "');");

            if (info.division == null) info.division = "未知";
            else info.division = AlreadyKnownThings.Division[info.division];
            jsBuilder.Append("$('#Idivision').text('" + info.division + "');");

            if (info.staff == null) info.staff = new Staff { name = "未设置" };
            jsBuilder.Append("$('#staff').text('" + info.staff.name + "');");

            if (info.telephone == null) info.telephone = "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#telephone').text('" + info.telephone + "');");

            if (info.website == null) info.website = "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#website').text('" + info.website + "');");

            if (info.introduction == null) info.introduction = "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#introduction').text('" + info.introduction + "');");
        }

        public async Task Execute()
        {
            if (IsBusy) return;
            SetIsBusy(true, "正在加载信息……");
            string LastReport;

            try
            {
                LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    SetIsBusy(false);
                    await View.ShowMessage("错误", "连接超时，请重试。");
                    return;
                }
                else
                {
                    throw ex;
                }
            }

            SetIsBusy(false);
            var obj = LastReport.ParseJSON<RootObject<CollegeInfo>>();
            var jsBuilder = new StringBuilder();
            CreateInfo(jsBuilder, obj.value[0]);
            Evaluate(jsBuilder.ToString());
        }
    }
}
