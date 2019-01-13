using HandSchool.Views;
using System;
using Xamarin.Forms;
using Adjust = Xamarin.Forms.PlatformConfiguration.AndroidSpecific.WindowSoftInputModeAdjust;
using Ext = Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Application;

namespace HandSchool
{
    public partial class App : Application
    {
        public App()
        {
            Core.Reflection.ForceLoad(false);
            Core.Initialize();
            InitializeComponent();
            MainPage = new MainPage();
            Ext.SetWindowSoftInputModeAdjust(this, Adjust.Pan);
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
