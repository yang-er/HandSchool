using System;

namespace HandSchool
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
}
