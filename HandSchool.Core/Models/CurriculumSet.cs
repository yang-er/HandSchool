using System;
using System.Collections.Generic;

namespace HandSchool.Models
{
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
                    if (InnerList[i + 1].Name != InnerList[i].Name) break;
                    des += "\n" + InnerList[i + 1].DescribeTime;
                }

                yield return new CurriculumDescription(title, des);
            }
        }
    }
}
