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
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }
        
        protected override void OnStart()
		{
            // Handle when your app starts
        }

        Page LastDetail;

		protected override void OnSleep()
		{
            // Handle when your app sleeps
            var pg = MainPage as MainPage;
            LastDetail = pg.Detail;
            pg.Detail = new ContentPage();
        }

		protected override void OnResume()
		{
            // Handle when your app resumes
            var pg = MainPage as MainPage;
            if (LastDetail is null)
                throw new InvalidOperationException("Before resume is sleep");
            pg.Detail = LastDetail;
        }
    }
}
