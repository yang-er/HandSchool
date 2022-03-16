using Android.App;
using Android.Content.PM;
using Android.OS;

using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using HandSchool.Droid.Internals;
using HandSchool.Internal;
using HandSchool.ViewModels;
using System.Threading.Tasks;
using Android.Webkit;
using HandSchool.Services;
using Google.Android.Material.Navigation;

namespace HandSchool.Droid
{
    [Activity(Label = "掌上校园", Icon = "@mipmap/ic_launcher",
        RoundIcon = "@mipmap/ic_launcher_round", Theme = "@style/AppTheme.NoActionBar",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [BindView(Resource.Layout.activity_main)]
    public class MainActivity : BaseActivity
    {
        [BindView(Resource.Id.nav_view)] 
        public NavigationView NavigationView { get; set; }

        [BindView(Resource.Id.drawer_layout)] 
        public DrawerLayout DrawerLayout { get; set; }

        int _lastItemId;

        public bool NavigationItemSelected(NavMenuItemV2 menuItem, IMenuItem menuItem2)
        {
            try
            {
                if (_lastItemId == menuItem2.Order) return false;
                NavigationView.Menu.GetItem(_lastItemId)?.SetChecked(false);
                menuItem2.SetChecked(true);
                _lastItemId = menuItem2.Order;
                TransactionV3(menuItem.FragmentV3.Item1, menuItem.FragmentV3.Item2);
                return true;
            }
            finally
            {
                Task.Delay(100).ContinueWith(t =>
                {
                    RunOnUiThread(() => DrawerLayout.CloseDrawer(GravityCompat.Start));
                });
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            Xamarin.Forms.Forms.Init(this, bundle);
            base.OnCreate(bundle);
            if (Weather.IsMi())
            {
                Core.Reflection.RegisterImplement<IWeatherReport, MiWeatherReport>();
            }
            PlatformImplV2.Register(this);
            var toggle = new ActionBarDrawerToggle(this, DrawerLayout, Toolbar, Resource.String.navigation_drawer_open,
                Resource.String.navigation_drawer_close);
            DrawerLayout.AddDrawerListener(toggle);
            toggle.SyncState();

            // get the navigation menu
            var listHandler = new NavMenuListHandler();
            listHandler.NavigationItemSelected += NavigationItemSelected;
            listHandler.InflateMenus(NavigationView.Menu);
            NavigationView.SetNavigationItemSelectedListener(listHandler);
            NavigationView.Menu.GetItem(0)?.SetChecked(true);

            var transactionArgs = listHandler.MenuItems[0][0].FragmentV3;
            TransactionV3(transactionArgs.Item1, transactionArgs.Item2);
            NavHeadViewHolder.Instance.SolveView(NavigationView.GetHeaderView(0));
            
            var x = new AndroidWebDialogAdditionalArgs {WebChromeClient = new CancelLostWebChromeClient(this)};
            x.WebViewClient = new CancelLostWebClient((CancelLostWebChromeClient) x.WebChromeClient);
            JLU.Loader.CancelLostWebAdditionalArgs = x;
            SettingViewModel.OnResetSettings += () =>
            {
                CookieManager.Instance?.RemoveAllCookies(new ObjectRes());
                return Task.CompletedTask;
            };
            _backHandler.Refresh();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            NavHeadViewHolder.Instance.CleanBind();
            this.CleanBind();
        }

        private readonly TimeoutManager _backHandler = new TimeoutManager(1.3);

        public override void OnBackPressed()
        {
            if (DrawerLayout.IsDrawerOpen(GravityCompat.Start))
            {
                DrawerLayout.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                if (_backHandler.IsTimeout())
                {
                    Toast.MakeText(this, "再按一次退出", ToastLength.Short).Show();
                }
                else
                {
                    base.OnBackPressed();
                }

                _backHandler.Refresh();
            }
        }
    }
}