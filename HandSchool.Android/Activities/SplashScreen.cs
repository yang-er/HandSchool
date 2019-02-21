using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace HandSchool.Droid
{
    [Activity(Label = "掌上校园", Icon = "@drawable/icon", MainLauncher = true,
              NoHistory = true, Theme = "@style/Theme.Splash",
              ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Forms.Forms.Init(this, bundle);
            PlatformImplV2.Register(this);
            Forwarder.NormalWay.Begin();
            var next = Core.Initialize() ? typeof(MainActivity) : typeof(SelectTypeActivity);
            StartActivity(new Intent(this, next));
            Finish();
        }
    }
}