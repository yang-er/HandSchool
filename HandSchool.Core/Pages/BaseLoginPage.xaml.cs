using System;
using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.ViewModels;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public abstract partial class BaseLoginPage : ViewObject, ILoginPage
    {
        private MemoryStream _imageMem;
        private readonly TaskCompletionSource<bool> _finished;

        protected BaseLoginPage()
        {
            InitializeComponent();
            _finished = new TaskCompletionSource<bool>();
            LoginButton.Command = new CommandAction(OnLoginRequested);
            CaptchaImage.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new CommandAction(UpdateCaptchaInformation)
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UsernameBox.Text = LoginViewModel?.Form?.Username ?? "";
            PasswordBox.Text = LoginViewModel?.Form?.Password ?? "";
        }

        public LoginViewModel LoginViewModel
        {
            get => BindingContext as LoginViewModel;
            set => ViewModel = value;
        }
        
        private void OnLoginRequested()
        {
            LoginViewModel.Form.Username = UsernameBox.Text ?? "";
            LoginViewModel.Form.Password = PasswordBox.Text ?? "";
            LoginViewModel.Form.CaptchaCode = CaptchaBox.Text ?? "";
            LoginViewModel.LoginCommand.Execute(null);
        }

        public async void OnLoginStateChanged(object sender, LoginStateEventArgs e)
        {
            switch (e.State)
            {
                case LoginState.Processing:
                    await RequestMessageAsync("正在登录", "正在登录中，请稍候……", "知道了");
                    break;
                case LoginState.Succeeded:
                    await CloseAsync();
                    break;
                case LoginState.Failed:
                    await RequestMessageAsync("登录失败", $"登录失败，{e.InnerError}。", "知道了");
                    UpdateCaptchaInformation();
                    break;
            }
        }

        public abstract Task ShowAsync();

        protected abstract Task CloseAsync();
        
        public Task LoginAsync()
        {
            return _finished.Task;
        }

        protected override void OnDisappearing()
        {
            _finished.TrySetResult(true);
            base.OnDisappearing();
        }
        public async void UpdateCaptchaInformation()
        {
            LoginViewModel.IsBusy = true;

            if (!(await LoginViewModel.Form.PrepareLogin()).IsSuccess)
            {
                await DisplayAlert("登录失败", "登录失败，出现了一些问题。", "知道了");
            }

            if (LoginViewModel.Form.CaptchaSource == null)
            {
                CaptchaPanel.IsVisible = false;
                AutoLoginBox.IsVisible = true;
            }
            else
            {
                CaptchaPanel.IsVisible = true;
                AutoLoginBox.IsVisible = false;

                _imageMem?.Close();
                _imageMem = new MemoryStream(LoginViewModel.Form.CaptchaSource, false);
                CaptchaImage.Source = ImageSource.FromStream(() => _imageMem);
            }

            LoginViewModel.IsBusy = false;
        }

        public virtual void SetNavigationArguments(LoginViewModel lvm)
        {
            LoginViewModel = lvm;
            UpdateCaptchaInformation();
        }
    }
}
