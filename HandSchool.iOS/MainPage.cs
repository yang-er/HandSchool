using HandSchool.iOS;
using HandSchool.ViewModels;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class MainPage : TabbedPage
    {
        private static MainPage Instance;

        public const string SelectPageSignal = "HandSchool.iOS.SelectPageFinal";
        
        public MainPage()
        {
            Instance = this;

            if (!Core.Initialized)
            {
                Children.Add(new SelectTypePage() { Title = "选择学校", Icon = "tab_feed.png" });
            }
            else
            {
                NavigationViewModel.Instance.ToString();
                PlatformImpl.Instance.NavigationMenu.ForEach(item => Children.Add(item.Page));
            }
        }

        public void FinishSettings()
        {
            NavigationViewModel.Instance.ToString();
            PlatformImpl.Instance.NavigationMenu.ForEach(item => Children.Add(item.Page));
            Children.RemoveAt(0);
            MessagingCenter.Send(this, SelectPageSignal, false);
        }
    }
}
