using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Activity = Xamarin.Forms.Platform.Android.FormsAppCompatActivity;
using XForms = Xamarin.Forms.Forms;

namespace HandSchool.Droid
{
    [Activity(Label = "掌上校园", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Activity
    {
        public static Context ActivityContext;
        public static MainActivity Instance;
        public static UpdateManager UpdateManager;

        protected override void OnCreate(Bundle bundle)
        {
            Instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            base.OnCreate(bundle);

            XForms.Init(this, bundle);
            ActivityContext = this;
            UpdateManager = new UpdateManager(this);
            UpdateManager.Update();
            // Intent it = new Intent(this, typeof(SampleService));
            // StartService(it);
            LoadApplication(new App() {});
        }
    }
}
