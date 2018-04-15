using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System;

namespace HandSchool.Internal
{
    public class CheckBehavior
    {
        public string Title { get; set; }
        public string Tips { get; set; }

#if __ANDROID__
        Android.App.ProgressDialog renderObj;
#elif __IOS__
            
#elif __UWP__

#endif

        public CheckBehavior(string tips, string title = "确认")
        {
            Title = title;
            Tips = tips;
        }
#if __UWP__
        public async System.Threading.Tasks.Task CheckAsync()
        {
            var dialog = new ContentDialog()
            {
                Title = this.Title,
                Content = this.Tips,
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消",
                FullSizeDesired = false
            };
            dialog.PrimaryButtonClick += (_s, _e) => { Core.App.Confrimed = true; };

            var a=await dialog.ShowAsync();
            return;
        }


#endif
    }
}