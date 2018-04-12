using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HandSchool
{
    public partial class Core
    {
        private static Core _instance;
        public static Core App => _instance;

        public ISchoolSystem Service;
        public IGradeEntrance GradePoint;
        public IScheduleEntrance Schedule;
        public IMessageEntrance Message;
        public IFeedEntrance Feed;
        public ISystemEntrance SelectCourse;
        public int DailyClassCount;
        public List<ObservableCollection<InfoEntranceWrapper>> InfoEntrances = new List<ObservableCollection<InfoEntranceWrapper>>();

        public static List<ISchoolWrapper> Schools { get; } = new List<ISchoolWrapper>();

        private Core() { }

        public static void Initialize()
        {
            _instance = new Core();
            foreach (var info in (typeof(Core)).GetProperties())
            {
                if (info.PropertyType.FullName == "HandSchool.ISchoolWrapper")
                    Schools.Add(info.GetValue(_instance) as ISchoolWrapper);
            }

            _instance.JLU.PreLoad();
            _instance.JLU.PostLoad();
        }
    }

    public interface ISchoolWrapper
    {
        void PreLoad();
        void PostLoad();
        string SchoolName { get; }
        string SchoolId { get; }
    }
}
