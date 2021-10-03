using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ViewObject, ILoginPage
    {
        MemoryStream image_mem;
        readonly TaskCompletionSource<bool> finished;
        
        public LoginPage()
        {
            InitializeComponent();
            finished = new TaskCompletionSource<bool>();
        }

        public LoginViewModel LoginViewModel
        {
            get => BindingContext as LoginViewModel;
            set
            {
                login.Command = value.LoginCommand;
                ViewModel = value;
            }
        }

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
                    UpdateCaptchaInformation();
                    break;
            }
        }
        private Task CloseAsync() => Application.Current.MainPage.Navigation.PopModalAsync();

        public Task ShowAsync()
        {

            Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(this));
            return finished.Task;
        }

        protected override void OnDisappearing()
        {
            finished.TrySetResult(true);
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
                CaptchaBox.IsVisible = false;
                AutoLoginBox.IsVisible = true;
            }
            else
            {
                CaptchaBox.IsVisible = true;
                AutoLoginBox.IsVisible = false;

                if (image_mem != null)
                    image_mem.Close();
                image_mem = new MemoryStream(LoginViewModel.Form.CaptchaSource, false);
                CaptchaImage.Source = ImageSource.FromStream(() => image_mem);
            }

            LoginViewModel.IsBusy = false;
        }

        public void SetNavigationArguments(LoginViewModel lvm)
        {
            LoginViewModel = lvm;
            On<_iOS_>().UseSafeArea().ShowLeftCancel();
            UpdateCaptchaInformation();
        }
    }
}
