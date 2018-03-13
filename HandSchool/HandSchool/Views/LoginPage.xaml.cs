using HandSchool.Internal;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public ISchoolSystem Service => App.Current.Service;

        public LoginPage()
        {
            InitializeComponent();
            AutoLogin.On = Helper.AutoLogin;
            AutoLogin.OnChanged += AutoLoginChanged;
            SavePassword.On = Helper.SavePassword;
            SavePassword.OnChanged += SavePassChanged;
            BindingContext = this;
        }
        
        public void Login_Clicked(object sender, EventArgs e)
        {
            App.Current.Service.Login();

            if (App.Current.Service.IsLogin)
            {
                App.Current.MainPage.Navigation.PopModalAsync();
            }
            else
            {
                DisplayAlert("登录失败", "登录失败，请检查您的用户名和密码或网络状态。", "知道了");
            }
        }

        private void SavePassChanged(object sender, ToggledEventArgs e)
        {
            Helper.SavePassword = e.Value;
            AutoLogin.On = Helper.AutoLogin;
        }

        private void AutoLoginChanged(object sender, ToggledEventArgs e)
        {
            Helper.AutoLogin = e.Value;
            SavePassword.On = Helper.SavePassword;
        }
    }
}
