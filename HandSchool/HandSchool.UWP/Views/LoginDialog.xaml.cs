using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace HandSchool.UWP
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
                    await Internal.Helper.ShowMessage("正在登录", "正在登录中，请稍后……", "知道了");
                    break;
                case LoginState.Succeeded:
                    deferral?.Complete();
                    break;
                case LoginState.Failed:
                    await Internal.Helper.ShowMessage("登录失败", $"登录失败，{e.InnerError}。", "知道了");
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
