using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private bool auto_login = true;
        private bool save_password = true;
        private static LoginViewModel instance = null;
        public Command LoginCommand { get; set; }
        public event EventHandler<LoginStateEventArgs> StateChanged;

        public static LoginViewModel Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = new LoginViewModel();
                    instance.LoginCommand = new Command(instance.Login);
                }
                return instance;
            }
        }

        public bool AutoLogin
        {
            get => auto_login;
            set
            {
                SetProperty(ref auto_login, value);
                if (value) SetProperty(ref save_password, true, "SavePassword");
            }
        }

        public bool SavePassword
        {
            get => save_password;
            set
            {
                SetProperty(ref save_password, value);
                if (!value) SetProperty(ref auto_login, true, "AutoLogin");
            }
        }

        public string Username
        {
            get => App.Current.Service.Username;
            set => App.Current.Service.Username = value;
        }

        public string Password
        {
            get => App.Current.Service.Password;
            set => App.Current.Service.Password = value;
        }

        public string Tips => App.Current.Service.Tips;

#warning 未实现, Tips: Behavior
        async void Login()
        {
            if (IsBusy)
            {
                StateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Processing));
                return;
            }

            IsBusy = true;
            var loadingAlert = Internal.Helper.ShowLoadingAlert("正在登录……");

            try
            {
                await App.Current.Service.Login();
            }
            catch (Exception ex)
            {
                StateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, ex.Message));
            }
            finally
            {
                IsBusy = false;
                loadingAlert.Invoke();
            }
            
            if (App.Current.Service.IsLogin)
            {
                StateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Succeeded));
            }
            else
            {
                StateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, App.Current.Service.InnerError));
            }
        }
    }

    public class LoginStateEventArgs : EventArgs
    {
        public LoginState State { get; set; }
        public string InnerError { get; set; }

        public LoginStateEventArgs(LoginState state, string error = "")
        {
            State = state;
            InnerError = error;
        }
    }

    public enum LoginState
    {
        Processing,
        Succeeded,
        Failed
    }
}
