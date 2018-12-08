using System;
using System.Threading.Tasks;
using HandSchool.Internal.HtmlObject;
using HandSchool.Services;
using HandSchool.ViewModels;
using HandSchool.JLU.JsonObject;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using HandSchool.Internal;
using HandSchool.Models;
using System.Collections.Generic;
using System.Linq;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("学生班级推荐课表", "可以来看看下学期的课表啦~", EntranceType.InfoEntrance)]
    public class AdviceSchedule : BaseController, IInfoEntrance
    {
        private int teachTermId = -1;
        private int admClassId = -1;

        private RootObject<TeachingTerm> termList;
        private RootObject<ScheduleValue> scheduleList;

        const string ScriptFileUri = "service/res.do";
        public string QueryTerms => "{\"tag\":\"search@teachingTerm\",\"branch\":\"default\",\"params\":{}}";
        public string QuerySchedule => $"{{\"tag\":\"tcmAdcAdvice@dep_recommandT\",\"branch\":\"byAdc\",\"params\":{{\"termId\":{teachTermId},\"adcId\":{admClassId}}}}}";

        public AdviceSchedule()
        {
            admClassId = int.Parse(Core.App.Service.AttachInfomation["adcId"]);
            var listId = Core.App.Service.AttachInfomation["schoolId"];
            var college = AlreadyKnownThings.Colleges.Find((o) => o.Id == listId);
            var collegeName = college.Name ?? "未知";

            var sb = new StringBuilder();
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
                    "<h4>学期</h4>".ToRawHtml(),
                    term,
                    "<h4>行政班所在学院</h4>".ToRawHtml(),
                    $"<p>{collegeName}</p>".ToRawHtml(),
                    "<button onclick=\"invokeCSharpAction('show='+$('#termId').val())\">".ToRawHtml()
                }
            };
        }

        public Bootstrap HtmlDocument { get; set; }

        public async Task SolveTermId()
        {
            if (IsBusy) return;
            SetIsBusy(true, "正在加载学期信息……");

            try
            {
                var LastReport = await Core.App.Service.Post(ScriptFileUri, QueryTerms);
                termList = LastReport.ParseJSON<RootObject<TeachingTerm>>();
                var sb = new StringBuilder();
                sb.Append("<option value=\"-1\" selected>请选择</option>");

                foreach (var opt in termList.value)
                {
                    sb.Append($"<option value=\"{opt.termId}\">{opt.termName}</option>");
                }

                Evaluate?.Invoke($"$('#termId').html('{sb.ToString()}')");
                sb.Clear();
                SetIsBusy(false);
            }
            catch (JsonException)
            {
                SetIsBusy(false);
                await ShowMessage("提示", "加载学期信息失败。");
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    SetIsBusy(false);
                    await ShowMessage("错误", "连接超时，请重试。");
                    return;
                }

                await ShowMessage("错误", ex.ToString());
                throw ex;
            }
        }

        public async Task SolveClassDetail()
        {
            if (IsBusy) return;
            SetIsBusy(true, "正在加载推荐课表……");

            try
            {
                var LastReport = await Core.App.Service.Post(ScriptFileUri, QuerySchedule);
                scheduleList = LastReport.ParseJSON<RootObject<ScheduleValue>>();
                SetIsBusy(false);
            }
            catch (JsonException)
            {
                SetIsBusy(false);
                await ShowMessage("提示", "加载推荐课表失败。");
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    SetIsBusy(false);
                    await ShowMessage("错误", "连接超时，请重试。");
                    return;
                }

                await ShowMessage("错误", ex.ToString());
                throw ex;
            }
        }

        public override async Task Receive(string data)
        {
            if (data == "term")
            {
                await SolveTermId();
            }
            else if (data.StartsWith("show="))
            {
                teachTermId = int.Parse(data.Split("=")[1]);
                await SolveClassDetail();

                // TODO: HTML implement class schedule
                throw new NotImplementedException();
            }
            else
            {
                await ShowMessage("错误", "请报告开发者，参数未知：" + data);
            }
        }

    }
}
