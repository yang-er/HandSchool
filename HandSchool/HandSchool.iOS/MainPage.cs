using HandSchool.ViewModels;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class MainPage : TabbedPage
    {
        public MainPage()
        {
            NavigationViewModel.Instance.PrimaryItems.ForEach((obj) => Children.Add(obj.DestPage));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (App.Current.Service.NeedLogin && !App.Current.Service.IsLogin)
            {
                LoginViewModel.RequestAsync(App.Current.Service);
            }
        }
    }
}