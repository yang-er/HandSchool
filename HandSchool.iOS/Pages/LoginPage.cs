using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class LoginPage : BaseLoginPage
    {
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