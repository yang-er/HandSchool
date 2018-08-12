using HandSchool.Models;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class NavigationViewModel : BaseViewModel
    {
        public List<MasterPageItem> PrimaryItems { get; set; } = new List<MasterPageItem>();
        public List<MasterPageItem> SecondaryItems { get; set; } = new List<MasterPageItem>();
        public List<MasterPageItem> AppleItems { get; set; } = new List<MasterPageItem>();
        public InfoEntranceGroup InAppEntrance { get; set; } = new InfoEntranceGroup("其他功能");
        
        static NavigationViewModel _instance;
        public static NavigationViewModel Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new NavigationViewModel();
                return _instance;
            }
        }
        
        private NavigationViewModel()
        {
            FetchOptions();
        }

        /// <summary>
        /// 添加菜单入口点
        /// </summary>
        /// <param name="title">菜单标题</param>
        /// <param name="dest">目标页面类名称</param>
        /// <param name="icon">UWP的图标</param>
        /// <param name="cg">分类</param>
        /// <param name="sel">是否被默认选中</param>
        /// <param name="apple">iOS系统展示的图标</param>
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
        /// 根据现有的条目添加菜单入口点
        /// </summary>
        public void FetchOptions()
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
                    (nav) => new WebViewPage(AboutViewModel.Instance).ShowAsync(nav))
            };
#endif

            AddMenuEntry("首页", "IndexPage", "\xE10F", sel: true, apple: "tab_feed.png");
            AddMenuEntry("课程表", "SchedulePage", "\xECA5", apple: "tab_feed.png");

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

        /// <summary>
        /// 猜想当前页面
        /// </summary>
        /// <returns>Android版挂起恢复时返回的页面</returns>
        public NavigationPage GuessCurrentPage()
        {
            var navitem = PrimaryItems.Find((item) => item.Selected);
            if (navitem is null) navitem = SecondaryItems.Find((item) => item.Selected);
            if (navitem is null) navitem = PrimaryItems[0];
            return navitem.DestPage;
        }
    }
}
