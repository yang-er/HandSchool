using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

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
        NameValueCollection Attach { get; }
        string Show { get; }
    }

    public class Grade
    {
        public static ObservableCollection<IGradeItem> Value = new ObservableCollection<IGradeItem>();
        public static event OnAddGrade OnAddGrade;

        public static void Add(IGradeItem iGi)
        {
            Value.Add(iGi);
            // OnAddGrade.Invoke(iGi);
        }
    }
}
