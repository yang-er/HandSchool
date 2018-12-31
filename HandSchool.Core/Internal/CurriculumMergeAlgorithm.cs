using HandSchool.Models;
using System.Collections.Generic;

namespace HandSchool.Internal
{
    /// <summary>
    /// 合并课程表的算法。
    /// </summary>
    internal sealed class CurriculumMergeAlgorithm
    {
        readonly List<CurriculumSet>[] ItemGrid;
        readonly int cnt;
        bool merged = false;

        public CurriculumMergeAlgorithm()
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
