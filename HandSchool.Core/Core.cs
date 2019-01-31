using HandSchool.Internal;
using HandSchool.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace HandSchool
{
    /// <summary>
    /// 程序核心类，提供了学校接口的访问和平台服务。
    /// </summary>
    public static partial class Core
    {
        /// <summary>
        /// 单例的加载了当前学校的App
        /// </summary>
        public static SchoolApplication App { get; private set; }

        /// <summary>
        /// 配置管理
        /// </summary>
        public static ConfigurationManager Configure { get; private set; }

        /// <summary>
        /// 反射处理
        /// </summary>
        public static ReflectionManager Reflection => ReflectionManager.Lazy.Value;

        /// <summary>
        /// 日志管理
        /// </summary>
        public static Logger Logger { get; private set; }

        /// <summary>
        /// 平台相关实现
        /// </summary>
        public static PlatformBase Platform { get; private set; }

        /// <summary>
        /// 当前软件版本号
        /// </summary>
        public static string Version => "2.0.19.0";

        /// <summary>
        /// 可用学校列表
        /// </summary>
        public static List<ISchoolWrapper> Schools { get; } = new List<ISchoolWrapper>();

        /// <summary>
        /// 是否已经加载完成
        /// </summary>
        public static bool Initialized => !(App?.Loader is null);
        
        /// <summary>
        /// 初始化平台相关内容
        /// </summary>
        /// <param name="platform">平台实现</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void InitPlatform(PlatformBase platform)
        {
            if (platform == null) return;
            Platform = platform;
            Configure = new ConfigurationManager(Platform.ConfigureDirectory);
            Logger = new Logger();
        }

        /// <summary>
        /// 初始化核心程序
        /// </summary>
        /// <returns>是否已经加载对应学校</returns>
        public static bool Initialize()
        {
            if (App != null) return true;
            App = new SchoolApplication();
            
            var type = Configure.Read("hs.school.bin");
            if (type == "") return false;
            var current = Schools.Find((sw) => sw.SchoolId == type);
            if (current is null) return false;
            App.InjectService(current);
            current.PreLoad();
            current.PostLoad();
            return true;
        }
    }
}
