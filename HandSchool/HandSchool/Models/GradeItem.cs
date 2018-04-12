using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace HandSchool
{
    namespace Models
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

    public interface IGradeEntrance : ISystemEntrance
    {
        string GPAPostValue { get; }
        Task<string> GatherGPA();
    }
}
