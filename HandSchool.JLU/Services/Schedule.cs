using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Services;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
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
    internal class Schedule : IScheduleEntrance
    {
        const string configKcbOrig = "jlu.kcb.json";
        const string configKcb = "jlu.kcb2.json";

        const string serviceResourceUrl = "service/res.do";
        const string schedulePostValue = "{\"tag\":\"teachClassStud@schedule\",\"branch\":\"default\",\"params\":{\"termId\":`term`,\"studId\":`studId`}}";
        
        static readonly string[] ClassBetween = { "8:00", "8:55", "10:00", "10:55", "13:30", "14:25", "15:30", "16:25", "18:30", "19:25", "20:20" };
        
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
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.Timeout) throw;
                await ScheduleViewModel.Instance.ShowTimeoutMessage();
            }
        }

        public static IEnumerable<CurriculumItem> ParseEnumer(IEnumerable<ScheduleValue> tableValue)
        {
            foreach (var obj in tableValue)
            {
                foreach (var time in obj.teachClassMaster.lessonSchedules)
                {
                    var item = new CurriculumItem
                    {
                        WeekBegin = int.Parse(time.timeBlock.beginWeek),
                        WeekEnd = int.Parse(time.timeBlock.endWeek),
                        WeekOen = (WeekOddEvenNone)(time.timeBlock.weekOddEven == null ? 2 : (time.timeBlock.weekOddEven == "O" ? 1 : 0)),
                        WeekDay = int.Parse(time.timeBlock.dayOfWeek),
                        Classroom = time.classroom.fullName,
                        CourseID = obj.teachClassMaster.name,
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

        public int GetClassNext()
        {
            for (int i = 0; i < 11; i++)
                if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(ClassBetween[i])) < 0)
                    return i;
            return 11;
        }

        public int ClassNext => GetClassNext();
    }
}
