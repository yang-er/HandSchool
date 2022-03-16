using System.Threading.Tasks;
using HandSchool.Internal;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class LoginPage : BaseLoginPage
    {
        public LoginPage()
        {
            On<_iOS_>().ShowLeftCancel();
        }

        public override Task ShowAsync()
        {
            return Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(this));
        }

        protected override Task CloseAsync()
        {
            return Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}