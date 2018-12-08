using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Command = Xamarin.Forms.Command;
using JsonException = Newtonsoft.Json.JsonException;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("学院开课情况查询", "看看可以去蹭哪些课吧~", EntranceType.InfoEntrance)]
    class ClassSchedule : BaseController, IInfoEntrance
    {
        public Bootstrap HtmlDocument { get; set; }

        private int termId = 135;
        private int schId = 101;
        private int tcmType = -1;
        private int lessonId;
        
        const string ScriptFileUri = "service/res.do";
        string PostValue => $"{{\"tag\":\"lesson@globalStore\",\"branch\":\"default\",\"params\":{{\"termId\":`term`,\"schId\":\"{schId}\"{(tcmType == -1 ? "" : ",\"tcmType\":\""+ tcmType +"\"")}}},\"orderBy\":\"courseInfo.courName, extLessonNo\"}}";
        string PostDetail => $"{{\"tag\":\"teachClassMaster@selectResultAdjust\",\"branch\":\"byLesson\",\"params\":{{\"lessonId\":\"{lessonId}\"}}}}";

        public ClassSchedule()
        {
            var sb = new StringBuilder();

            sb.Append("<select class=\"form-control\" id=\"schId\">");
            foreach (var opt in AlreadyKnownThings.Colleges)
                sb.Append($"<option value=\"{opt.Id}\">{opt.Name}</option>");
            sb.Append("</select>");
            var sch = sb.ToRawHtml();
            sb.Clear();

            sb.Append("<select class=\"form-control\" id=\"tcmType\">");
            sb.Append("<option value=\"-1\" selected>任意类型</option>");
            sb.Append("<option value=\"3080\">专业课</option>");
            sb.Append("<option value=\"3082\">重修</option>");
            sb.Append("<option value=\"3084\">体育课</option>");
            sb.Append("<option value=\"3085\">校选修课</option>");
            sb.Append("</select>");
            var tcmt = sb.ToRawHtml();
            sb.Clear();

            sb.Append("<select class=\"form-control\" id=\"termId\">");
            sb.Append("<option value=\"136\">2018-2019学年第2学期</option>");
            sb.Append("<option value=\"135\" selected>2018-2019学年第1学期</option>");
            sb.Append("<option value=\"134\">2017-2018学年第2学期</option>");
            sb.Append("<option value=\"133\">2017-2018学年第1学期</option>");
            sb.Append("<option value=\"132\">2016-2017学年第2学期</option>");
            sb.Append("<option value=\"131\">2016-2017学年第1学期</option>");
            sb.Append("<option value=\"130\">2015-2016学年第2学期</option>");
            sb.Append("</select>");
            var term = sb.ToRawHtml();
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
                                term.WrapFormGroup(),
                                sch.WrapFormGroup(),
                                tcmt.WrapFormGroup(),
                            }
                        },
                        Children =
                        {
                            "<table class=\"table table-responsive\" id=\"lessonIdP\"><thead><tr>" +
                            "<th scope=\"col\" style=\"min-width:15em\">上课时间地点</th>" +
                            "<th scope=\"col\" style=\"min-width:6em\">任课教师</th>" +
                            "<th scope=\"col\" style=\"min-width:6em\">选课人数</th>" +
                            "</tr></thead><tbody id=\"lessonId\"></tbody></table>" +
                            "<table class=\"table table-responsive\"><thead><tr>" +
                            "<th scope=\"col\" style=\"min-width:17em\">课程名称</th>" +
                            "<th scope=\"col\" style=\"min-width:8em\">课程代码</th>" +
                            "<th scope=\"col\" style=\"min-width:7em\">类别</th>" +
                            "<th scope=\"col\" style=\"min-width:9em\">课程负责人</th>" +
                            "</tr></thead><tbody id=\"lessonList\"></tbody></table>".ToRawHtml()
                        }
                    }
                },
                JavaScript =
                {
                    "$('#schId').on('change',function(){invokeCSharpAction('schId='+$(this).val())});",
                    "$('#tcmType').on('change',function(){invokeCSharpAction('tcmType='+$(this).val())});",
                    "$('#termId').on('change',function(){invokeCSharpAction('termId='+$(this).val())});",
                    "function showDetail(v){invokeCSharpAction('lessonId='+v)};",
                    "$(function(){$('#schId').val('101')});"
                }
            };

            Menu.Add(new InfoEntranceMenu("加载", new Command(SolveLessonList), "\uE721"));
        }

        private async void SolveLessonList()
        {
            if (IsBusy) return;
            SetIsBusy(true, "正在加载课程列表……");
            Evaluate?.Invoke($"$('#lessonList').html('<tr><td colspan=\"4\">正在加载…</td></tr>');$('#lessonId').html('')");

            try
            {
                var LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
                var lists = LastReport.ParseJSON<RootObject<CollegeCourse>>();
                var sb = new StringBuilder();
                foreach (var opt in lists.value)
                {
                    sb.Append(
                        $"<tr><td><a class=\"linked-a\" onclick=\"showDetail({opt.lessonId})\">{opt.courseInfo.courName}</a></td>" +
                        $"<td>{opt.courseInfo.extCourseNo}</td><td>" +
                        (opt.extLessonNo.EndsWith("-cx") ? "重修" : AlreadyKnownThings.Type5Name(opt.courseInfo.type5)) +
                        $"</td><td>{opt.leader.name}</td></tr>");
                }
                Evaluate?.Invoke($"$('#lessonList').html('{sb.ToString()}')");
                sb.Clear();
                SetIsBusy(false);
            }
            catch (JsonException)
            {
                SetIsBusy(false);
                await ShowMessage("提示", "加载课程列表失败。");
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

        private async void SolveLessonId()
        {
            if (IsBusy) return;
            SetIsBusy(true, "正在加载教学班列表……");
            Evaluate?.Invoke($"$('#lessonId').html('<tr><td colspan=\"4\">正在加载…</td></tr>')");

            try
            {
                var LastReport = await Core.App.Service.Post(ScriptFileUri, PostDetail);
                var lists = LastReport.ParseJSON<RootObject<LessonIdList>>();
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
                SetIsBusy(false);
            }
            catch (JsonException)
            {
                SetIsBusy(false);
                await ShowMessage("提示", "加载教学班列表失败。");
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

        public override async Task Receive(string data)
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
                await ShowMessage("错误", "未知响应：" + data);
            }
        }
    }
}
