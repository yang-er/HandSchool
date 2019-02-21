using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HandSchool.Views;

namespace HandSchool.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    [BindView(0)]
    public class SelectTypeActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            base.OnCreate(savedInstanceState);
            PlatformImplV2.Register(this);
            var page = new SelectTypePage();
            page.SchoolSelected += () => { /*???*/ };
            Transaction(new SelectTypePage());
            // 
            // 记得设置一下上面bindview
            // Create your application here
        }
    }
}