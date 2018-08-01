using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using System.IO;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : PopContentPage
    {
        MemoryStream image_mem;
        
        internal LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            UpdateCaptchaInfomation();
        }

        internal async void Response(object sender, LoginStateEventArgs e)
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

        public async void UpdateCaptchaInfomation()
        {
            var viewModel = ViewModel as LoginViewModel;
            ViewModel.IsBusy = true;

            if (!await viewModel.Form.PrepareLogin())
            {
                await DisplayAlert("登录失败", "登录失败，出现了一些问题。", "知道了");
            }

            if (viewModel.Form.CaptchaSource == null)
            {
                CaptchaFullBox.IsEnabled = false;
                AutoLoginBox.IsEnabled = true;
            }
            else
            {
                CaptchaFullBox.IsEnabled = true;
                AutoLoginBox.IsEnabled = false;

                if (image_mem != null)
                    image_mem.Close();
                image_mem = new MemoryStream(viewModel.Form.CaptchaSource, false);
                CaptchaImage.Source = ImageSource.FromStream(() => image_mem);
            }

            ViewModel.IsBusy = false;
        }
    }
}
