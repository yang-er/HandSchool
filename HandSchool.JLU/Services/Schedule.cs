using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Services;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: RegisterService(typeof(Schedule))]
namespace HandSchool.JLU.Services
{
    
    /// <summary>
    /// UIMS课程表解析服务。
    /// </summary>
    /// <inheritdoc cref="IScheduleEntrance" />
    [Entrance("JLU", "课程表", "提供解析UIMS课程表的功能。")]
    [UseStorage("JLU", configKcb, configKcbOrig)]
    public class Schedule : IScheduleEntrance
    {
        const string configKcbOrig = "jlu.kcb.json";
        const string configKcb = "jlu.kcb2.json";

        const string serviceResourceUrl = "service/res.do";
        const string schedulePostValue = "{\"tag\":\"teachClassStud@schedule\",\"branch\":\"default\",\"params\":{\"termId\":`term`,\"studId\":`studId`}}";

        static readonly (string begin, string over)[] ClassBetween = {
            ("8:00", "8:45"),
            ("8:55", "9:40"),
            ("10:00", "10:45"),
            ("10:55", "11:40"),
            ("13:30", "14:15"),
            ("14:25", "15:10"),
            ("15:30", "16:15"),
            ("16:25", "17:10"),
            ("18:30", "19:15"),
            ("19:25", "20:10"),
            ("20:20", "21:05")
        };
        
        public async Task Execute()
        {
            try
            {
                var scheduleValue = await Core.App.Service.Post(serviceResourceUrl, schedulePostValue);
                Core.Configure.Write(configKcbOrig, scheduleValue);
                var table = scheduleValue.ParseJSON<RootObject<ScheduleValue>>();
                ScheduleViewModel.Instance.RemoveAllItem(obj => !obj.IsCustom);
                var iterator = ParseEnumer(table.value);
                foreach (var item in iterator)
                    ScheduleViewModel.Instance.AddItem(item);
                ScheduleViewModel.Instance.SaveToFile();
            }
            catch (WebsException ex)
            {
                if (ex.Status != WebStatus.Timeout) throw;
                await ScheduleViewModel.Instance.ShowTimeoutMessage();
            }
        }

        public static IEnumerable<CurriculumItem> ParseEnumer(IEnumerable<ScheduleValue> tableValue)
        {
            foreach (var obj in tableValue)
            {
                foreach (var time in obj.teachClassMaster.lessonSchedules)
                {
                    if (string.IsNullOrEmpty(time.timeBlock.dayOfWeek)) continue;

                    var item = new CurriculumItem
                    {
                        WeekBegin = int.Parse(time.timeBlock.beginWeek ?? "1"),
                        WeekEnd = int.Parse(time.timeBlock.endWeek ?? "19"),
                        WeekOen = (WeekOddEvenNone)(time.timeBlock.weekOddEven == null ? 2 : (time.timeBlock.weekOddEven == "O" ? 1 : 0)),
                        WeekDay = int.Parse(time.timeBlock.dayOfWeek),
                        Classroom = time.classroom.fullName,
                        CourseId = obj.teachClassMaster.name,
                        SelectDate = obj.dateAccept,
                        Name = obj.teachClassMaster.lessonSegment.fullName,
                    };

                    foreach (var t in obj.teachClassMaster.lessonTeachers)
                        item.Teacher += t.teacher.name + " ";
                    item.Teacher = item.Teacher.Trim();

                    int tmp = int.Parse(time.timeBlock.classSet);
                    int tmp2 = tmp & (-tmp);
                    while (tmp != 0)
                    {
                        tmp >>= 1;
                        tmp2 >>= 1;
                        if (tmp2 > 1)
                            item.DayBegin++;
                        else if (tmp2 == 1)
                            item.DayEnd = ++item.DayBegin;
                        else if (tmp >= 1)
                            item.DayEnd++;
                    }

                    yield return item;
                }
            }
        }

        public static IEnumerable<CurriculumItem> ParseEnumer(IEnumerable<LessonSchedule> tableValue)
        {
            foreach (var obj in tableValue)
            {
                var item = new CurriculumItem
                {
                    WeekBegin = int.Parse(obj.timeBlock.beginWeek ?? "1"),
                    WeekEnd = int.Parse(obj.timeBlock.endWeek ?? "19"),
                    WeekOen = (WeekOddEvenNone) (obj.timeBlock.weekOddEven == null
                        ? 2
                        : (obj.timeBlock.weekOddEven == "O" ? 1 : 0)),
                    WeekDay = int.Parse(obj.timeBlock.dayOfWeek),
                    Classroom = obj.classroom.fullName,
                    Name = obj.teachClassMaster.lessonSegment.fullName
                };
                foreach (var t in obj.teachClassMaster.lessonTeachers)
                    item.Teacher += t.teacher.name + " ";
                item.Teacher = item.Teacher.Trim();
                var tmp = int.Parse(obj.timeBlock.classSet);
                var tmp2 = tmp & (-tmp);
                while (tmp != 0)
                {
                    tmp >>= 1;
                    tmp2 >>= 1;
                    if (tmp2 > 1)
                        item.DayBegin++;
                    else if (tmp2 == 1)
                        item.DayEnd = ++item.DayBegin;
                    else if (tmp >= 1)
                        item.DayEnd++;
                }
                yield return item;
            }
        }
        public (int section, SectionState? state) GetCurrentClass()
        {
            var i = 10;
            var now = DateTime.Now;
            while (i >= 0)
            {
                if (now > Convert.ToDateTime(ClassBetween[i].begin))
                    break;
                i--;
            }
            if (i < 0) return (0, null);
            return (i + 1, now > Convert.ToDateTime(ClassBetween[i].over) ? SectionState.ClassOver : SectionState.ClassOn);
        }

        public (int section, SectionState? state) CurrentClass => GetCurrentClass();
    }
}
