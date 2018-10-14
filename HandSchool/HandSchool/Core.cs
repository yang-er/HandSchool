using System;
using System.IO;

namespace HandSchool
{
    /// <summary>
    /// 程序核心类，提供了学校接口的访问和平台服务。
    /// </summary>
    public sealed partial class Core
    {
        /// <summary>
        /// 单例的加载了当前学校的App
        /// </summary>
        public static Core App { get; private set; }
        
        /// <summary>
        /// 当前软件版本号
        /// </summary>
        public static string Version => "1.6.13.0";
        
        /// <summary>
        /// 初始化核心程序
        /// </summary>
        /// <returns>是否已经加载对应学校</returns>
        public static bool Initialize()
        {
            if (App != null) return true;
            
            App = new Core();
            ListSchools();

#if __UWP__
            ConfigDirectory = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#elif __IOS__
            ConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
#elif __ANDROID__
            ConfigDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#elif __MOCKS__
            ConfigDirectory = Path.Combine(Environment.CurrentDirectory, "log");
#endif

            var type = ReadConfig("hs.school.bin");

            if (type == "")
            {
                return false;
            }
            else
            {
                var current = Schools.Find((sw) => sw.SchoolId == type);
                if (current is null) return false;
                Profile(0, "学校加载");
                current.PreLoad();
                Profile(1, "学校预加载完成");
                current.PostLoad();
                Profile(1, "学校后加载完成");

                Profile(2);
                return true;
            }
        }
        
        private Core() { }
    }
}
