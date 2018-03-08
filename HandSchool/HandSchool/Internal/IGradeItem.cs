using System;
using System.Collections.Generic;

namespace HandSchool.Internal
{
    public interface IGradeItem
    {
        string Name { get; }
        string Score { get; }
        string Point { get; }
        string Type { get; }
        string Credit { get; }
        bool ReSelect { get; }
        bool Pass { get; }
        string Term { get; }
        DateTime Date { get; }
    }

    public class Grade
    {
        public static List<IGradeItem> Value = new List<IGradeItem>();
        public static event OnAddGrade OnAddGrade;

        public static void Add(IGradeItem iGi)
        {
            Value.Add(iGi);
            OnAddGrade.Invoke(iGi);
        }
    }
}
