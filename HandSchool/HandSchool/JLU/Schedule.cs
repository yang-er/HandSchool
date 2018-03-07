using HandSchool.Internal;
using System;
using System.Collections.Generic;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    class Schedule : ISystemEntrance, ICurriculumParser<TeachClassDetail>
    {
        private UIMS uims;
        public ISchoolSystem Parent => uims;
        private string lastReport;

        public string Name => "课程表";
        public string ScriptFileUri => "service/res.do";
        public bool IsPost => true;
        public string PostValue => "{\"tag\":\"teachClassStud@schedule\",\"branch\":\"default\",\"params\":{\"termId\":" + uims.AttachInfomation["termId"] + ",\"studId\":" + uims.AttachInfomation["stuId"] + "}}";
        
        public void Execute()
        {
            lastReport = Parent.Post(ScriptFileUri, PostValue);
        }

        public void ParseTable(List<TeachClassDetail> list, string inputData = "", bool append = false)
        {
            if (inputData == "")
                inputData = lastReport;
            var table = JSON<RootObject>(inputData);
            foreach (var obj in table.value)
            {
                foreach (var time in obj.teachClassMaster.lessonSchedules)
                {
                    var p = new TeachClassDetail()
                    {
                        WeekBegin = int.Parse(time.timeBlock.beginWeek),
                        WeekEnd = int.Parse(time.timeBlock.endWeek),
                        WeekOen = (TeachClassDetail.WeekOddEvenNone)(time.timeBlock.weekOddEven == "" ? 2 : (time.timeBlock.weekOddEven == "O" ? 1 : 0)),
                        WeekDay = int.Parse(time.timeBlock.dayOfWeek),
                        Classroom = time.classroom.fullName,
                        CourseID = obj.teachClassMaster.name,
                        SelectDate = obj.dateAccept,
                        TCMID = int.Parse(obj.teachClassMaster.tcmId),
                        TCSID = int.Parse(obj.tcsId),
                        Name = obj.teachClassMaster.lessonSegment.fullName
                    };
                    int tmp = int.Parse(time.timeBlock.classSet);
                    int tmp2 = tmp & (-tmp);
                    while (tmp != 0)
                    {
                        tmp >>= 1;
                        tmp2 >>= 1;
                        if (tmp2 > 1)
                            p.DayBegin++;
                        else if (tmp2 == 1)
                            p.DayEnd = ++p.DayBegin;
                        else if (tmp >= 1)
                            p.DayEnd++;
                    }
                    foreach (var t in obj.teachClassMaster.lessonTeachers)
                        p.Teacher += t.teacher.name + " ";
                    p.Teacher = p.Teacher.Trim();
                    list.Add(p);
                }
            }
            throw new NotImplementedException();
        }

        public CurriculumSmall SmallInfo(TeachClassDetail tcd)
        {
            return new CurriculumSmall
            {
                Name = tcd.Name,
                Position = tcd.Classroom,
                Weekday = tcd.WeekDay,
                From = tcd.DayBegin,
                To = tcd.DayEnd
            };
        }

        public CurriculumDetail DetailInfo(TeachClassDetail curriculum)
        {
            throw new NotImplementedException();
        }

        public bool IfShow(int week, TeachClassDetail tcd)
        {
            bool show = ((int)tcd.WeekOen == 2) || ((int)tcd.WeekOen == week % 2);
            show &= (week >= tcd.WeekBegin) && (week <= tcd.WeekEnd);
            return show;
        }

        public class RootValue
        {
            public TeachClassMaster teachClassMaster { get; set; }
            public string tcsId { get; set; }
            public object student { get; set; }
            public DateTime dateAccept { get; set; }
        }

        public class RootObject
        {
            public string id { get; set; }
            public int status { get; set; }
            public RootValue[] value { get; set; }
            public string resName { get; set; }
            public string msg { get; set; }
        }

    }

    public class TeachClassDetail
    {
        public string Name = "";
        public string Teacher = "";
        public string CourseID = "";
        public string Classroom = "";
        public int TCMID = 0;
        public int TCSID = 0;
        public int WeekBegin = 0;
        public int WeekEnd = 0;
        public WeekOddEvenNone WeekOen = WeekOddEvenNone.None;
        public int WeekDay = 0;
        public int DayBegin = 0;
        public int DayEnd = 0;
        public DateTime SelectDate = DateTime.Today;

        public enum WeekOddEvenNone
        {
            Odd, Even, None
        }
    }
}
