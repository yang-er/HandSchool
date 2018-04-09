using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;

namespace HandSchool.Droid
{
    [Activity(Label = "掌上校园", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static Context ActivityContext;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Internal.Helper.DataBaseDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            Internal.Helper.SegoeMDL2 = "segmdl2.ttf#Segoe MDL2 Assets";
            Internal.Helper.AndroidContext = this;
            ActivityContext = this;
            LoadApplication(new App() {});
        }
    }
}
