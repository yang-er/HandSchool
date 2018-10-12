using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace HandSchool
{
    /// <summary>
    /// 程序核心类
    /// </summary>
    public sealed partial class Core
    {
        #region 学校服务

        /// <summary>
        /// 学校的教务中心服务
        /// </summary>
        public ISchoolSystem Service;

        /// <summary>
        /// 获取绩点的入口点
        /// </summary>
        public IGradeEntrance GradePoint;

        /// <summary>
        /// 获取课程表的入口点
        /// </summary>
        public IScheduleEntrance Schedule;

        /// <summary>
        /// 获取系统消息的入口点
        /// </summary>
        public IMessageEntrance Message;

        /// <summary>
        /// 获取消息更新的入口点
        /// </summary>
        public IFeedEntrance Feed;
        public ISystemEntrance SelectCourse;

        /// <summary>
        /// 每天有多少节标准课时
        /// </summary>
        public int DailyClassCount;

        /// <summary>
        /// 信息查询入口点创造
        /// </summary>
        public List<InfoEntranceGroup> InfoEntrances = new List<InfoEntranceGroup>();

        #endregion

        #region 运行时服务

        /// <summary>
        /// 单例的加载了当前学校的App
        /// </summary>
        public static Core App { get; private set; }

        /// <summary>
        /// 可用学校列表
        /// </summary>
        public static List<ISchoolWrapper> Schools { get; } = new List<ISchoolWrapper>();

        /// <summary>
        /// 当前软件版本号
        /// </summary>
        public static string Version => "1.6.13.0";

        /// <summary>
        /// 当前软件运行的平台
        /// </summary>
#if __UWP__
        public static string RuntimePlatform => "UWP";
#elif __IOS__
        public static string RuntimePlatform => "iOS";
#elif __ANDROID__
        public static string RuntimePlatform => "Android";
#elif __MOCKS__
        public static string RuntimePlatform => "UnitTest";
#endif

        /// <summary>
        /// 类似Xamarin的OnPlatform函数
        /// </summary>
#if __UWP__
        public static T OnPlatform<T>(T android, T ios, T uwp) => uwp;
#elif __IOS__
        public static T OnPlatform<T>(T android, T ios, T uwp) => ios;
#elif __ANDROID__
        public static T OnPlatform<T>(T android, T ios, T uwp) => android;
#elif __MOCKS__
        public static T OnPlatform<T>(T android, T ios, T uwp) => android;
#endif

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
                current.PreLoad();
                current.PostLoad();
                return true;
            }
        }

        /// <summary>
        /// 数据基础目录
        /// </summary>
        public static string ConfigDirectory { get; private set; }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>读取内容</returns>
        public static string ReadConfig(string name)
        {
            string fn = Path.Combine(ConfigDirectory, name);
            return File.Exists(fn) ? File.ReadAllText(fn) : "";
        }

        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="name">文件名</param>
        /// <param name="value">写入内容</param>
        public static void WriteConfig(string name, string value)
        {
            File.WriteAllText(Path.Combine(ConfigDirectory, name), value);
        }

        #endregion

        #region 调试服务

        /// <summary>
        /// 调试阶段的断言
        /// </summary>
        /// <param name="cond">条件</param>
        /// <param name="val">断言内容</param>
        public static void Assert(bool cond, string val)
        {
#if DEBUG
            if (cond) throw new Exception(val);
#endif
        }

        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="output">内容</param>
        public static void Log(string output)
        {
            Debug.WriteLine(output);
        }

        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="output">内容</param>
        public static void Log(Exception output)
        {
            Debug.WriteLine(output);
        }

        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="format">格式</param>
        /// <param name="param">参数</param>
        public static void Log(string format, params object[] param)
        {
            Debug.WriteLine(format, param);
        }
        
        /// <summary>
        /// 在主线程上运行异步操作
        /// </summary>
        public static Task EnsureOnMainThread(Func<Task> task)
        {
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != 1)
            {
                var awaiter = new Task(() => { });

                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    await task();
                    awaiter?.Start();
                });

                return awaiter;
            }
            else
            {
                return task();
            }
        }

        /// <summary>
        /// 在主线程上运行异步操作
        /// </summary>
        public static Task<T> EnsureOnMainThread<T>(Func<Task<T>> task)
        {
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != 1)
            {
                var awaiter = new TaskCompletionSource<T>();

                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    awaiter.TrySetResult(await task());
                });

                return awaiter.Task;
            }
            else
            {
                return task();
            }
        }

        private Core() { }

        #endregion
    }
}
