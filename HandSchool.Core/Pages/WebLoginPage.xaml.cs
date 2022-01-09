using System.Net;
using System.Threading.Tasks;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Views;
using Xamarin.Forms.Xaml;

namespace HandSchool.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public abstract partial class WebLoginPage : ViewObject, ILoginPage
    {
        public WebLoginPage()
        {
            InitializeComponent();
        }

        private WebLoginPageEvents _events;
        public string LoginUrl => Form.LoginUrl;
        public TaskCompletionSource<TaskResp> Result { get; set; }

        public LoginViewModel LoginViewModel
        {
            get => BindingContext as LoginViewModel;
            set => ViewModel = value;
        }
        public override void SetNavigationArguments(object param)
        {
            base.SetNavigationArguments(param);
            Form = param as IWebLoginField;
            if (Form is null) return;
            _events = Form.Events;
            Title = "登录" + Form.FormName;
            LoginView.Events = Form.Events.WebViewEvents;
            LoginView.Source = Form.LoginUrl;
            if (_events != null)
            {
                Result = _events.Result = new TaskCompletionSource<TaskResp>();
                _events.Page = this;
            }
        }
        public void SetNavigationArguments(LoginViewModel lvm)
        {
            LoginViewModel = lvm;
            UpdateCaptchaInformation();
        }

        private IWebLoginField Form { get; set; }
        public abstract Task CloseAsync();
        protected override void OnDisappearing()
        {
            if (!Form.IsLogin)
            {
                Result.TrySetResult(TaskResp.False);
            }

            Form = null;
            if (_events != null)
            {
                _events.Page = null;
                Result = _events.Result = null;
            }

            LoginView.Source = null;
        }

        public abstract Task ShowAsync();
        public async void OnLoginStateChanged(object sender, LoginStateEventArgs e)
        {
            switch (e.State)
            {
                case LoginState.Processing:
                    await DisplayAlert("正在登录", "正在登录中，请稍候……", "知道了");
                    break;
                case LoginState.Succeeded:
                    await CloseAsync();
                    break;
                case LoginState.Failed:
                    await DisplayAlert("登录失败", $"登录失败，{e.InnerError}。", "知道了");
                    break;
            }
        }

        public void UpdateCaptchaInformation()
        {
            LoginView.Source = LoginUrl;
        }
    }
}