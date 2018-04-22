using HandSchool.Models;
using HandSchool.Services;
using System.Collections.Generic;
using static HandSchool.Internal.Helper;

namespace HandSchool
{
    public sealed partial class Core
    {
        private static Core _instance;
        public static Core App => _instance;
        public bool Loaded { get; set; } = false;
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

        public static bool Initialize()
        {
            _instance = new Core();
            foreach (var info in (typeof(Core)).GetProperties())
            {
                if (info.PropertyType.FullName == "HandSchool.Services.ISchoolWrapper")
                    Schools.Add(info.GetValue(_instance) as ISchoolWrapper);
            }

            var type = "jlu";
            if (type == "")
            {
                return false;
            }
            else
            {
                var current = Schools.Find((sw) => sw.SchoolId == type);
                current.PreLoad();
                current.PostLoad();
                _instance.Loaded = true;
                return true;
            }
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
