using HandSchool.iOS;
using HandSchool.ViewModels;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class MainPage : TabbedPage
    {
        private static MainPage Instance;
        
        public MainPage()
        {
            Instance = this;
            Title = "掌上校园";
            NavigationViewModel.Instance.ToString();
            PlatformImpl.Instance.NavigationMenu.ForEach(item => Children.Add(item.Page));
        }
    }
}