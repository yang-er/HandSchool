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
        public ISchoolSystem Service;
        public IGradeEntrance GradePoint;
        public IScheduleEntrance Schedule;
        public IMessageEntrance Message;
        public IFeedEntrance Feed;
        public ISystemEntrance SelectCourse;
        public int DailyClassCount;
        public List<InfoEntranceGroup> InfoEntrances = new List<InfoEntranceGroup>();
        public static List<ISchoolWrapper> Schools { get; } = new List<ISchoolWrapper>();
        public static string Version => "1.4.9.0";
#if __UWP__
        public static string RuntimePlatform => "UWP";
        public static T OnPlatform<T>(T android, T ios, T uwp) => uwp;
#elif __IOS__
        public static string RuntimePlatform => "iOS";
        public static T OnPlatform<T>(T android, T ios, T uwp) => ios;
#elif __ANDROID__
        public static string RuntimePlatform => "Android";
        public static T OnPlatform<T>(T android, T ios, T uwp) => android;
#endif

        private Core() { }

        public static bool Initialize()
        {
            if (_instance != null) return true;

            _instance = new Core();
            foreach (var info in (typeof(Core)).GetProperties())
            {
                if (info.PropertyType == typeof(ISchoolWrapper))
                    Schools.Add(info.GetValue(_instance) as ISchoolWrapper);
            }

            var type = ReadConfFile("hs.school.bin");
            if (type == "")
            {
                return false;
            }
            else
            {
                var current = Schools.Find((sw) => sw.SchoolId == type);
                current.PreLoad();
                current.PostLoad();
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
