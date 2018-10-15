using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace HandSchool
{
    public sealed partial class Core
    {
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
        /// 类似Xamarin的OnPlatform函数，三个平台同时传入。
        /// </summary>
        /// <param name="android">Android 平台上的值。</param>
        /// <param name="ios">iOS 平台上的值。</param>
        /// <param name="uwp">通用 Windows 平台上的值。</param>
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
        /// 数据基础目录
        /// </summary>
        public static string ConfigDirectory { get; private set; }

        /// <summary>
        /// 从文件读取配置数据。
        /// </summary>
        /// <param name="name">即将读取的文件名。</param>
        /// <returns>读取得到的内容。</returns>
        [DebuggerStepThrough]
        public static string ReadConfig(string name)
        {
            string fn = Path.Combine(ConfigDirectory, name);
            return File.Exists(fn) ? File.ReadAllText(fn) : "";
        }

        /// <summary>
        /// 将配置数据写入文件。
        /// </summary>
        /// <param name="name">即将写入的文件名。</param>
        /// <param name="value">将要写入的内容。</param>
        [DebuggerStepThrough]
        public static void WriteConfig(string name, string value)
        {
            File.WriteAllText(Path.Combine(ConfigDirectory, name), value);
        }
        
        /// <summary>
        /// 在主线程上运行异步操作。
        /// </summary>
        [DebuggerStepThrough]
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
        /// 在主线程上运行等待值返回的异步操作。
        /// </summary>
        [DebuggerStepThrough]
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
    }
}
