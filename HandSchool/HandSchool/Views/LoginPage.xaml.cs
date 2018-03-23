using HandSchool.Internal;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : PopContentPage
    {
        public ISchoolSystem Service => App.Current.Service;

        bool IsBusyLogin = false;

        public LoginPage()
        {
            InitializeComponent();
            AutoLogin.On = Helper.AutoLogin;
            AutoLogin.OnChanged += AutoLoginChanged;
            SavePassword.On = Helper.SavePassword;
            SavePassword.OnChanged += SavePassChanged;
            LoginButton.Command = new Command(async () => await Login_Clicked());
            BindingContext = this;
#if __ANDROID__
            loadingBar.HeightRequest = 16;
#endif
        }
        
        async Task Login_Clicked()
        {
            if (IsBusyLogin)
            {
                await DisplayAlert("正在登录", "正在登录中，请稍后……", "知道了");
                return;
            }

            IsBusyLogin = true;
            loadingBar.IsRunning = true;
            await App.Current.Service.Login();
            IsBusyLogin = false;
            loadingBar.IsRunning = false;

            if (App.Current.Service.IsLogin)
            {
                await Close();
            }
            else
            {
                await DisplayAlert("登录失败", $"登录失败，{App.Current.Service.InnerError}。", "知道了");
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
