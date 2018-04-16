using HandSchool.Models;
using HandSchool.Services;
using System.Collections.Generic;

namespace HandSchool
{
    public partial class Core
    {
        private static Core _instance;
        public static Core App => _instance;
        public bool Confirmed { get; set; } = false;
        public ISchoolSystem Service;
        public IGradeEntrance GradePoint;
        public IScheduleEntrance Schedule;
        public IMessageEntrance Message;
        public IFeedEntrance Feed;
        public ISystemEntrance SelectCourse;
        public int DailyClassCount;
        public List<InfoEntranceGroup> InfoEntrances = new List<InfoEntranceGroup>();
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

    namespace Services
    {
        public interface ISchoolWrapper
        {
            void PreLoad();
            void PostLoad();
            string SchoolName { get; }
            string SchoolId { get; }
        }
    }
}
