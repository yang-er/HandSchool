using System;
using HandSchool.Views;

namespace HandSchool.iOS.Pages
{
    public class WelcomeIOSPage:WelcomePage
    {
        public WelcomeIOSPage(){}
        protected override void enter_main_clicked(object s, EventArgs e)
        {
            Xamarin.Forms.Application.Current.MainPage.Navigation.PopModalAsync();
            Xamarin.Forms.Application.Current.MainPage = new MainPage();
        }
    }
}
