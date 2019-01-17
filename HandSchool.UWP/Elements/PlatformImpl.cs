using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace HandSchool.UWP
{
    /// <summary>
    /// UWP平台相关代码的接口要求。
    /// </summary>
    internal class PlatformImpl : PlatformBase
    {
        /// <summary>
        /// 平台实现的实例。
        /// </summary>
        public static PlatformImpl Instance { get; private set; }

        /// <summary>
        /// 应用商店检查更新链接
        /// </summary>
        public string UpdateSourceLink { get; }
        
        /// <summary>
        /// 系统导航内容
        /// </summary>
        public List<NavigationMenuItemImpl> NavigationItems { get; private set; }

        /// <summary>
        /// 初始化平台相关的参数。
        /// </summary>
        private PlatformImpl()
        {
            StoreLink = "ms-windows-store://review/?productid=9PD2FR9HHJQP";
            UpdateSourceLink = "ms-windows-store://pdp/?productid=9PD2FR9HHJQP";
            ConfigureDirectory = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            RuntimeName = "UWP";

            Core.InitPlatform(Instance = this);
            NavigationItems = new List<NavigationMenuItemImpl>();
            ViewResponseImpl = new ViewResponseImpl();
        }

        /// <summary>
        /// 注册平台相关代码。
        /// </summary>
        public static void Register()
        {
            Debug.Assert(Instance is null);
            new PlatformImpl();

            Core.Reflection.RegisterType<MessageDetailPage>();
            Core.Reflection.RegisterType<WebViewPage>();
            Core.Reflection.RegisterType<InfoQueryPage, InfoQueryPageF>();
        }

        /// <summary>
        /// 创建一个登录页面。
        /// </summary>
        /// <param name="viewModel">登录页面的视图模型。</param>
        /// <returns>登录页面</returns>
        public override ILoginPage CreateLoginPage(LoginViewModel viewModel)
        {
            return new LoginDialog(viewModel);
        }
        
        /// <summary>
        /// 创建一个添加课程表的页面。
        /// </summary>
        /// <param name="item">课程表项</param>
        /// <param name="navigationContext">导航上下文</param>
        public override async Task<bool> ShowNewCurriculumPageAsync(CurriculumItem item, Views.INavigate navigationContext)
        {
            var dialog = new CurriculumDialog(item, true);
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        /// <summary>
        /// 检查应用程序更新。
        /// </summary>
        public override void CheckUpdate()
        {
            OpenUrl(UpdateSourceLink);
        }
        
        /// <summary>
        /// 添加菜单入口点到主要页面中。
        /// </summary>
        /// <param name="title">入口点菜单的标题。</param>
        /// <param name="dest">目标页面的类名称，将通过反射创建实例。</param>
        /// <param name="category">学校命名空间，如果为空默认为全局类。</param>
        /// <param name="uwp">UWP 的图标。</param>
        /// <param name="ios">iOS 系统展示的图标。为空时收起到信息查询中。</param>
        public override void AddMenuEntry(string title, string dest, string category, string uwp, string ios)
        {
            NavigationItems.Add(new NavigationMenuItemImpl(title, dest, category, uwp));
        }
    }
}
