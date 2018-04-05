using HandSchool.Internal;
using HandSchool.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : PopContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = LoginViewModel.Instance;
            LoginViewModel.Instance.StateChanged += Response;
        }

        async void Response(object sender, LoginStateEventArgs e)
        {
            switch (e.State)
            {
                case LoginState.Processing:
                    await DisplayAlert("正在登录", "正在登录中，请稍后……", "知道了");
                    break;
                case LoginState.Succeeded:
                    await Close();
                    break;
                case LoginState.Failed:
                    await DisplayAlert("登录失败", $"登录失败，{App.Current.Service.InnerError}。", "知道了");
                    break;
            }
        }
    }
}
