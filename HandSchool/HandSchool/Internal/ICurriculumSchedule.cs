using System;
using System.Collections.Generic;

namespace HandSchool.Internal
{
    public enum WeekOddEvenNone { Odd, Even, None }

    public interface ICurriculumItem
    {
        string Name { get; set; }
        string Teacher { get; set; }
        string CourseID { get; set; }
        string Classroom { get; set; }
        int WeekBegin { get; set; }
        int WeekEnd { get; set; }
        WeekOddEvenNone WeekOen { get; set; }
        int WeekDay { get; set; }
        int DayBegin { get; }
        int DayEnd { get; }
        DateTime SelectDate { get; }
        bool IfShow(int week);
        bool SetTime(int begin, int end);
    }
    
    public class CurriculumSchedule
    {
        public static List<ICurriculumItem> Table = new List<ICurriculumItem>();

        public static event EventHandler OnTableRefresh;

        public static List<ICurriculumItem> RenderWeek(int week, bool showAll = false)
        {
            if (showAll)
                throw new NotImplementedException();

            List<ICurriculumItem> ret = new List<ICurriculumItem>();
            foreach (ICurriculumItem item in Table)
            {
                if (showAll || item.IfShow(week))
                    ret.Add(item);
            }
            return ret;
        }
    }
}
