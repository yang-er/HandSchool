using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace HandSchool.UWP.Views
{
    public sealed partial class LoginDialog : ContentDialog
    {
        private LoginViewModel ViewModel { get; }
        private ContentDialogButtonClickDeferral deferral;

        public LoginDialog(LoginViewModel vm) : base()
        {
            InitializeComponent();
            ViewModel = vm;
            DataContext = vm.Form;
            UpdateCaptchaInfomation();
        }
        
        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            args.Cancel = !ViewModel.Form.IsLogin;
        }
        
        internal async void Response(object sender, LoginStateEventArgs e)
        {
            switch (e.State)
            {
                case LoginState.Processing:
                    await ViewResponse.ShowMessageAsync("正在登录", "正在登录中，请稍后……", "知道了");
                    break;
                case LoginState.Succeeded:
                    deferral?.Complete();
                    break;
                case LoginState.Failed:
                    await ViewResponse.ShowMessageAsync("登录失败", $"登录失败，{e.InnerError}。", "知道了");
                    deferral?.Complete();
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
                ViewModel.LoginCommand.Execute(null);
            }
        }

        public async void UpdateCaptchaInfomation()
        {
            ViewModel.IsBusy = true;

            if (!await ViewModel.Form.PrepareLogin())
            {
                await ViewResponse.ShowMessageAsync("登录失败", "登录失败，出现了一些问题。");
            }

            if (ViewModel.Form.CaptchaSource == null)
            {
                CaptchaBpx.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                CaptchaImage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                CaptchaBpx.Visibility = Windows.UI.Xaml.Visibility.Visible;
                CaptchaImage.Visibility = Windows.UI.Xaml.Visibility.Visible;

                var ret = new BitmapImage();
                var stream = new InMemoryRandomAccessStream();
                var writer = new DataWriter(stream.GetOutputStreamAt(0));
                writer.WriteBytes(ViewModel.Form.CaptchaSource);
                await writer.StoreAsync();
                await ret.SetSourceAsync(stream);
                CaptchaImage.Source = ret;
            }

            ViewModel.IsBusy = false;
        }
    }

    public class BoolReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }
    }
}
