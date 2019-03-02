using Android.App;
using Android.Content;
using Android.OS;
using HandSchool.Views;
using System;
using System.Threading.Tasks;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using MessagingCenter = Xamarin.Forms.MessagingCenter;

namespace HandSchool.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    [BindView(Resource.Layout.activity_selecttype)]
    public class SelectTypeActivity : BaseActivity
    {
        protected bool firstTime = true;

        protected override void SetTransactionArguments(FragmentTransaction fragmentTransaction)
        {
            if (firstTime)
            {
                firstTime = false;
            }
            else
            {
                fragmentTransaction.SetCustomAnimations(
                    Resource.Animation.slide_right_in,
                    Resource.Animation.slide_left_out,
                    Resource.Animation.slide_left_in,
                    Resource.Animation.slide_right_out);
            }
        }

        public override Task PushAsync(Type pageType, object param)
        {
            pageType = Core.Reflection.TryGetType(pageType);

            if (typeof(ViewObject).IsAssignableFrom(pageType))
            {
                var vo = Core.Reflection.CreateInstance<ViewObject>(pageType);
                vo.SetNavigationArguments(param);
                Transaction(vo);
                return Task.CompletedTask;
            }
            else
            {
                return base.PushAsync(pageType, param);
            }
        }
        
        private void OnSelectTypeFinished(WelcomePage wp)
        {
            RunOnUiThread(() =>
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            base.OnCreate(savedInstanceState);
            PlatformImplV2.Register(this);
            if (!(savedInstanceState?.ContainsKey("android:viewHierarchyState") ?? false))
                Transaction(new SelectTypePage());
            MessagingCenter.Subscribe<WelcomePage>(this, WelcomePage.FinishSignal, OnSelectTypeFinished);
        }

        protected override void OnDestroy()
        {
            MessagingCenter.Unsubscribe<WelcomePage>(this, WelcomePage.FinishSignal);
            base.OnDestroy();
        }
    }
}