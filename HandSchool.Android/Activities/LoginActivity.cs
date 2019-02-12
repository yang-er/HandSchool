using Android.App;
using Android.OS;

namespace HandSchool.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    [BindView(Resource.Layout.activity_login)]
    public class LoginActivity : BaseActivity
    {
        private LoginFragment Fragment { get; set; }

        protected override void OnNavigatedParameter(object obj)
        {
            Fragment = obj as LoginFragment;
            TransactionV3(Fragment, Fragment);

            var ActionBar = SupportActionBar;
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);
        }
        
        public override void Finish()
        {
            base.Finish();
            Fragment.Completed?.Start();
        }
    }
}