using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JsonException = Newtonsoft.Json.JsonException;
using Command = Xamarin.Forms.Command;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("学院开课情况查询", "看看可以去蹭哪些课吧~", EntranceType.InfoEntrance)]
    class ClassSchedule : IInfoEntrance
    {
        public Bootstrap HtmlDocument { get; set; }
        public IViewResponse Binding { get; set; }
        public Action<string> Evaluate { get; set; }
        public string LastReport { get; set; }
        public List<InfoEntranceMenu> Menu { get; set; } = new List<InfoEntranceMenu>();

        private int termId = 135;
        private int schId = 101;
        private int tcmType = -1;
        private int lessonId;
        private bool is_busy = false;
        
        public string ScriptFileUri => "service/res.do";
        public bool IsPost => true;
        public string PostValue => $"{{\"tag\":\"lesson@globalStore\",\"branch\":\"default\",\"params\":{{\"termId\":{termId},\"schId\":\"{schId}\"{(tcmType == -1 ? "" : ",\"tcmType\":\""+ tcmType +"\"")}}},\"orderBy\":\"courseInfo.courName, extLessonNo\"}}";
        public string PostDetail => $"{{\"tag\":\"teachClassMaster@selectResultAdjust\",\"branch\":\"byLesson\",\"params\":{{\"lessonId\":\"{lessonId}\"}}}}";
        public string StorageFile => "No storage";

        public Task Execute() { throw new InvalidOperationException(); }
        public void Parse() { throw new InvalidOperationException(); }

        public ClassSchedule()
        {
            var sb = new StringBuilder();

            sb.Append("<select class=\"form-control\" id=\"schId\">");
            foreach (var opt in AlreadyKnownThings.Colleges)
            {
                sb.Append($"<option value=\"{opt.Id}\">{opt.Name}</option>");
            }
            sb.Append("</select>");
            var sch = (RawHtml)sb.ToString();
            sb.Clear();

            sb.Append("<select class=\"form-control\" id=\"tcmType\">");
            sb.Append("<option value=\"-1\" selected>任意类型</option>");
            sb.Append("<option value=\"3080\">专业课</option>");
            sb.Append("<option value=\"3082\">重修</option>");
            sb.Append("<option value=\"3084\">体育课</option>");
            sb.Append("<option value=\"3085\">校选修课</option>");
            sb.Append("</select>");
            var tcmt = (RawHtml)sb.ToString();
            sb.Clear();

            sb.Append("<select class=\"form-control\" id=\"termId\">");
            sb.Append("<option value=\"135\" selected>2018-2019学年第1学期</option>");
            sb.Append("<option value=\"134\">2017-2018学年第2学期</option>");
            sb.Append("<option value=\"133\">2017-2018学年第1学期</option>");
            sb.Append("<option value=\"132\">2016-2017学年第2学期</option>");
            sb.Append("<option value=\"131\">2016-2017学年第1学期</option>");
            sb.Append("<option value=\"130\">2015-2016学年第2学期</option>");
            sb.Append("</select>");
            var term = (RawHtml)sb.ToString();
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
                                new FormGroup { Children = { term } },
                                new FormGroup { Children = { sch } },
                                new FormGroup { Children = { tcmt } },
                            }
                        },
                        Children =
                        {
                            (RawHtml) "<table class=\"table table-responsive\" style=\"max-height:40em;overflow-y:auto;\"><thead><tr>",
                            (RawHtml) ( "<th scope=\"col\" style=\"min-width:17em\">课程名称</th>" +
                                        "<th scope=\"col\" style=\"min-width:8em\">课程代码</th>" +
                                        "<th scope=\"col\" style=\"min-width:7em\">类别</th>" +
                                        "<th scope=\"col\" style=\"min-width:9em\">课程负责人</th>"),
                            (RawHtml) "</tr></thead><tbody id=\"lessonList\"></tbody></table>",
                            (RawHtml) "<table class=\"table table-responsive\" id=\"lessonIdP\"><thead><tr>",
                            (RawHtml) ( "<th scope=\"col\" style=\"min-width:15em\">上课时间地点</th>" +
                                        "<th scope=\"col\" style=\"min-width:6em\">任课教师</th>" +
                                        "<th scope=\"col\" style=\"min-width:6em\">选课人数</th>"),
                            (RawHtml) "</tr></thead><tbody id=\"lessonId\"></tbody></table>",
                        }
                    }
                },
                JavaScript =
                {
                    "$('#schId').on('change',function(){invokeCSharpAction('schId='+$(this).val())});",
                    "$('#tcmType').on('change',function(){invokeCSharpAction('tcmType='+$(this).val())});",
                    "$('#termId').on('change',function(){invokeCSharpAction('termId='+$(this).val())});",
                    "function showDetail(obj,v){" +
                        "$('#lessonList .table-primary').removeClass('table-primary');" +
                        "$(obj).addClass('table-primary');" +
                        "invokeCSharpAction('lessonId='+v);" +
                    "};",
                    "$(function(){$('#schId').val('101')});"
                }
            };

            Menu.Add(new InfoEntranceMenu("加载", new Command(SolveLessonList), "\uE721"));
        }

        async void SolveLessonList()
        {
            if (is_busy) return;
            Binding.SetIsBusy(is_busy = true, "正在加载课程列表……");
            Evaluate?.Invoke($"$('#lessonList').html('<tr><td colspan=\"4\">正在加载…</td></tr>');$('#lessonId').html('')");

            try
            {
                LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
                var lists = Helper.JSON<RootObject<CollegeCourse>>(LastReport);
                var sb = new StringBuilder();
                foreach (var opt in lists.value)
                {
                    sb.Append($"<tr ondblclick=\"showDetail(this,{opt.lessonId})\">" +
                        $"<td>{opt.courseInfo.courName}</td>" +
                        $"<td>{opt.courseInfo.extCourseNo}</td><td>" +
                        (opt.extLessonNo.EndsWith("-cx") ? "重修" : AlreadyKnownThings.Type5Name(opt.courseInfo.type5)) +
                        $"</td><td>{opt.leader.name}</td></tr>");
                }
                Evaluate?.Invoke($"$('#lessonList').html('{sb.ToString()}')");
                sb.Clear();
                Binding.SetIsBusy(is_busy = false);
            }
            catch (JsonException)
            {
                Binding.SetIsBusy(is_busy = false);
                await Binding.ShowMessage("提示", "加载课程列表失败。");
            }
        }

        async void SolveLessonId()
        {
            if (is_busy) return;
            Binding.SetIsBusy(is_busy = true, "正在加载教学班列表……");
            Evaluate?.Invoke($"$('#lessonId').html('<tr><td colspan=\"4\">正在加载…</td></tr>')");

            try
            {
                LastReport = await Core.App.Service.Post(ScriptFileUri, PostDetail);
                var lists = Helper.JSON<RootObject<LessonIdList>>(LastReport);
                var sb = new StringBuilder();
                foreach (var opt in lists.value)
                {
                    sb.Append("<tr><td>");
                    foreach (var i in opt.lessonSchedules)
                        sb.Append(i.timeBlock.name + i.classroom.fullName + "<br>");
                    if (opt.lessonSchedules.Length > 0) sb.Remove(sb.Length - 4, 4);
                    sb.Append("</td><td>");
                    foreach (var i in opt.lessonTeachers)
                        sb.Append(i.teacher.name + " ");
                    if (opt.lessonTeachers.Length > 0) sb.Remove(sb.Length - 1, 1);
                    sb.Append($"</td><td>{opt.studCnt}</td></tr>");
                }
                Evaluate?.Invoke($"$('#lessonId').html('{sb.ToString()}')");
                sb.Clear();
                Binding.SetIsBusy(is_busy = false);
            }
            catch (JsonException)
            {
                Binding.SetIsBusy(is_busy = false);
                await Binding.ShowMessage("提示", "加载教学班列表失败。");
            }
        }

        public void Receive(string data)
        {
            if (data.StartsWith("termId="))
            {
                termId = int.Parse(data.Substring(7));
            }
            else if (data.StartsWith("schId="))
            {
                schId = int.Parse(data.Substring(6));
            }
            else if (data.StartsWith("tcmType="))
            {
                tcmType = int.Parse(data.Substring(8));
            }
            else if (data.StartsWith("lessonId="))
            {
                lessonId = int.Parse(data.Substring(9));
                SolveLessonId();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
