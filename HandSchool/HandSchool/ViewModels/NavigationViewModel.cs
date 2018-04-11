using HandSchool.Internal;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class NavigationViewModel : BaseViewModel
    {
        public Color ActiveColor => Color.FromRgb(0, 120, 215);
        public ObservableCollection<MasterPageItem> PrimaryItems { get; set; }
        public ObservableCollection<MasterPageItem> SecondaryItems { get; set; }

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
            PrimaryItems = new ObservableCollection<MasterPageItem> {
                new MasterPageItem
                {
                    DestPage = new NavigationPage(new IndexPage()),
                    Title = "首页",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE10F",
                    Color = ActiveColor,
                    Selected = true,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                },
                new MasterPageItem
                {
                    DestPage = new NavigationPage(new SchedulePage()),
                    Title = "课程表",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xECA5",
                    Color = Color.Black,
                    Selected = false,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                },
                new MasterPageItem
                {
                    DestPage = new NavigationPage(new FeedPage()),
                    Title = "学校通知",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xED0D",
                    Color = Color.Black,
                    Selected = false,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                },
                new MasterPageItem
                {
                    DestPage = new NavigationPage(new MessagePage()),
                    Title = "站内消息",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE715",
                    Color = Color.Black,
                    Selected = false,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                },
                new MasterPageItem
                {
                    DestPage = new NavigationPage(new GradePointPage()),
                    Title = "学分成绩",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE82D",
                    Color = Color.Black,
                    Selected = false,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                },
                new MasterPageItem
                {
                    DestPage = new NavigationPage(new InfoQueryPage()),
                    Title = "信息查询",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE946",
                    Color = Color.Black,
                    Selected = false,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                }
            };

            SecondaryItems = new ObservableCollection<MasterPageItem>
            {
                new MasterPageItem
                {
                    DestPage = new NavigationPage(new ConfigPage()),
                    Title = "设置",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE713",
                    Color = Color.Black,
                    Selected = false,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                },
                new MasterPageItem
                {
                    DestPage = new NavigationPage(new AboutPage()),
                    Title = "关于",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE783",
                    Color = Color.Black,
                    Selected = false,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                },
            };
        }

        public static Page GetMainPage()
        {
#if __IOS__
            return new TabMainPage();
#else
            return new MainPage();
#endif
        }
    }
}
