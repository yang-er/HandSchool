using HandSchool.Internal;
using HandSchool.Models;
using System;
using System.Collections.Generic;

namespace HandSchool
{
    namespace Models
    {
        public enum WeekOddEvenNone { Even, Odd,None }

        public class CurriculumItem : NotifyPropertyChanged
        {
            private string _name;
            private string _teacher;
            private string _courseID;
            private string _classroom;
            private int _weekBegin;
            private int _weekEnd;
            private WeekOddEvenNone _weekOen;
            private int _weekDay;
            private int _dayBegin;
            private int _dayEnd;
            private DateTime _selectDate;
            private bool _isCustom;

            public string Name { get => _name; set => SetProperty(ref _name, value); }
            public string Teacher { get => _teacher; set => SetProperty(ref _teacher, value); }
            public string CourseID { get => _courseID; set => SetProperty(ref _courseID, value); }
            public string Classroom { get => _classroom; set => SetProperty(ref _classroom, value); }
            public int WeekBegin { get => _weekBegin; set => SetProperty(ref _weekBegin, value); }
            public int WeekEnd { get => _weekEnd; set => SetProperty(ref _weekEnd, value); }
            public WeekOddEvenNone WeekOen { get => _weekOen; set => SetProperty(ref _weekOen, value); }
            public int WeekDay { get => _weekDay; set => SetProperty(ref _weekDay, value); }
            public int DayBegin { get => _dayBegin; set => SetProperty(ref _dayBegin, value); }
            public int DayEnd { get => _dayEnd; set => SetProperty(ref _dayEnd, value); }
            public DateTime SelectDate { get => _selectDate; set => SetProperty(ref _selectDate, value); }
            public bool IsCustom { get => _isCustom; set => SetProperty(ref _isCustom, value); }

            public CurriculumItem()
            {
                _name = _teacher = _courseID = _classroom = string.Empty;
                _weekBegin = _weekEnd = _weekDay = _dayBegin = _dayEnd = 0;
                _weekOen = WeekOddEvenNone.None;
                _selectDate = DateTime.Now;
                _isCustom = false;
            }

            public bool IfShow(int week)
            {
                bool show = ((int)_weekOen == 2) || ((int)_weekOen == week % 2);
                show &= (week >= _weekBegin) && (week <= _weekEnd);
                return show;
            }
        }
    }

    namespace Services
    {
        public interface IScheduleEntrance : ISystemEntrance
        {
            int ClassNext { get; }
            List<CurriculumItem> Items { get; }
            void RenderWeek(int week, out List<CurriculumItem> list, bool showAll = false);
            void Save();
        }
    }
}
