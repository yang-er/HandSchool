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
            AutoLogin.On = App.Service.AutoLogin;
            AutoLogin.OnChanged += AutoLoginChanged;
            SavePassword.On = App.Service.SavePassword;
            SavePassword.OnChanged += SavePassChanged;
            BindingContext = this;
        }
        
        public void Login_Clicked(object sender, EventArgs e)
        {
            Service.Login();

            if (Service.IsLogin)
            {
                (Application.Current as App).MainPage.Navigation.PopModalAsync();
            }
            else
            {
                DisplayAlert("登录失败", "登录失败，请检查您的用户名和密码或网络状态。", "知道了");
            }
        }

        private void SavePassChanged(object sender, ToggledEventArgs e)
        {
            App.Service.SavePassword = e.Value;
            AutoLogin.On = App.Service.AutoLogin;
        }

        private void AutoLoginChanged(object sender, ToggledEventArgs e)
        {
            App.Service.AutoLogin = e.Value;
            SavePassword.On = App.Service.SavePassword;
        }
    }
}
