using System;
using HandSchool.Models;
using System.Collections.Generic;

namespace HandSchool.Internals
{
    /// <summary>
    /// 合并课程表的算法。
    /// </summary>
    /// <author>miasakachenmo & Xhl</author>
    internal sealed class CurriculumMergeAlgorithm
    {
        private int _classCount;
        private List<CurriculumSet>[] _mergeCache;
        //把所有加入的课程展开成一个7*课程数的表格
        private readonly CurriculumSet[,] _curriculumSetGrid;

        public CurriculumMergeAlgorithm(int classCount)
        {
            _classCount = classCount;
            _curriculumSetGrid = new CurriculumSet[7 + 1, classCount + 1];
        }
        public void AddClass(CurriculumItem item)
        {
            _mergeCache = null;
            for (var i = item.DayBegin;  i <= item.DayEnd; i++)
            {
                _curriculumSetGrid[item.WeekDay, i] ??= new CurriculumSet {DayBegin = i, DayEnd = i, WeekDay = item.WeekDay};
                _curriculumSetGrid[item.WeekDay, i].Add(item);
            }
        }
        
        private List<CurriculumSet>[] Merge()
        {
            try
            {
                var res = new List<CurriculumSet>[8];
                for (var i = 1; i <= 7; i++)
                {
                    res[i] = new List<CurriculumSet>();
                    var s = 1;
                    while (s <= _classCount && _curriculumSetGrid[i, s] is null) s++;
                    var e = s + 1;
                    while (s <= _classCount && e <= _classCount)
                    {
                        if (_curriculumSetGrid[i, s] is null) break;
                        if (_curriculumSetGrid[i, s].SameAs(_curriculumSetGrid[i, e]))
                        {
                            e++;
                        }
                        else
                        {
                            var set = new CurriculumSet
                            {
                                DayBegin = s, DayEnd = e - 1, WeekDay = i
                            };
                            set.Add(_curriculumSetGrid[i, s]);
                            res[i].Add(set);
                            
                            s = e;
                            while (s <= _classCount && _curriculumSetGrid[i, s] is null) s++;
                            e = s + 1;
                        }
                    }

                    if (s < _classCount && !(_curriculumSetGrid[i, s] is null))
                    {
                        var curriculumSet = new CurriculumSet();
                        curriculumSet.Add(_curriculumSetGrid[i, s]);
                        curriculumSet.DayBegin = s;
                        curriculumSet.DayEnd = e - 1;
                        curriculumSet.WeekDay = i;
                        res[i].Add(curriculumSet);
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                Core.Logger.WriteLine("error", e.Message);
                return Array.Empty<List<CurriculumSet>>();
            }
        }
        
        public IEnumerable<CurriculumSet> ToList()
        {
            _mergeCache ??= Merge();
            for (var i = 1; i <= 7; i++)
            {
                foreach (var item in _mergeCache[i])
                {
                    yield return item;
                }
            }
        }
    }
}
