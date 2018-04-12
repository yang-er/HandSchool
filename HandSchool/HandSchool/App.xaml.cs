using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using HandSchool.Internal;
using HandSchool.ViewModels;
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
            NavigationViewModel.Instance.IsBusy = false;
            post.Invoke();
            //if (Device.RuntimePlatform == Device.UWP) return;
            MainPage = new MainPage();
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
        
    }
}
