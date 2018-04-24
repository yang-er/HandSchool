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
            Core.Initialize();
            MainPage = NavigationViewModel.GetMainPage();
        }
        
        protected override void OnStart()
		{
            // Handle when your app starts
        }

		protected override void OnSleep()
		{
            // Handle when your app sleeps
#if !_UWP_
            (MainPage as MainPage).Detail = new ContentPage();
#endif
        }

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
    }
}
