using HandSchool.Internal;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public ISchoolSystem Service => App.Service;

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = this;
        }
        
        async void Login_Clicked(object sender, EventArgs e)
        {
            Service.Login();
            await Navigation.PopModalAsync();
        }
    }
}