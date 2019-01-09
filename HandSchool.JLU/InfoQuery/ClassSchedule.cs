using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Command = Xamarin.Forms.Command;
using JsonException = Newtonsoft.Json.JsonException;

namespace HandSchool.JLU.InfoQuery
{
    /// <summary>
    /// 学院开课情况查询功能。
    /// </summary>
    /// <inheritdoc cref="BaseController" />
    /// <inheritdoc cref="IInfoEntrance" />
    [Entrance("JLU", "学院开课情况查询", "看看可以去蹭哪些课吧~", EntranceType.InfoEntrance)]
    internal class ClassSchedule : BaseController, IInfoEntrance
    {
        public Bootstrap HtmlDocument { get; set; }

        private int termId = 136;
        private int schId = 101;
        private int tcmType = -1;
        private int lessonId;
        
        const string ScriptFileUrl = "service/res.do";
        string PostValue => $"{{\"tag\":\"lesson@globalStore\",\"branch\":\"default\",\"params\":{{\"termId\":{termId},\"schId\":\"{schId}\"{(tcmType == -1 ? "" : ",\"tcmType\":\""+ tcmType +"\"")}}},\"orderBy\":\"courseInfo.courName, extLessonNo\"}}";
        string PostDetail => $"{{\"tag\":\"teachClassMaster@selectResultAdjust\",\"branch\":\"byLesson\",\"params\":{{\"lessonId\":\"{lessonId}\"}}}}";

        public ClassSchedule()
        {
            var schIdSelect = new Select
            (
                "schId",
                new EnumerableAdapter
                (
                    from item in AlreadyKnownThings.Colleges
                    select new KeyValuePair<string, string>(item.Id, item.Name)
                )
            );
            
            var tcmTypeSelect = new Select("tcmType")
            {
                { "-1", "任意类型" },
                { "3080", "专业课" },
                { "3082", "重修" },
                { "3084", "体育课" },
                { "3085", "校选修课" },
            };

            var termIdSelect = new Select
            (
                "termId",
                AlreadyKnownThings.TermInfo
            );

            var lessonIdTable = new TableResponsive(bodyId: "lessonId")
            {
                { "上课时间地点", 15 },
                { "任课教师", 6 },
                { "选课人数", 6 },
            };

            var lessonListTable = new TableResponsive(bodyId: "lessonList")
            {
                { "课程名称", 17 },
                { "课程代码", 8 },
                { "类别", 7 },
                { "课程负责人", 9 },
            };

            HtmlDocument = new Bootstrap
            {
                Children =
                {
                    new MasterDetail(new Form
                    {
                        termIdSelect.WrapFormGroup(),
                        schIdSelect.WrapFormGroup(),
                        tcmTypeSelect.WrapFormGroup(),
                    }){
                        lessonIdTable,
                        lessonListTable
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
            if (IsBusy) return; IsBusy = true;
            Evaluate?.Invoke($"$('#lessonList').html('<tr><td colspan=\"4\">正在加载…</td></tr>');$('#lessonId').html('')");

            try
            {
                var LastReport = await Core.App.Service.Post(ScriptFileUrl, PostValue);
                var lists = LastReport.ParseJSON<RootObject<CollegeCourse>>();
                var sb = new StringBuilder();

                foreach (var opt in lists.value)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td><a class=\"linked-a\" onclick=\"showDetail({opt.lessonId})\">{opt.courseInfo.courName}</a></td>");
                    sb.Append($"<td>{opt.courseInfo.extCourseNo}</td>");
                    sb.Append("<td>" + (opt.extLessonNo.EndsWith("-cx")
                                  ? "重修"
                                  : AlreadyKnownThings.Type5Name(opt.courseInfo.type5)) + "</td>");
                    sb.Append($"<td>{opt.leader.name}</td>");
                    sb.Append("</tr>");
                }

                Evaluate?.Invoke($"$('#lessonList').html('{sb}')");
                sb.Clear();
                IsBusy = false;
            }
            catch (JsonException)
            {
                IsBusy = false;
                await RequestMessageAsync("提示", "加载课程列表失败。");
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.Timeout) throw;
                IsBusy = false;
                await this.ShowTimeoutMessage();
            }
        }

        private async void SolveLessonId()
        {
            if (IsBusy) return; IsBusy = true;
            Evaluate?.Invoke("$('#lessonId').html('<tr><td colspan=\"4\">正在加载…</td></tr>')");

            try
            {
                var LastReport = await Core.App.Service.Post(ScriptFileUrl, PostDetail);
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

                Evaluate?.Invoke($"$('#lessonId').html('{sb}')");
                sb.Clear();
                IsBusy = false;
            }
            catch (JsonException)
            {
                IsBusy = false;
                await RequestMessageAsync("提示", "加载教学班列表失败。");
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.Timeout) throw;
                IsBusy = false;
                await this.ShowTimeoutMessage();
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
                await RequestMessageAsync("错误", "未知响应：" + data);
            }
        }
    }
}
