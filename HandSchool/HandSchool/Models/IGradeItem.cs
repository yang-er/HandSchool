using System;
using System.Collections.Specialized;

namespace HandSchool
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
}
