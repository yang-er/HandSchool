using HandSchool.Internal;
using HandSchool.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

#if __UWP__
using LoginPage = HandSchool.Views.LoginDialog;
#else
using LoginPage = HandSchool.Views.LoginPage;
#endif

namespace HandSchool.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; set; }
        public ILoginField Form { get; }
        public LoginPage Page { get; set; }
        
        private LoginViewModel(ILoginField form)
        {
            Form = form;
            Title = "登录" + form.FormName;
        }
        
        public static Task<bool> RequestAsync(ILoginField form)
        {
            return Core.EnsureOnMainThread(async () =>
            {
                var viewModel = new LoginViewModel(form);
                viewModel.LoginCommand = new Command(viewModel.Login);
                viewModel.Page = new LoginPage(viewModel);
                await viewModel.Page.ShowAsync();
                return form.IsLogin;
            });
        }
        
        async void Login()
        {
            if (IsBusy)
            {
                Page.Response(this, new LoginStateEventArgs(LoginState.Processing));
                return;
            }

            SetIsBusy(true, "正在登录……");
            Form.LoginStateChanged += Page.Response;

            try
            {
                await Form.Login();
            }
            finally
            {
                SetIsBusy(false);
                Form.LoginStateChanged -= Page.Response;
            }
        }
    }
}
