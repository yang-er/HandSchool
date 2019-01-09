using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace HandSchool.Views
{
    public sealed partial class LoginDialog : ViewDialog, ILoginPage
    {
        private ContentDialogButtonClickDeferral deferral;

        public LoginViewModel LoginViewModel
        {
            get => ViewModel as LoginViewModel;
            private set => ViewModel = value;
        }

        public LoginDialog(LoginViewModel vm) : base()
        {
            InitializeComponent();
            LoginViewModel = vm;
            UpdateCaptchaInfomation();
        }
        
        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (args.Result != ContentDialogResult.None)
                args.Cancel = !LoginViewModel.Form.IsLogin;
        }
        
        public async void Response(object sender, LoginStateEventArgs e)
        {
            switch (e.State)
            {
                case LoginState.Processing:
                    await RequestMessageAsync("正在登录", "正在登录中，请稍后……", "知道了");
                    break;
                case LoginState.Succeeded:
                    deferral.Complete();
                    break;
                case LoginState.Failed:
                    await RequestMessageAsync("登录失败", $"登录失败，{e.InnerError}。", "知道了");
                    UpdateCaptchaInfomation();
                    deferral.Complete();
                    break;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (ViewModel.IsBusy)
            {
                args.Cancel = true;
            }
            else
            {
                deferral = args.GetDeferral();
                LoginViewModel.LoginCommand.Execute(null);
            }
        }

        public async void UpdateCaptchaInfomation()
        {
            ViewModel.IsBusy = true;

            if (!await LoginViewModel.Form.PrepareLogin())
            {
                await RequestMessageAsync("登录失败", "登录失败，出现了一些问题。", "知道了");
            }

            if (LoginViewModel.Form.CaptchaSource == null)
            {
                CaptchaBpx.Visibility = Visibility.Collapsed;
                CaptchaImage.Visibility = Visibility.Collapsed;
                AutoLoginBox.Visibility = Visibility.Visible;
            }
            else
            {
                CaptchaBpx.Visibility = Visibility.Visible;
                CaptchaImage.Visibility = Visibility.Visible;
                AutoLoginBox.Visibility = Visibility.Collapsed;

                var ret = new BitmapImage();
                var stream = new InMemoryRandomAccessStream();
                var writer = new DataWriter(stream.GetOutputStreamAt(0));
                writer.WriteBytes(LoginViewModel.Form.CaptchaSource);
                await writer.StoreAsync();
                await ret.SetSourceAsync(stream);
                CaptchaImage.Source = ret;
            }

            ViewModel.IsBusy = false;
        }
    }
}
