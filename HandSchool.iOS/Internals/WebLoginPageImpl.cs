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

        public override async Task CloseAsync()
        {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PopModalAsync();
            await base.CloseAsync();
        }

        public override Task ShowAsync()
        {
            return Xamarin.Forms.Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(this));
        }
    }
}