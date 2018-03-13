using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    class Schedule : IScheduleEntrance
    {
        public string Name => "课程表";
        public string ScriptFileUri => "service/res.do";
        public bool IsPost => true;
        public string LastReport { get; private set; }
        public List<CurriculumItem> Items { get; }
        public string StorageFile => "jlu.kcb.json";
        public string PostValue => "{\"tag\":\"teachClassStud@schedule\",\"branch\":\"default\",\"params\":{\"termId\":" + App.Current.Service.AttachInfomation["term"] + ",\"studId\":" + App.Current.Service.AttachInfomation["studId"] + "}}";

        public void RenderWeek(int week, List<CurriculumLabel> list, bool showAll = false)
        {
            if (showAll)
                throw new NotImplementedException();
            
            foreach (CurriculumItem item in Items)
            {
                if (showAll || item.IfShow(week))
                    list.Add(new CurriculumLabel(item));
            }
        }

        public async Task Execute()
        {
            LastReport = await App.Current.Service.PostJson(ScriptFileUri, PostValue);
            WriteConfFile(StorageFile, LastReport);
            Parse();
        }
        
        public void Parse()
        {
            var table = JSON<RootObject>(LastReport);
            foreach (var obj in table.value)
            {
                foreach (var time in obj.teachClassMaster.lessonSchedules)
                {
                    var item = new CurriculumItem
                    {
                        WeekBegin = int.Parse(time.timeBlock.beginWeek),
                        WeekEnd = int.Parse(time.timeBlock.endWeek),
                        WeekOen = (WeekOddEvenNone)(time.timeBlock.weekOddEven == "" ? 2 : (time.timeBlock.weekOddEven == "O" ? 1 : 0)),
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
                    Items.Add(item);
                }
            }
        }

        public Schedule()
        {
            LastReport = ReadConfFile(StorageFile);
            if (LastReport != "") Parse();
        }

        public class RootObject
        {
            public string id { get; set; }
            public int status { get; set; }
            public ScheduleValue[] value { get; set; }
            public string resName { get; set; }
            public string msg { get; set; }
        }
    }
}
