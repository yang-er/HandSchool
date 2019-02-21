using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using HandSchool.Internals;
using HandSchool.Views;
using System;

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

        [BindView(Resource.Id.fab)]
        public FloatingActionButton FloatingButton { get; set; }

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
                    Core.Platform.EnsureOnMainThread(() => DrawerLayout.CloseDrawer(GravityCompat.Start));
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

            FloatingButton.Click += FabOnClick;

            // get the navigation menu
            var listHandler = new NavMenuListHandler();
            listHandler.NavigationItemSelected += NavigationItemSelected;
            listHandler.InflateMenus(NavigationView.Menu);
            NavigationView.SetNavigationItemSelectedListener(listHandler);
            NavigationView.Menu.GetItem(0).SetChecked(true);

            var transactionArgs = listHandler.MenuItems[0][0].FragmentV3;
            TransactionV3(transactionArgs.Item1, transactionArgs.Item2);

            NavHeadViewHolder.Instance.SolveView(NavigationView.GetHeaderView(0));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            NavHeadViewHolder.Instance.CleanBind();
            this.CleanBind();
        }

        public override void OnBackPressed()
        {
            if (DrawerLayout.IsDrawerOpen(GravityCompat.Start))
            {
                DrawerLayout.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (s) => this.PushAsync<DemoFragment>()).Show();
        }
    }
}
