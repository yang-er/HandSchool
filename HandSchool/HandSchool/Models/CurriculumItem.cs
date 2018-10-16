using HandSchool.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace HandSchool.Models
{
    /// <summary>
    /// 课程在单双周的表现枚举
    /// </summary>
    public enum WeekOddEvenNone
    {
        /// <summary>
        /// 双周
        /// </summary>
        Even,

        /// <summary>
        /// 单周
        /// </summary>
        Odd,

        /// <summary>
        /// 单双周
        /// </summary>
        None
    }

    /// <summary>
    /// 描述课程的具体信息，用于显示。
    /// </summary>
    public class CurriculumDescription
    {
        internal CurriculumDescription(string tit, string desc)
        {
            Title = tit;
            Description = desc;
        }

        /// <summary>
        /// 课程的标题。
        /// </summary>
        public readonly string Title;

        /// <summary>
        /// 课程的描述，如操作地点和时间等。
        /// </summary>
        public readonly string Description;
    }

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

    /// <summary>
    /// 表示多节课程合并后的项目的类。
    /// </summary>
    public class CurriculumSet : CurriculumItemBase
    {
        /// <summary>
        /// 创建课程集合，方便输出。
        /// </summary>
        public CurriculumSet()
        {
            InnerList = new List<CurriculumItem>();
        }

        /// <summary>
        /// 内部课程列表
        /// </summary>
        public List<CurriculumItem> InnerList { get; set; }

        /// <summary>
        /// 添加课程项进入集合。
        /// </summary>
        /// <param name="item">将要被添加的课程项目。</param>
        public void Add(CurriculumItem item)
        {
            InnerList.Add(item);
        }

        /// <summary>
        /// 合并别的课程集合进入此集合。
        /// </summary>
        /// <param name="item">将要被合并的课程集合。</param>
        public void Add(CurriculumSet set)
        {
            InnerList.AddRange(set.InnerList);
        }

        /// <summary>
        /// 比较是否为同一节课。
        /// </summary>
        /// <param name="that">另一节课。</param>
        /// <returns>比较结果。</returns>
        public bool CompareTo(CurriculumSet that)
        {
            if (that is null || InnerList.Count != that.InnerList.Count)
                return false;
            for (int i = 0; i < InnerList.Count; i++)
                if (!InnerList[i].CompareTo(that.InnerList[i]))
                    return false;
            return true;
        }

        /// <summary>
        /// 处理内部的列表，进行合并。
        /// </summary>
        public void MergeClasses()
        {
            if (InnerList.Count == 0)
            {
                DayEnd = 0;
                return;
            }

            DayEnd = Core.App.DailyClassCount + 1;

            for (int i = 0; i < InnerList.Count; i++)
            {
                for (int j = i + 1; j < InnerList.Count; j++)
                {
                    if (InnerList[i].CompareTo(InnerList[j]))
                        InnerList.RemoveAt(j);
                }
            }

            InnerList.Sort((a, b) => a.WeekBegin.CompareTo(b.WeekBegin));

            foreach (var item in InnerList)
                DayEnd = Math.Min(DayEnd, item.DayEnd);
            WeekDay = InnerList[0].WeekDay;
        }
        
        /// <summary>
        /// 表示多节课程合并后的项目的类。
        /// </summary>
        public override IEnumerable<CurriculumDescription> ToDescription()
        {
            InnerList.Sort((a, b) => a.Name.CompareTo(b.Name));

            for (int i = 0; i < InnerList.Count; i++)
            {
                string title = InnerList[i].Name;
                string des = InnerList[i].DescribeTime;

                for (; i < InnerList.Count - 1; i++)
                {
                    if (InnerList[i+1].Name != InnerList[i].Name) break;
                    des += "\n" + InnerList[i+1].DescribeTime;
                }
                
                yield return new CurriculumDescription(title, des);
            }
        }

        /// <summary>
        /// 合并课程表的算法。
        /// </summary>
        public class MergeAlgorithm
        {
            readonly List<CurriculumSet>[] ItemGrid;
            readonly int cnt;
            bool merged = false;

            public MergeAlgorithm()
            {
                ItemGrid = new List<CurriculumSet>[8];
                cnt = Core.App.DailyClassCount;

                for (int i = 1; i <= 7; i++)
                {
                    ItemGrid[i] = new List<CurriculumSet>(cnt + 1);

                    for (int j = 0; j <= cnt; j++)
                    {
                        ItemGrid[i].Add(new CurriculumSet());
                    }
                }
            }

            public void AddClass(CurriculumItem newItem)
            {
                if (newItem.WeekDay == 0 || newItem.DayBegin < 1 || newItem.DayEnd > cnt) return;
                for (int i = newItem.DayBegin; i <= newItem.DayEnd; i++)
                {
                    ItemGrid[newItem.WeekDay][i].Add(newItem);
                    ItemGrid[newItem.WeekDay][i].DayBegin = i;
                }
            }

            public void MergeClassSet()
            {
                foreach (var dayList in ItemGrid)
                {
                    if (dayList is null) continue;

                    foreach (var classSet in dayList)
                    {
                        classSet.MergeClasses();
                    }

                    dayList.RemoveAt(0);

                    for (int i = 0; i < dayList.Count - 1; i++)
                    {
                        if (dayList[i].CompareTo(dayList[i + 1]) || dayList[i + 1].DayEnd == 0)
                        {
                            if (dayList[i + 1].DayBegin != 0)
                                dayList[i].DayEnd = dayList[i + 1].DayBegin;
                            dayList.RemoveAt(i + 1);
                            i--;
                        }
                    }

                    if (dayList[0].DayEnd == 0)
                    {
                        dayList.RemoveAt(0);
                    }
                }

                merged = true;
            }

            public IEnumerable<CurriculumSet> ToList()
            {
                if (!merged) MergeClassSet();

                foreach (var itemList in ItemGrid)
                {
                    if (itemList is null) continue;
                    foreach (var item in itemList)
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
