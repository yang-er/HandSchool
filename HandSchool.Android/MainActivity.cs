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
    public class MainActivity : BaseActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        public static MainActivity Instance;

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            throw new System.NotImplementedException();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }
        
        protected override void OnCreate(Bundle bundle)
        {
            ContentViewResource = Resource.Layout.activity_main;
            base.OnCreate(bundle);
            XForms.Init(this, bundle);
            new PlatformImpl(this);
            Forwarder.NormalWay.Begin();
            Core.Configure.Write("hs.school.bin", "jlu");
            Core.Initialize();

            Transaction(new Views.IndexPage().CreateSupportFragment(this));
            
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, Toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            var fib = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fib.Click += FabOnClick;

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);
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
