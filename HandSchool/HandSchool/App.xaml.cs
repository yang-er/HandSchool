using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using HandSchool.Internal;
using HandSchool.Views;
using Xamarin.Forms;

namespace HandSchool
{
	public partial class App : Application
	{

        #region School Components

        /// <summary>将学校功能加载到此 App 实例，并返回之后要调用的操作。</summary>
        public delegate Action LoadSchool();
        public Dictionary<string, LoadSchool> Support = new Dictionary<string, LoadSchool>();
        public ISchoolSystem Service;
        public IGradeEntrance GradePoint;
        public IScheduleEntrance Schedule;
        public IMessageEntrance Message;
        public IFeedEntrance Feed;
        public ISystemEntrance SelectCourse;
        public int DailyClassCount;
        public static new App Current => Application.Current as App;
        public List<ObservableCollection<InfoEntranceWrapper>> InfoEntrances = new List<ObservableCollection<InfoEntranceWrapper>>();

        #endregion

        public App()
        {
            InitializeComponent();
            Support.Add("吉林大学", LoadJLU);
            var post = LoadJLU();

            PrimaryItems = new List<MasterPageItem> {
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
                    Icon = "\xE11F",
                    Color = Color.Black,
                    Selected = false,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                },
                new MasterPageItem
                {
                    DestPage = new NavigationPage(new FeedPage()),
                    Title = "学校通知",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE12F",
                    Color = Color.Black,
                    Selected = false,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                },
                new MasterPageItem
                {
                    DestPage = new NavigationPage(new MessagePage()),
                    Title = "站内消息",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE12F",
                    Color = Color.Black,
                    Selected = false,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                },
                new MasterPageItem
                {
                    DestPage = new NavigationPage(new GradePointPage()),
                    Title = "学分成绩",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE12F",
                    Color = Color.Black,
                    Selected = false,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                },
                new MasterPageItem
                {
                    DestPage = new NavigationPage(new InfoQueryPage()),
                    Title = "信息查询",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE12F",
                    Color = Color.Black,
                    Selected = false,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                }
            };

            SecondaryItems = new List<MasterPageItem> {
                new MasterPageItem
                {
                    DestPage = new NavigationPage(new AboutPage()),
                    Title = "关于",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE783",
                    Color = Color.Black,
                    Selected = false,
                    AppleIcon = new FileImageSource { File = "tab_feed.png" }
                }
            };
            
            post.Invoke();
            if (Device.RuntimePlatform == Device.iOS)
            {
                MainPage = new TabMainPage();
            }
            else
            { 
                MainPage = new MainPage();
            }
        }

        #region State Change

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

        #endregion

        #region Navigating Items

        public Color ActiveColor => Color.FromRgb(0, 120, 215);
        public List<MasterPageItem> PrimaryItems;
        public List<MasterPageItem> SecondaryItems;

        #endregion

    }
}
