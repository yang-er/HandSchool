using HandSchool.Internals;
using HandSchool.Internals.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HandSchool.JLU.InfoQuery
{
    /// <summary>
    /// 学院介绍查询的功能。
    /// </summary>
    /// <inheritdoc cref="BaseController" />
    /// <inheritdoc cref="IInfoEntrance" />
    [Entrance("JLU", "学院介绍查询", "查询各个学院的详细信息。", EntranceType.InfoEntrance)]
    internal class CollegeIntroduce : BaseController, IInfoEntrance
    {
        /// <summary>
        /// 使用的Bootstrap文档
        /// </summary>
        public Bootstrap HtmlDocument { get; set; }

        private readonly Lazy<ISchoolSystem> lazyService;
        private ISchoolSystem Service => lazyService.Value;

        private int schId = 101;
        const string serviceResourcesUrl = "service/res.do";
        private string PostValue => $"{{\"tag\":\"school@schoolSearch\",\"branch\":\"byId\",\"params\":{{\"schId\":\"{schId}\"}}}}";
        
        public CollegeIntroduce(Lazy<ISchoolSystem> service)
        {
            lazyService = service;

            var divisionSelect = new Select("division", AlreadyKnownThings.Division)
            {
                FirstKeyValuePair = new KeyValuePair<string, string>("*", "全部学部"),
                OnChanged = "getList()"
            };

            var campusSelect = new Select("campus", AlreadyKnownThings.Campus)
            {
                FirstKeyValuePair = new KeyValuePair<string, string>("*", "任意校区"),
                OnChanged = "getList()"
            };
            
            var sb = new StringBuilder();
            sb.Append("<select class=\"form-control\" id=\"schId\">");

            foreach (var college in AlreadyKnownThings.Colleges)
            {
                sb.Append($"<option value=\"{college.Id}\"");
                if (college.Campus != null) sb.Append($" data-campus=\"{college.Campus}\"");
                if (college.Division != null) sb.Append($" data-part=\"{college.Division}\"");
                sb.Append($">{college.Name}</option>");
            }
            
            sb.Append("</select>");
            var sch = sb.ToRawHtml();
            sb.Clear();

            sb.Append("<h4>名称</h4><p id=\"schoolName\">软件学院</p>");
            sb.Append("<h4>英文名称</h4><p id=\"englishName\">College of Software</p>");
            sb.Append("<h4>外部编号</h4><p id=\"extSchNo\">54</p>");
            sb.Append("<h4>校区</h4><p id=\"Icampus\">前卫校区</p>");
            sb.Append("<h4>学部</h4><p id=\"Idivision\">信息科学学部</p>");
            sb.Append("<h4>负责人</h4><p id=\"staff\">未设置</p>");
            sb.Append("<h4>联系电话</h4><p id=\"telephone\">学校很懒，什么也没有留下……</p>");
            sb.Append("<h4>院系主页</h4><p id=\"website\">学校很懒，什么也没有留下……</p>");
            sb.Append("<h4>院系介绍</h4><p id=\"introduction\">学校很懒，什么也没有留下……</p>");
            var bodyContent = sb.ToRawHtml();
            
            HtmlDocument = new Bootstrap
            {
                Children =
                {
                    new MasterDetail(new Form
                    {
                        divisionSelect.WrapFormGroup(),
                        campusSelect.WrapFormGroup(),
                        sch.WrapFormGroup()
                    }) { bodyContent }
                },
                JavaScript =
                {
                    "function getList() { var campus = $('#campus').val(); var selector = '#schId option' + (campus == '*' ? '' : '[data-campus=\"' + campus + '\"]'); var division = $('#division').val(); selector += (division == '*' ? '' : '[data-part=\"' + division + '\"]'); $('#schId > option').wrap('<span>').hide(); $(selector).unwrap().show(); }",
                    "function getSchId() { invokeCSharpAction('schId=' + $('#schId').val()); }",
                    "$('#schId').val('101')"
                }
            };

            Menu.Add(new HandSchool.Views.MenuEntry
            {
                Title = "查询",
                UWPIcon = "\uE721",
                Command = new CommandAction(() => Evaluate("getSchId()"))
            });
        }

        public override async Task Receive(string data)
        {
            if (data.StartsWith("schId"))
            {
                if (data == "schId=null")
                {
                    await RequestMessageAsync("信息查询", "未指定查询学院。");
                    return;
                }

                schId = int.Parse(data.Split('=')[1]);
                await Execute();
            }
            else
            {
                await RequestMessageAsync("信息查询", "未定义操作。");
                await RequestMessageAsync("信息查询", "未知响应：" + data);
            }
        }
        
        private static void CreateInfo(StringBuilder jsBuilder, CollegeInfo info)
        {
            info.schoolName = info.schoolName ?? "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#schoolName').text('" + info.schoolName + "');");

            info.englishName = info.englishName ?? "School is lazy, left nothing...";
            jsBuilder.Append("$('#englishName').text('" + info.englishName + "');");

            info.extSchNo = info.extSchNo ?? "??";
            jsBuilder.Append("$('#extSchNo').text('" + info.extSchNo + "');");

            info.campus = info.campus == null ? "未知" : AlreadyKnownThings.Campus[info.campus];
            jsBuilder.Append("$('#Icampus').text('" + info.campus + "');");

            info.division = info.division == null ? "未知" : AlreadyKnownThings.Division[info.division];
            jsBuilder.Append("$('#Idivision').text('" + info.division + "');");

            info.staff = info.staff ?? new Staff { name = "未设置" };
            jsBuilder.Append("$('#staff').text('" + info.staff.name + "');");

            info.telephone = info.telephone ?? "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#telephone').text('" + info.telephone + "');");

            info.website = info.website ?? "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#website').text('" + info.website + "');");

            info.introduction = info.introduction ?? "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#introduction').text('" + info.introduction + "');");
        }

        private async Task Execute()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var lastReport = await Service.Post(serviceResourcesUrl, PostValue);
                var obj = lastReport.ParseJSON<RootObject<CollegeInfo>>();
                var jsBuilder = new StringBuilder();
                CreateInfo(jsBuilder, obj.value[0]);
                Evaluate?.Invoke(jsBuilder.ToString());
            }
            catch (WebsException ex)
            {
                if (ex.Status != WebStatus.Timeout) throw;
                await RequestMessageAsync("提示", "加载失败，连接超时，请重试。");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
