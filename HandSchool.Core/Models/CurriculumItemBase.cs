using HandSchool.Internals;
using System.Collections.Generic;

namespace HandSchool.Models
{
    /// <summary>
    /// 实现了课程表内容的基类，可以表示课程在周中的时间。
    /// </summary>
    public abstract class CurriculumItemBase : NotifyPropertyChanged
    {
        private int _dayBegin, _dayEnd, _weekDay;

        /// <summary>
        /// 星期几
        /// </summary>
        public int WeekDay
        {
            get => _weekDay;
            set => SetProperty(ref _weekDay, value);
        }

        /// <summary>
        /// 开始节
        /// </summary>
        public int DayBegin
        {
            get => _dayBegin;
            set => SetProperty(ref _dayBegin, value);
        }

        /// <summary>
        /// 结束节
        /// </summary>
        public int DayEnd
        {
            get => _dayEnd;
            set => SetProperty(ref _dayEnd, value);
        }

        /// <summary>
        /// 创建描述课程的参数。
        /// </summary>
        /// <returns>描述课程的信息。</returns>
        public abstract IEnumerable<CurriculumDescription> ToDescription();
    }
}
