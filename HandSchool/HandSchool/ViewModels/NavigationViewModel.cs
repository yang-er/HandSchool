using HandSchool.Views;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class NavigationViewModel : BaseViewModel
    {
        public List<MasterPageItem> PrimaryItems { get; set; } = new List<MasterPageItem>();
        public List<MasterPageItem> SecondaryItems { get; set; } = new List<MasterPageItem>();

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
            PrimaryItems.Add(new MasterPageItem("首页", "IndexPage", "\xE10F", "tab_feed.png", true));
            PrimaryItems.Add(new MasterPageItem("课程表", "SchedulePage", "\xECA5", "tab_feed.png"));
            if (Core.App.Feed != null) PrimaryItems.Add(new MasterPageItem("学校通知", "FeedPage", "\xED0D", "tab_feed.png"));
            if (Core.App.Message != null) PrimaryItems.Add(new MasterPageItem("站内消息", "MessagePage", "\xE715", "tab_feed.png"));
            if (Core.App.GradePoint != null) PrimaryItems.Add(new MasterPageItem("学分成绩", "GradePointPage", "\xE82D", "tab_feed.png"));
            if (Core.App.InfoEntrances.Count > 0) PrimaryItems.Add(new MasterPageItem("信息查询", "InfoQueryPage", "\xE946", "tab_feed.png"));
            SecondaryItems.Add(new MasterPageItem("设置", "ConfigPage", "\xE713", "tab_feed.png"));
            SecondaryItems.Add(new MasterPageItem("关于", "AboutPage", "\xE783", "tab_feed.png"));
        }

        public static Page GetMainPage()
        {
#if __IOS__
            return new MainPage();
#else
            return new MainPage();
#endif
        }
    }
}
