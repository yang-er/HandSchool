using System;
using System.Diagnostics;
using System.Threading;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Internal
{
    /// <summary>
    /// 平台相关代码的接口要求。
    /// </summary>
    public abstract class PlatformBase
    {
        /// <summary>
        /// 创建一个登录页面。
        /// </summary>
        /// <param name="viewModel">登录页面的视图模型。</param>
        /// <returns>登录页面</returns>
        public abstract ILoginPage CreateLoginPage(LoginViewModel viewModel);

        /// <summary>
        /// 创建一个空白视图页面。
        /// </summary>
        /// <returns>视图页面的内容</returns>
        public abstract IViewPage CreatePage();

        /// <summary>
        /// 创建一个添加课程表的页面。
        /// </summary>
        /// <param name="item">课程表项</param>
        /// <param name="navigationContext">导航上下文</param>
        public abstract Task<bool> ShowNewCurriculumPage(CurriculumItem item, INavigate navigationContext);

        /// <summary>
        /// 开始菜单的添加，进行简单的初始化。
        /// </summary>
        public abstract void BeginMenu();

        /// <summary>
        /// 添加菜单入口点到主要页面中。
        /// </summary>
        /// <param name="title">入口点菜单的标题。</param>
        /// <param name="dest">目标页面的类名称，将通过反射创建实例。</param>
        /// <param name="category">学校命名空间，如果为空默认为全局类。</param>
        /// <param name="uwp">UWP 的图标。</param>
        /// <param name="ios">iOS 系统展示的图标。为空时收起到信息查询中。</param>
        public abstract void AddMenuEntry(string title, string dest, string category, string uwp, string ios);
        
        /// <summary>
        /// 打开网址页面。
        /// </summary>
        /// <param name="url">统一资源标识符</param>
        public void OpenUrl(string url)
        {
            EnsureOnMainThread(() =>
            {
                Device.OpenUri(new Uri(url));
            });
        }

        /// <summary>
        /// 设备的种类
        /// </summary>
        public virtual TargetIdiom Idiom => Device.Idiom;

        /// <summary>
        /// 完成菜单的添加。
        /// </summary>
        public abstract void FinalizeMenu();

        /// <summary>
        /// 应用商店链接
        /// </summary>
        public abstract string StoreLink { get; }

        /// <summary>
        /// 运行时名称
        /// </summary>
        public abstract string RuntimeName { get; }

        /// <summary>
        /// 设置文件夹
        /// </summary>
        public abstract string ConfigureDirectory { get; }

        /// <summary>
        /// 检查应用程序更新。
        /// </summary>
        public abstract void CheckUpdate();
        
        /// <summary>
        /// 在主线程上运行异步操作。
        /// </summary>
        [DebuggerStepThrough]
        public Task EnsureOnMainThread(Func<Task> task)
        {
            if (Thread.CurrentThread.ManagedThreadId == 1) return task();
            var awaiter = new Task(() => { });

            Device.BeginInvokeOnMainThread(async () =>
            {
                await task();
                awaiter.Start();
            });

            return awaiter;
        }

        /// <summary>
        /// 在主线程上运行异步操作。
        /// </summary>
        [DebuggerStepThrough]
        public void EnsureOnMainThread(Action action)
        {
            if (Thread.CurrentThread.ManagedThreadId == 1) action();
            else Device.BeginInvokeOnMainThread(action);
        }

        /// <summary>
        /// 在主线程上运行等待值返回的异步操作。
        /// </summary>
        [DebuggerStepThrough]
        public Task<T> EnsureOnMainThread<T>(Func<Task<T>> task)
        {
            if (Thread.CurrentThread.ManagedThreadId == 1) return task();
            var awaiter = new TaskCompletionSource<T>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                awaiter.TrySetResult(await task());
            });

            return awaiter.Task;
        }
    }
}
