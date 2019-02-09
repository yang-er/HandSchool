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
    public partial class LoginPage : ViewPage, ILoginPage
    {
        MemoryStream image_mem;
        
        internal LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            LoginViewModel = viewModel;
            On<_iOS_>().UseSafeArea().ShowLeftCancel();
            On<_Each_>().ShowLoading();
            UpdateCaptchaInfomation();
        }

        public LoginViewModel LoginViewModel
        {
            get => BindingContext as LoginViewModel;
            set => ViewModel = value;
        }

        public async void Response(object sender, LoginStateEventArgs e)
        {
            switch (e.State)
            {
                case LoginState.Processing:
                    await DisplayAlert("正在登录", "正在登录中，请稍后……", "知道了");
                    break;
                case LoginState.Succeeded:
                    await CloseAsync();
                    break;
                case LoginState.Failed:
                    await DisplayAlert("登录失败", $"登录失败，{e.InnerError}。", "知道了");
                    UpdateCaptchaInfomation();
                    break;
            }
        }

        public Task ShowAsync()
        {
            return (this as Page).Navigation.PushModalAsync(this);
        }

        public async void UpdateCaptchaInfomation()
        {
            LoginViewModel.IsBusy = true;

            if (!await LoginViewModel.Form.PrepareLogin())
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
    }
}
