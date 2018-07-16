using System;
using Xamarin.Forms;

namespace HandSchool.Internal
{
    [Obsolete("Use Helper.ShowLoadingAlert before we make up one new")]
    public class LoadingBehavior : Behavior<Page>
    {
        public string Title { get; set; }
        public string Tips { get; set; }

#if __ANDROID__
        Android.App.ProgressDialog renderObj;
#elif __IOS__
        
#elif __UWP__
        
#endif

        public LoadingBehavior(string tips, string title = "提示")
        {
            Title = title;
            Tips = tips;
        }

        protected override void OnAttachedTo(Page bindable)
        {
            base.OnAttachedTo(bindable);
#if __ANDROID__
            renderObj = new Android.App.ProgressDialog(HandSchool.Droid.MainActivity.ActivityContext);
            renderObj.SetTitle(Title);
            renderObj.SetMessage(Tips);
            renderObj.Indeterminate = true;
            renderObj.Show();
#elif __IOS__
            
#elif __UWP__
            bindable.IsBusy = true;
#endif
        }

        protected override void OnDetachingFrom(Page bindable)
        {
            base.OnDetachingFrom(bindable);
#if __ANDROID__
            renderObj.Dismiss();
#elif __IOS__
            
#elif __UWP__
            bindable.IsBusy = false;
#endif
        }
    }
}
