using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandSchool.Internals;
using HandSchool.Internals.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Services;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HandSchool.JLU.JsonObject
{
    public class RoomScheduleBuilding
    {
        public int? compus { get; set; }
        public int? activeStatus { get; set; }
        public int? extOrderNo { get; set; }
        public string name { get; set; }
        public int? buildingId { get; set; }
    }

    public class RoomScheduleRoot
    {
        public string notes { get; set; }
        public string roomNo { get; set; }
        public string usage { get; set; }
        public string fullName { get; set; }
        public int? clsrmType { get; set; }
        public string seatColGroup { get; set; }
        public int? roomId { get; set; }
        public RoomScheduleBuilding building { get; set; }
        public int? examVolume { get; set; }
        public int? volume { get; set; }
        public string seatRows { get; set; }
        public string allowConflict { get; set; }
        public int? floor { get; set; }
    }
}

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("JLU", "教室课程表", "", EntranceType.InfoEntrance)]

    public class RoomSchedule : BaseController, IInfoEntrance
    {
        private readonly string[] _numList = {"一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一"};
        private IList<RoomScheduleRoot> _values;
        private int _roomId = -1;
        private List<LessonSchedule> _scheduleList;
        private const string ServerUrl = "service/res.do";
        private readonly int _termId;
        string QuerySchedule =>
            "{\"tag\":\"lessonSchedule@classroomUseSearch\",\"branch\":\"default\",\"params\":{\"termId\":" + _termId +",\"roomId\":" +
            _roomId + "},\"orderBy\":\"timeBlock.classSet, timeBlock.tmbId\"}";
        
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
        public RoomSchedule(int campus, int bid)
        {
            _campus = campus;
            _bid = bid;
            _termId = Tools.GetTermId() ?? -1;
            var room = new Select("roomId")
            {
                {"-1", "加载中……"}
            };

            var sb = new StringBuilder();
            sb.Append("<div class=\"table-responsive\"><table class=\"curriculumTable\"><thead><tr><th>&nbsp;</th>");
            foreach (var weekday in _numList.Take(6))
                sb.Append($"<th class=\"head\">星期{weekday}</th>");
            sb.Append("<th class=\"head\">星期日</th>");
            sb.Append("</tr></thead><tbody id=\"currTableBody\">");
            foreach (var classes in _numList)
                sb.Append(
                    $"<tr><th class=\"left\">{classes}</th><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>");
            sb.Append("</tbody></table></div>");
            var origTable = sb.ToRawHtml();

            HtmlDocument = new Bootstrap
            {
                Children =
                {
                    new MasterDetail(new Form
                    {
                        room.WrapFormGroup()
                    })
                    {
                        origTable
                    }
                },
                JavaScript =
                {
                    "$(function(){invokeCSharpAction('roomId')})",
                },
                Css = GetCss()
            };
            Menu.Add(new HandSchool.Views.MenuEntry
            {
                Title = "加载",
                Command = new CommandAction(() => Evaluate?.Invoke("invokeCSharpAction('show='+$('#roomId').val())"))
            });
        }

        private readonly int _campus;
        private readonly int _bid;
        public override async Task Receive(string data)
        {
            if (data.StartsWith("roomId"))
            {
                try
                {
                    var postValue =
                        "{\"tag\":\"clsrm_building\",\"branch\":\"default\",\"params\":{\"campus\":\"" + _campus + "\",\"bid\":\"" + _bid + "\",\"usage\":\"T\",\"searchUc\":[]},\"orderBy\":\"roomNo\"}";
                    var res = await Core.App.Service.Post(ServerUrl, postValue);
                    var resn = JsonConvert.DeserializeObject<JObject>(res);
                    _values = resn["value"].ToObject<List<RoomScheduleRoot>>();
                    var sb = new StringBuilder();
                    var selected = true;
                    foreach (var room in _values)
                    {
                        if (room.roomId is null) continue;
                        sb.Append(
                            $"<option value=\"{room.roomId}\"{(selected ? "selected" : "")}>{room.roomNo}</option>");
                        selected = false;
                    }

                    Evaluate?.Invoke($"$('#roomId').html('{sb}')");
                    sb.Clear();
                }
                catch (Exception e)
                {
                    await NoticeError("加载教学楼教室信息失败！");
                    Core.Logger.WriteLine("error", e.Message);
                }
            }
            else if (data.StartsWith("show"))
            {
                _roomId = int.Parse(data.Split('=')[1]);
                await ProduceClassDetail();
            }
        }

        private async Task SolveClassDetail()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var res = await Core.App.Service.Post(ServerUrl, QuerySchedule);
                var resD = res.ParseJSON<JObject>();
                if (resD is null)
                {
                    await NoticeError("解析失败！");
                    return;
                }
                _scheduleList = resD["value"]?.ToObject<List<LessonSchedule>>();
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
            var vm = new TemplateScheduleViewModel($"{_roomId}");
            vm.Items = Schedule.ParseEnumer(_scheduleList);
            vm.RenderWeek(0, out var currList);
            var strTable = new string[7, 11];
            for (var i = 0; i < 7; i++)
            for (var j = 0; j < 11; j++)
                strTable[i, j] = "<td></td>";

            var sb = new StringBuilder();
            foreach (var iSet in currList)
            {
                var set = iSet as CurriculumSet;
                if (set is null) return;
                var i = iSet.WeekDay - 1;
                var j = iSet.DayBegin - 1;
                for (var k = iSet.DayBegin; k < iSet.DayEnd; k++)
                    strTable[i, k] = "";
                sb.Append($"<td rowspan=\"{iSet.DayEnd - j}\">");

                var iiii = false;
                foreach (var section in set.InnerList)
                {
                    if (iiii) sb.Append("<br><br>");
                    sb.Append(
                        $"{section.Name}<br>{section.Teacher}<br>第{section.WeekBegin}~{section.WeekEnd}周 {CurriculumItem.WeekEvenOddToString[(int) section.WeekOen]}");
                    iiii = true;
                }

                sb.Append("</td>");
                strTable[i, j] = sb.ToString();
                sb.Clear();
            }

            for (int i = 0; i < 11; i++)
            {
                sb.Append($"<tr><th class=\"left\">{_numList[i]}</th>");
                for (int j = 0; j < 7; j++)
                    sb.Append(strTable[j, i]);
                sb.Append("</tr>");
            }

            sb.Replace("'", "\'");
            Evaluate?.Invoke("$('#currTableBody').html('" + sb + "')");
        }

        public Bootstrap HtmlDocument { get; set; }
    }
}