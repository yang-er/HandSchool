using Android.App;
using Android.OS;

namespace HandSchool.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    public class LoginActivity : BaseActivity
    {
        private LoginFragment Fragment { get; set; }

        protected override void OnNavigatedParameter(object obj)
        {
            Fragment = obj as LoginFragment;
            TransactionV3(Fragment, Fragment);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            ContentViewResource = Resource.Layout.activity_login;
            base.OnCreate(savedInstanceState);
            var bar = SupportActionBar;
            bar.SetDisplayHomeAsUpEnabled(true);
            bar.SetHomeButtonEnabled(true);
        }

        public override void Finish()
        {
            base.Finish();
            Fragment.Completed?.Start();
        }
    }
}