using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace HandSchool.Droid
{
    [Activity]
    public class SecondActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_popup);
            
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frameLayout1, new IndexFragment()).Commit();

            var bar = SupportActionBar;
            bar.SetDisplayHomeAsUpEnabled(true);
            bar.SetHomeButtonEnabled(true);

            // Create your application here
        }
    }
}