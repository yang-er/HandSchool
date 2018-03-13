using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using System;
using System.Threading.Tasks;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    class Schedule : ISystemEntrance
    {
        public string Name => "课程表";
        public string ScriptFileUri => "service/res.do";
        public bool IsPost => true;
        public string LastReport { get; private set; }
        public string StorageFile => "jlu.kcb.json";
        public string PostValue => "{\"tag\":\"teachClassStud@schedule\",\"branch\":\"default\",\"params\":{\"termId\":" + App.Current.Service.AttachInfomation["term"] + ",\"studId\":" + App.Current.Service.AttachInfomation["studId"] + "}}";
        
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
                    CurriculumSchedule.Table.Add(new TeachClassDetail(time, obj));
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
    
    public class TeachClassDetail : ICurriculumItem
    {
        public TeachClassDetail(LessonSchedule time, ScheduleValue obj)
        {
            WeekBegin = int.Parse(time.timeBlock.beginWeek);
            WeekEnd = int.Parse(time.timeBlock.endWeek);
            WeekOen = (WeekOddEvenNone)(time.timeBlock.weekOddEven == "" ? 2 : (time.timeBlock.weekOddEven == "O" ? 1 : 0));
            WeekDay = int.Parse(time.timeBlock.dayOfWeek);
            Classroom = time.classroom.fullName;
            CourseID = obj.teachClassMaster.name;
            SelectDate = obj.dateAccept;
            Name = obj.teachClassMaster.lessonSegment.fullName;
            DayEnd = 0;
            DayBegin = 0;
            int tmp = int.Parse(time.timeBlock.classSet);
            int tmp2 = tmp & (-tmp);
            while (tmp != 0)
            {
                tmp >>= 1;
                tmp2 >>= 1;
                if (tmp2 > 1)
                    DayBegin++;
                else if (tmp2 == 1)
                    DayEnd = ++DayBegin;
                else if (tmp >= 1)
                    DayEnd++;
            }
            foreach (var t in obj.teachClassMaster.lessonTeachers)
                Teacher += t.teacher.name + " ";
            Teacher = Teacher.Trim();
        }
        
        public string Name { get; set; }
        public string Teacher { get; set; }
        public string CourseID { get; set; }
        public string Classroom { get; set; }
        public int WeekBegin { get; set; }
        public int WeekEnd { get; set; }
        public WeekOddEvenNone WeekOen { get; set; }
        public int WeekDay { get; set; }
        public int DayBegin { get; private set; }
        public int DayEnd { get; private set; }
        public DateTime SelectDate { get; }

        public bool IfShow(int week)
        {
            bool show = ((int)WeekOen == 2) || ((int)WeekOen == week % 2);
            show &= (week >= WeekBegin) && (week <= WeekEnd);
            return show;
        }

        public bool SetTime(int begin, int end)
        {
            if (begin > end)
                return false;
            if (begin <= 0 || end >= 12)
                return false;
            DayBegin = begin;
            DayEnd = end;
            return true;
        }
    }
}
