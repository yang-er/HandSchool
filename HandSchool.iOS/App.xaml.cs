using HandSchool.Views;
using Xamarin.Forms;
using XApplication = Xamarin.Forms.Application;

namespace HandSchool.iOS
{
    public partial class App : XApplication
    {
        public static new App Current
        {
            get => XApplication.Current as App;
        }

        public App()
        {
            PlatformImpl.Register();
            Forwarder.NormalWay.Begin();
            InitializeComponent();
            Core.Initialize();

            if (Core.Initialized)
                SetMainPage<MainPage>();
            else
                SetMainPage<SelectTypePage>();
        }

        public void SetMainPage<T>()
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