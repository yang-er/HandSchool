using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.Platform;
using IOS = Xamarin.Forms.PlatformConfiguration.iOS;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace HandSchool.Views
{
    public class MainPage : TabbedPage
    {
        public MainPage()
        {
            if (Core.App.Service is null)
            {
                Children.Add(new SelectTypePage() { Title = "选择学校", Icon = "tab_feed.png" });
            }
            else
            {
                NavigationViewModel.Instance.AppleItems.ForEach((obj) => Children.Add(obj.DestPage));
            }

            var sp = this.CreateViewController();
            sp.HidesBottomBarWhenPushed = true;
        }
        
        public void FinishSettings()
        {
            NavigationViewModel.Instance.AppleItems.ForEach((obj) => Children.Add(obj.DestPage));
            Children.RemoveAt(0);
        }
    }
}