using System.Threading.Tasks;
using HandSchool.Internal;
using HandSchool.Pages;
using Xamarin.Forms;

namespace HandSchool.iOS.Internals
{
    public class WebLoginPageImpl: WebLoginPage
    {
        public WebLoginPageImpl()
        {
            SetValue(PlatformExtensions.ShowLeftCancelProperty, true);
        }
        public override Task CloseAsync()
        {
            return Xamarin.Forms.Application.Current.MainPage.Navigation.PopModalAsync();
        }

        public override Task ShowAsync()
        {
            return Xamarin.Forms.Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(this));
        }
    }
}