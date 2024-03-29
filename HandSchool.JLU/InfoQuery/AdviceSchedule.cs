﻿using HandSchool.Internals;
using HandSchool.Internals.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Services;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("JLU", "学生班级推荐课表", "可以来看看下学期的课表啦~", EntranceType.InfoEntrance)]
    internal class AdviceSchedule : BaseController, IInfoEntrance
    {
        private int teachTermId = -1;
        private RootObject<TeachingTerm> termList;
        private RootObject<ScheduleValue> scheduleList;
        private readonly string[] numList = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一" };

        const string ScriptFileUri = "service/res.do";
        public string QueryTerms => "{\"tag\":\"search@teachingTerm\",\"branch\":\"default\",\"params\":{}}";
        public string QuerySchedule => $"{{\"tag\":\"tcmAdcAdvice@dep_recommandT\",\"branch\":\"byAdc\",\"params\":{{\"termId\":{teachTermId},\"adcId\":`adcId`}}}}";

        public AdviceSchedule()
        {
            var term = new Select("termId")
            {
                { "-1", "加载中……" }
            };

            var sb = new StringBuilder();
            sb.Append("<div class=\"table-responsive\"><table class=\"curriculumTable\"><thead><tr><th>&nbsp;</th>");
            foreach (var weekday in numList.Take(6))
                sb.Append($"<th class=\"head\">星期{weekday}</th>");
            sb.Append("<th class=\"head\">星期日</th>");
            sb.Append("</tr></thead><tbody id=\"currTableBody\">");
            foreach (var classes in numList)
                sb.Append($"<tr><th class=\"left\">{classes}</th><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>");
            sb.Append("</tbody></table></div>");
            var orig_table = sb.ToRawHtml();
            
            HtmlDocument = new Bootstrap
            {
                Children =
                {
                    new MasterDetail(new Form
                    {
                        term.WrapFormGroup()
                    }) {
                        orig_table
                    }
                },
                JavaScript =
                {
                    "$(function(){invokeCSharpAction('term')})",
                },
                Css = GetCss()
            };

            Menu.Add(new HandSchool.Views.MenuEntry
            {
                Title = "加载",
                UWPIcon = "\uE72C",
                Command = new CommandAction(() => Evaluate?.Invoke("invokeCSharpAction('show='+$('#termId').val())"))
            });
        }

        public Bootstrap HtmlDocument { get; set; }

        private string GetCss()
        {
            return ".curriculumTable th{border:1px solid #000;text-align:center;}" +
                    ".curriculumTable td{border:1px solid #000;text-align:center;}" +
                    ".curriculumTable td[tcm]{background-color:green;}" +
                    ".curriculumTable .selected{background-color:blue;}" +
                    ".curriculumTable .selected[tcm]{background-color:blue;}" +
                    ".curriculumTable .temp{background-color:yellow;}" +
                    ".curriculumTable .temp[tcm]{background-color:red;}" +
                    ".curriculumTable .headSmall{height:20px;width:25px;background-color:#ccc;}" +
                    ".curriculumTable .leftSmall{height:25px;width:20px;background-color:#ddd;}" +
                    ".curriculumTable .head{height:30px;width:150px;background-color:#ccc;}" +
                    ".curriculumTable .left{width:50px;background-color:#ddd;height:50px;}" +
                    ".curriculumTable .days{width:110px;}" +
                    ".curriculumTable .tmb{width:110px;}" +
                    ".curriculumTable .phase{width:40px;}" +
                    ".curriculumTable .section{width:50px;}" +
                    ".curriculumTable .classNos{color:blue;}" +
                    ".curriculumTable .studCnt{color:darkviolet;}" +
                    ".curriculumItem{position:absolute;border:1px solid black;width:110px;background-color:#f8f3d5;word-break:break-all;overflow:hidden;}" +
                    ".curriculumItemZoom{border:1px solid black;width:220px;background-color:gray;word-break:break-all;overflow:auto;font-size:large;}" +
                    ".currLegend{border-collapse:collapse;}" +
                    ".currLegend td[tcm]{background-color:green;}" +
                    ".currLegend .selected{background-color:blue;}" +
                    ".currLegend .temp{background-color:yellow;}" +
                    ".currLegend .temp[tcm]{background-color:red;}" +
                    ".curriculumTable .usage{background-color:green;}" +
                    ".curriculumTable .takeup{background-color:yellow;}" +
                    ".curriculumTable .takeupExam{background-color:blue;}" +
                    ".curriculumTable .conflict{background-color:red;}" +
                    ".currLegend .usage{background-color:green;}" +
                    ".currLegend .takeup{background-color:yellow;}" +
                    ".currLegend .takeupExam{background-color:blue;}" +
                    ".currLegend .conflict{background-color:red;}" +
                    ".curriculumTable{width:50em;}";
        }

        private async Task SolveTermId()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var LastReport = await Core.App.Service.Post(ScriptFileUri, QueryTerms);
                LastReport = LastReport.Replace("Date\":null", "Date2\":null");
                termList = LastReport.ParseJSON<RootObject<TeachingTerm>>();
                var sb = new StringBuilder();
                bool selected = true;

                foreach (var opt in termList.value)
                {
                    sb.Append($"<option value=\"{opt.termId}\"{(selected ? "selected" : "")}>{opt.termName}</option>");
                    selected = false;
                }

                Evaluate?.Invoke($"$('#termId').html('{sb}')");
                sb.Clear();
            }
            catch (JsonException)
            {
                await RequestMessageAsync("提示", "加载学期信息失败，解析数据出现错误。");
            }
            catch (WebsException ex)
            {
                await RequestMessageAsync("错误", ex.Status.ToDescription() + "。");
            }

            IsBusy = false;
        }

        private async Task SolveClassDetail()
        {
            if (IsBusy) return; IsBusy = true;

            try
            {
                var LastReport = await Core.App.Service.Post(ScriptFileUri, QuerySchedule);
                scheduleList = LastReport.ParseJSON<RootObject<ScheduleValue>>();
            }
            catch (JsonException)
            {
                await RequestMessageAsync("提示", "加载推荐课表失败。");
            }
            catch (WebsException ex)
            {
                await RequestMessageAsync("错误", ex.Status.ToDescription() + "。");
            }

            IsBusy = false;
        }

        private async Task ProduceClassDetail()
        {
            await SolveClassDetail();
            var vm = new TemplateScheduleViewModel("学生班级推荐课表");
            vm.Items = Schedule.ParseEnumer(scheduleList.value);
            vm.RenderWeek(0, SchoolState.Normal, out var currList);

            var strTable = new string[7, 11];
            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 11; j++)
                    strTable[i, j] = "<td></td>";

            var sb = new StringBuilder();
            foreach (var iset in currList)
            {
                int i = iset.WeekDay - 1;
                int j = iset.DayBegin - 1;
                for (int k = iset.DayBegin; k < iset.DayEnd; k++)
                    strTable[i, k] = "";
                sb.Append($"<td rowspan=\"{iset.DayEnd - j}\">");
                bool iiii = false;

                foreach (var ii in iset.ToDescription())
                {
                    if (iiii) sb.Append("<br><br>");
                    sb.Append(ii.Title + "<br>" + ii.Description.Replace("\n", "；"));
                    iiii = true;
                }

                sb.Append("</td>");
                strTable[i, j] = sb.ToString();
                sb.Clear();
            }

            for (int i = 0; i < 11; i++)
            {
                sb.Append($"<tr><th class=\"left\">{numList[i]}</th>");
                for (int j = 0; j < 7; j++)
                    sb.Append(strTable[j, i]);
                sb.Append("</tr>");
            }

            sb.Replace("'", "\'");
            Evaluate?.Invoke("$('#currTableBody').html('" + sb + "')");
        }

        public override async Task Receive(string data)
        {
            if (data == "term")
            {
                await SolveTermId();
            }
            else if (data.StartsWith("show="))
            {
                teachTermId = int.Parse(data.Split('=')[1]);
                await ProduceClassDetail();
            }
            else
            {
                await RequestMessageAsync("错误", "请报告开发者，参数未知：" + data);
            }
        }
    }
}
