using HandSchool.Models;
using System;
using System.Collections.Generic;

namespace HandSchool
{
    public enum WeekOddEvenNone { Odd, Even, None }
    
    public class CurriculumItem
    {
        public string Name { get; set; }
        public string Teacher { get; set; }
        public string CourseID { get; set; }
        public string Classroom { get; set; }
        public int WeekBegin { get; set; }
        public int WeekEnd { get; set; }
        public WeekOddEvenNone WeekOen { get; set; }
        public int WeekDay { get; set; }
        public int DayBegin { get; set; }
        public int DayEnd { get; set; }
        public DateTime SelectDate { get; set; }
        public bool IsCustom { get; set; }

        public CurriculumItem()
        {
            Name = Teacher = CourseID = Classroom = string.Empty;
            WeekBegin = WeekEnd = WeekDay = DayBegin = DayEnd = 0;
            WeekOen = WeekOddEvenNone.None;
            SelectDate = DateTime.Now;
            IsCustom = false;
        }
        
        public bool IfShow(int week)
        {
            bool show = ((int)WeekOen == 2) || ((int)WeekOen == week % 2);
            show &= (week >= WeekBegin) && (week <= WeekEnd);
            return show;
        }
    }

    public interface IScheduleEntrance : ISystemEntrance
    {
        List<CurriculumItem> Items { get; }
        void RenderWeek(int week, List<CurriculumLabel> list, bool showAll = false);
    }
}
