using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        public static ObservableCollection<ScheduleViewItem> List = new ObservableCollection<ScheduleViewItem>();

        public static event EventHandler OnTableRefresh;

        public static void RenderWeek(int week, bool showAll = false)
        {
            if (showAll)
                throw new NotImplementedException();

            List.Clear();
            foreach (ICurriculumItem item in Table)
            {
                if (showAll || item.IfShow(week))
                    List.Add(new ScheduleViewItem(item));
            }
        }
    }

    public class ScheduleViewItem
    {
        public ICurriculumItem Source { get; }

        public ScheduleViewItem(ICurriculumItem source)
        {
            Source = source;
        }
    }
}
