using HandSchool.Internal;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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
        /// 平台相关实现
        /// </summary>
        public static PlatformBase Platform { get; private set; }

        /// <summary>
        /// 当前软件版本号
        /// </summary>
        public static string Version => "1.7.17.0";

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
        public static void InitPlatform(PlatformBase platform)
        {
            Debug.Assert(Platform == null);
            Debug.Assert(platform != null);
            Platform = platform;
            Configure = new ConfigurationManager(Platform.ConfigureDirectory);
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

        /// <summary>
        /// 获取所有的学校程序集并尝试载入。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAvaliableSchools()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var file in Directory.EnumerateFiles(baseDir, "HandSchool.*.dll"))
            {
                var fileShort = file.Replace(baseDir, "");
                // if (fileShort == "HandSchool.Core.dll") continue;
                yield return fileShort.Replace(".dll", "");
            }
        }
        
        /// <summary>
        /// 当程序集加载时，检查是否是学校对应的代码。
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">包含了程序集的参数</param>
        public static void AssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            var assembly = args.LoadedAssembly;
            var export = assembly.GetCustomAttribute<ExportSchoolAttribute>();
            if (export is null) return;
            var loader = Activator.CreateInstance(export.RegisterType) as ISchoolWrapper;
            Schools.Add(loader);
        }

        /// <summary>
        /// 向调试器写入调试信息。
        /// </summary>
        /// <param name="output">输出的字符串内容。</param>
        [DebuggerStepThrough]
        public static void Log(string output) => Debug.WriteLine(output);

        /// <summary>
        /// 向调试器写入调试信息。
        /// </summary>
        /// <param name="output">产生的异常内容。</param>
        [DebuggerStepThrough]
        public static void Log(Exception output) => Debug.WriteLine(output);

        /// <summary>
        /// 向调试器写入调试信息。
        /// </summary>
        /// <param name="format">字符串的输出格式。</param>
        /// <param name="param">对应输出内容的参数。</param>
        [DebuggerStepThrough]
        public static void Log(string format, params object[] param) => Debug.WriteLine(format, param);
    }
}
