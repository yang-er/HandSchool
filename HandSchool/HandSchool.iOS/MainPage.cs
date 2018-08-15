using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.Platform;
using IOS = Xamarin.Forms.PlatformConfiguration.iOS;
using PF = Xamarin.Forms.Platform.iOS.Platform;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace HandSchool.Views
{
    public class MainPage : TabbedPage
    {
        bool _isselpage = false;
        internal bool IsSelectPage
        {
            get => _isselpage;
            set
            {
                if (_isselpage == value)
                    return;
                _isselpage = value;
                OnPropertyChanged("IsSelectPage");
            }
        }

        public MainPage()
        {
            if (Core.App.Service is null)
            {
                Children.Add(new SelectTypePage() { Title = "选择学校", Icon = "tab_feed.png" });
                IsSelectPage = true;
            }
            else
            {
                NavigationViewModel.Instance.AppleItems.ForEach((obj) => Children.Add(obj.DestPage));
            }
        }

        public void FinishSettings()
        {
            NavigationViewModel.Instance.AppleItems.ForEach((obj) => Children.Add(obj.DestPage));
            Children.RemoveAt(0);
            IsSelectPage = false;
        }
    }
}