using Android.App;
using Android.OS;

namespace HandSchool.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    public class SecondActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ContentViewResource = Resource.Layout.activity_popup;
            base.OnCreate(savedInstanceState);
            
            Transaction(new IndexFragment());

            var bar = SupportActionBar;
            bar.SetDisplayHomeAsUpEnabled(true);
            bar.SetHomeButtonEnabled(true);
        }
    }
}