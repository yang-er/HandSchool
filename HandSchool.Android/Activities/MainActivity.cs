using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;

using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using HandSchool.Droid.Internals;
using HandSchool.Internals;
using System;
using System.IO;


namespace HandSchool.Droid
{
    [Activity(Label = "掌上校园", Icon = "@drawable/icon", Theme = "@style/AppTheme.NoActionBar",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [BindView(Resource.Layout.activity_main)]
    public class MainActivity : BaseActivity
    {
        [BindView(Resource.Id.nav_view)]
        public NavigationView NavigationView { get; set; }

        [BindView(Resource.Id.drawer_layout)]
        public DrawerLayout DrawerLayout { get; set; }
        
        int lastItemId = 0;

        public bool NavigationItemSelected(NavMenuItemV2 menuItem, IMenuItem menuItem2)
        {
            try
            {
                if (lastItemId == menuItem2.Order) return false;
                NavigationView.Menu.GetItem(lastItemId).SetChecked(false);
                menuItem2.SetChecked(true);
                lastItemId = menuItem2.Order;

                TransactionV3(menuItem.FragmentV3.Item1, menuItem.FragmentV3.Item2);
                return true;
            }
            finally
            {
                System.Threading.Tasks.Task.Delay(100).ContinueWith(t => 
                {
                    RunOnUiThread(() => DrawerLayout.CloseDrawer(GravityCompat.Start));
                });
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            Xamarin.Forms.Forms.Init(this, bundle);
            base.OnCreate(bundle);
            PlatformImplV2.Register(this);
            var toggle = new ActionBarDrawerToggle(this, DrawerLayout, Toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            DrawerLayout.AddDrawerListener(toggle);
            toggle.SyncState();

            // get the navigation menu
            var listHandler = new NavMenuListHandler();
            listHandler.NavigationItemSelected += NavigationItemSelected;
            listHandler.InflateMenus(NavigationView.Menu);
            NavigationView.SetNavigationItemSelectedListener(listHandler);
            NavigationView.Menu.GetItem(0).SetChecked(true);

            var transactionArgs = listHandler.MenuItems[0][0].FragmentV3;
            
            TransactionV3(transactionArgs.Item1, transactionArgs.Item2);

            
            NavHeadViewHolder.Instance.SolveView(NavigationView.GetHeaderView(0));

            var x = new AndroidWebDialogAdditionalArgs { WebChromeClient = new CancelLostWebChromeClient(this) };
            x.WebViewClient = new CancelLostWebClient((CancelLostWebChromeClient)x.WebChromeClient);
            x.Cookies = new System.Collections.Generic.List<(string, System.Net.Cookie)>();
            JLU.Loader.CancelLostWebAdditionalArgs = x;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            NavHeadViewHolder.Instance.CleanBind();
            this.CleanBind();
        }

        static void RmDir(string path)
        {
            if (!Directory.Exists(path)) return;
            var dirs = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            foreach (var item in files)
            {
                try
                {
                    File.Delete(item);
                }
                catch { }
            }
            foreach (var item in dirs)
            {
                RmDir(item);
            }
            try
            {
                Directory.Delete(path);
            }
            catch { }
        }
        public static void ClearWebViewCache()
        {
            var path = Directory.GetParent(Core.Configure.Directory).FullName;
            RmDir(path + "/app_textures");
            RmDir(path + "/app_webview");
            RmDir(path + "/shared_prefs");
        }

        BackHandler backHandler = new BackHandler();

        public override void OnBackPressed()
        {
            if (DrawerLayout.IsDrawerOpen(GravityCompat.Start))
            {
                DrawerLayout.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                if (backHandler.IsDoubleBack())
                {
                    ClearWebViewCache();
                    base.OnBackPressed();
                }
                else
                {
                    Toast.MakeText(this, "再按一次退出", ToastLength.Short).Show();
                }
            }
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (s) => this.PushAsync<DemoFragment>()).Show();
        }
    }
    class BackHandler
    {
        DateTime? lastBack = null;
        public bool IsDoubleBack()
        {
            bool res = false;
            var now = DateTime.Now;
            TimeSpan? timeSpan = null;
            if (lastBack != null)
            {
                timeSpan = now - lastBack.Value;
            }
            if (lastBack != null && timeSpan.Value.TotalMilliseconds > 0 && timeSpan.Value.TotalMilliseconds < 1000)
            {
                res = true;
            }
            lastBack = now;
            return res;
        }
    }
}
