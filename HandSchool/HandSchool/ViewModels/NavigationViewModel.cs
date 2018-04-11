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
        
        NavigationPage _index = new NavigationPage(new IndexPage());
        NavigationPage _schedule = new NavigationPage(new SchedulePage());
        NavigationPage _config = new NavigationPage(new ConfigPage());
        NavigationPage _about = new NavigationPage(new AboutPage());
        NavigationPage _feed, _message, _grade, _query;

        private NavigationViewModel()
        {
            if (App.Current.Feed != null) _feed = new NavigationPage(new FeedPage());
            if (App.Current.Message != null) _message = new NavigationPage(new MessagePage());
            if (App.Current.GradePoint != null) _grade = new NavigationPage(new GradePointPage());
            if (App.Current.InfoEntrances.Count > 0) _query = new NavigationPage(new InfoQueryPage());
            
            PrimaryItems.Add(new MasterPageItem("首页", _index, "\xE10F", "tab_feed.png", true));
            PrimaryItems.Add(new MasterPageItem("课程表", _schedule, "\xECA5", "tab_feed.png"));
            if (_feed != null) PrimaryItems.Add(new MasterPageItem("学校通知", _feed, "\xED0D", "tab_feed.png"));
            if (_message != null) PrimaryItems.Add(new MasterPageItem("站内消息", _message, "\xE715", "tab_feed.png"));
            if (_grade != null) PrimaryItems.Add(new MasterPageItem("学分成绩", _grade, "\xE82D", "tab_feed.png"));
            if (_query != null) PrimaryItems.Add(new MasterPageItem("信息查询", _query, "\xE946", "tab_feed.png"));
            SecondaryItems.Add(new MasterPageItem("设置", _config, "\xE713", "tab_feed.png"));
            SecondaryItems.Add(new MasterPageItem("关于", _about, "\xE783", "tab_feed.png"));
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
