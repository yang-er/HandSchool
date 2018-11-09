using HandSchool.Models;
using HandSchool.Views;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 界面导航的视图模型，提供了视图服务。
    /// </summary>
    public class NavigationViewModel : BaseViewModel
    {
        static NavigationViewModel _instance;

        /// <summary>
        /// Android 版本侧栏入口点列表
        /// </summary>
        public List<MasterPageItem> PrimaryItems { get; }

        /// <summary>
        /// Android 版本侧栏设置
        /// </summary>
        public List<MasterPageItem> SecondaryItems { get; }

        /// <summary>
        /// iOS 版本下底栏
        /// </summary>
        public List<MasterPageItem> AppleItems { get; }

        /// <summary>
        /// iOS 版本下非底栏入口点列表
        /// </summary>
        public InfoEntranceGroup InAppEntrance { get; }
        
        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static NavigationViewModel Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new NavigationViewModel();
                return _instance;
            }
        }
        
        /// <summary>
        /// 加载预定义的所有菜单内容。
        /// </summary>
        private NavigationViewModel()
        {
            PrimaryItems = new List<MasterPageItem>();
            SecondaryItems = new List<MasterPageItem>();
#if __IOS__
            AppleItems = new List<MasterPageItem>();
            InAppEntrance = new InfoEntranceGroup("其他功能");
#endif
            FetchOptions();
        }

        /// <summary>
        /// 添加菜单入口点到主要页面中。
        /// </summary>
        /// <param name="title">入口点菜单的标题。</param>
        /// <param name="dest">目标页面的类名称，将通过反射创建实例。</param>
        /// <param name="icon">UWP 的图标。</param>
        /// <param name="cg">学校命名空间，如果为空默认为全局类。</param>
        /// <param name="sel">是否被默认选中。</param>
        /// <param name="apple">iOS 系统展示的图标。为空时收起到信息查询中。</param>
        public void AddMenuEntry(string title, string dest, string icon, string cg = "", bool sel = false, string apple = "")
        {
            var item = new MasterPageItem(title, dest, icon, sel, cg);
            PrimaryItems.Add(item);

#if __IOS__
            if (apple != "")
            {
                AppleItems.Add(item);
                item.AppleIcon = new FileImageSource { File = apple };
            }
            else
            {
                InAppEntrance.Add(new TapEntranceWrapper(item.Title, item.Title + "的入口。",
                    (nav) => nav.PushAsync(item.CorePage)
                ));
            }
#endif
        }
        
        /// <summary>
        /// 根据现有的预定义菜单内容添加入口点。
        /// </summary>
        private void FetchOptions()
        {
            PrimaryItems.Clear();
            SecondaryItems.Clear();
            
            SecondaryItems.Add(new MasterPageItem("设置", "SettingPage"));

#if __IOS__
            AppleItems.Clear();
            InAppEntrance.Clear();

            var settingList = new InfoEntranceGroup("设置")
            {
                new TapEntranceWrapper("设置", "调整程序运行的参数。",
                    (nav) => nav.PushAsync(SecondaryItems[0].CorePage)),
                new TapEntranceWrapper("关于", "程序的版本信息、开发人员、许可证和隐私声明等。",
                    (nav) => nav.PushAsync(new AboutPage())),
            };
#endif

            AddMenuEntry("首页", "IndexPage", "\xE10F", sel: true, apple: "tab_rec.png");
            AddMenuEntry("课程表", "SchedulePage", "\xECA5", apple: "tab_sched.png");

#if !__ANDROID__
            if (Core.App.Feed != null)
                AddMenuEntry("学校通知", "FeedPage", "\xED0D", apple: "tab_feed.png");
            if (Core.App.Message != null)
                AddMenuEntry("站内消息", "MessagePage", "\xE715");
#else
            if (Core.App.Feed != null && Core.App.Message != null)
                AddMenuEntry("消息通知", "MessageTabbedPage", "\xE715");
            else if (Core.App.Feed != null)
                AddMenuEntry("学校通知", "FeedPage", "\xED0D");
            else if (Core.App.Message != null)
                AddMenuEntry("站内消息", "MessagePage", "\xE715");
#endif

            if (Core.App.GradePoint != null)
                AddMenuEntry("学分成绩", "GradePointPage", "\xE82D");
            if (Core.App.InfoEntrances.Count > 0 || Core.RuntimePlatform == "iOS")
                AddMenuEntry("信息查询", "InfoQueryPage", "\xE946", apple: "tab_about.png");

#if __IOS__
            if (InAppEntrance.Count > 0)
                Core.App.InfoEntrances.Insert(0, InAppEntrance);
            Core.App.InfoEntrances.Add(settingList);
#endif
        }
    }
}
