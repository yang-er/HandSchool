using HandSchool.Internal;
using HandSchool.iOS;
using HandSchool.ViewModels;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class MainPage : TabbedPage
    {
        public MainPage()
        {
            Title = "掌上校园";
            NavigationViewModel.Instance.ToString();
            PlatformImpl.Instance.NavigationMenu.ForEach(item => Children.Add(item.GetNavigationPage()));
        }
    }
}