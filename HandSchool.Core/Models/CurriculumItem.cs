using System;
using System.Collections.Generic;

namespace HandSchool.Models
{
    /// <summary>
    /// 表示单节课程表项目的类。
    /// </summary>
    public class CurriculumItem : CurriculumItemBase
    {
        private string _name, _teacher, _courseID, _classroom;
        private int _weekBegin, _weekEnd;
        private WeekOddEvenNone _weekOen;
        private DateTime _selectDate;
        private bool _isCustom;
        public static string[] WeekEvenOddToString = new string[3] { "双周", "单周", "" };

        /// <summary>
        /// 课程名称
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// 任课教师
        /// </summary>
        public string Teacher
        {
            get => _teacher;
            set => SetProperty(ref _teacher, value);
        }

        /// <summary>
        /// 课程编号
        /// </summary>
        public string CourseID
        {
            get => _courseID;
            set => SetProperty(ref _courseID, value);
        }

        /// <summary>
        /// 上课教室
        /// </summary>
        public string Classroom
        {
            get => _classroom;
            set => SetProperty(ref _classroom, value);
        }

        /// <summary>
        /// 开始周
        /// </summary>
        public int WeekBegin
        {
            get => _weekBegin;
            set => SetProperty(ref _weekBegin, value);
        }

        /// <summary>
        /// 结束周
        /// </summary>
        public int WeekEnd
        {
            get => _weekEnd;
            set => SetProperty(ref _weekEnd, value);
        }

        /// <summary>
        /// 单双周信息
        /// </summary>
        public WeekOddEvenNone WeekOen
        {
            get => _weekOen;
            set => SetProperty(ref _weekOen, value);
        }
        
        /// <summary>
        /// 选课日期
        /// </summary>
        public DateTime SelectDate
        {
            get => _selectDate;
            set => SetProperty(ref _selectDate, value);
        }

        /// <summary>
        /// 是否为自定义课程
        /// </summary>
        public bool IsCustom
        {
            get => _isCustom;
            set => SetProperty(ref _isCustom, value);
        }

        /// <summary>
        /// 创建一个课程表项目。
        /// </summary>
        public CurriculumItem()
        {
            _name = _teacher = _courseID = _classroom = string.Empty;
            _weekBegin = _weekEnd = 0;
            _weekOen = WeekOddEvenNone.None;
            _selectDate = DateTime.Now;
            _isCustom = false;
        }

        /// <summary>
        /// 在某一周是否显示？
        /// </summary>
        /// <param name="week">第x周。</param>
        /// <returns>是否显示。</returns>
        public bool IfShow(int week)
        {
            bool show = ((int)_weekOen == 2) || ((int)_weekOen == week % 2);
            show &= (week >= _weekBegin) && (week <= _weekEnd);
            return show;
        }

        /// <summary>
        /// 创建描述课程的参数。
        /// </summary>
        /// <returns>描述课程的信息。</returns>
        public override IEnumerable<CurriculumDescription> ToDescription()
        {
            yield return new CurriculumDescription(Name, Classroom);
        }

        /// <summary>
        /// 获取描述时间的字符串。
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public string DescribeTime => $"{WeekEvenOddToString[(int)WeekOen]}第{WeekBegin}-{WeekEnd}周";

        /// <summary>
        /// 比较是否为同一节课。
        /// </summary>
        /// <param name="that">另一节课。</param>
        /// <returns>比较结果。</returns>
        public bool CompareTo(CurriculumItem that)
        {
            if (this == that) return true;
            return this.Name == that.Name
                && this.DayBegin == that.DayBegin
                && this.DayEnd == that.DayEnd
                && this.Teacher == that.Teacher
                && this.Classroom == that.Classroom
                && this.WeekBegin == that.WeekBegin;
        }
    }
}
