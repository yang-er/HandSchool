using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using System;
using Xamarin.Forms.Platform.Android;
using XForms = Xamarin.Forms.Forms;

namespace HandSchool.Droid
{
    [Activity(Label = "掌上校园", Icon = "@drawable/icon", Theme = "@style/AppTheme.NoActionBar",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : BaseActivity
    {
        public static MainActivity Instance;

        internal NavigationView NavigationView { get; private set; }
        internal DrawerLayout DrawerLayout { get; private set; }
        int lastItemId = 0;

        public bool NavigationItemSelected(NavMenuItemV2 menuItem, IMenuItem menuItem2)
        {
            NavigationView.Menu.GetItem(lastItemId).SetChecked(false);
            menuItem2.SetChecked(true);
            lastItemId = menuItem2.ItemId;
            
            if (menuItem.IsFragment)
            {
                Transaction(menuItem.CreateFragment());
            }
            else if (menuItem.IsTabbedPage)
            {
                return false;
            }
            else
            {
                Transaction(menuItem.CreateObject());
            }

            return false;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(6, 6, 6, "666");
            return true;
        }
        
        protected override void OnCreate(Bundle bundle)
        {
            ContentViewResource = Resource.Layout.activity_main;
            XForms.Init(this, bundle);
            new PlatformImplV2(this);
            base.OnCreate(bundle);
            PlatformImplV2.Instance.UpdateManager.Update();
            Forwarder.NormalWay.Begin();
            Core.Configure.Write("hs.school.bin", "jlu");
            Core.Initialize();

            Transaction(new Views.IndexPage().CreateSupportFragment(this));

            DrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, DrawerLayout, Toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            DrawerLayout.AddDrawerListener(toggle);
            toggle.SyncState();

            var fib = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fib.Click += FabOnClick;

            NavigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            var listHandler = new NavMenuListHandler();
            listHandler.NavigationItemSelected += NavigationItemSelected;
            listHandler.InflateMenus(NavigationView.Menu);
            NavigationView.SetNavigationItemSelectedListener(listHandler);
        }

        public override void OnBackPressed()
        {
            var drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
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
                .SetAction("Action", (s) => StartActivity(new Android.Content.Intent(this, typeof(SecondActivity)))).Show();
        }
    }
}
