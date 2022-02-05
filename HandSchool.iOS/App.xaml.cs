using System;
using System.Threading.Tasks;
using HandSchool.iOS.Pages;
using HandSchool.ViewModels;
using HandSchool.Views;
using WebKit;
using Xamarin.Forms;
using XApp = Xamarin.Forms.Application;

namespace HandSchool.iOS
{
    public partial class App : XApp
    {
        public new static App Current => XApp.Current as App;
        
        public App()
        {
            PlatformImpl.Register();
            Forwarder.NormalWay.Begin();
            InitializeComponent();
            Core.Initialize();
            SettingViewModel.OnResetSettings += DeleteWKWebViewCookies;
            if (Core.Initialized)
            {
                SetMainPage<MainPage>();
            }
            else
            {
                //替换掉以前的WelcomePage;
                Core.Reflection.RegisterType<WelcomePage, WelcomeIOSPage>();
                MainPage = new NavigationPage(new SelectTypePage());
            }
        }

        private static async Task DeleteWKWebViewCookies()
        {
            var dataStore = WKWebsiteDataStore.DefaultDataStore;
            var types = await dataStore.FetchDataRecordsOfTypesAsync(WKWebsiteDataStore.AllWebsiteDataTypes);
            var i = new nuint(0);
            while (i < types.Count)
            {
                var item = types.GetItem<WKWebsiteDataRecord>(i);
                await dataStore.RemoveDataOfTypesAsync(item.DataTypes, new []{item});
                i++;
            }
        }

        private void SetMainPage<T>()
            where T : Page, new()
        {
            MainPage = new T();
        }

        protected override void OnStart()
		{
            // Handle when your app starts
        }
        
		protected override void OnSleep()
		{
            // Handle when your app sleeps
        }

		protected override void OnResume()
		{
            // Handle when your app resumes
        }
    }
}