using HandSchool.ViewModels;
using Xamarin.Forms;
using UIViewController = UIKit.UIViewController;
using PlatformAPI = Xamarin.Forms.Platform.iOS.Platform;

namespace HandSchool.Views
{
    public class MainPage : TabbedPage
    {
        private static MainPage Instance;

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
            Instance = this;

            if (!Core.Initialized)
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
