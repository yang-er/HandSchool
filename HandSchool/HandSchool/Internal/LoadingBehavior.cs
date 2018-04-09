using Xamarin.Forms;

namespace HandSchool.Internal
{
    public class LoadingBehavior : Behavior<Page>
    {
        public string Title { get; set; }
        public string Tips { get; set; }
        object renderObj;

        public LoadingBehavior(string tips, string title = "提示")
        {
            Title = title;
            Tips = tips;
        }

        protected override void OnAttachedTo(Page bindable)
        {
            base.OnAttachedTo(bindable);
#if __ANDROID__
            var dialog = new Android.App.ProgressDialog(HandSchool.Droid.MainActivity.ActivityContext);
            dialog.SetTitle(Title);
            dialog.SetMessage(Tips);
            dialog.Indeterminate = true;
            dialog.Show();
            renderObj = dialog;
#elif __IOS__
            
#elif __UWP__
            
#endif
        }

        protected override void OnDetachingFrom(Page bindable)
        {
            base.OnDetachingFrom(bindable);
#if __ANDROID__
            (renderObj as Android.App.ProgressDialog).Dismiss();
#elif __IOS__
            
#elif __UWP__
            
#endif
        }
    }
}
