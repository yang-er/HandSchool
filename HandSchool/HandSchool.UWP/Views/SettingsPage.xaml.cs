using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
    public sealed partial class SettingsPage : ViewPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = SettingViewModel.Instance;

            AboutWebView.DataContext = AboutViewModel.Instance;
            AboutViewModel.Instance.BindingContext = new ViewResponse(this);
            var sb = new StringBuilder();
            AboutViewModel.Instance.HtmlDocument.ToHtml(sb);
            AboutWebView.Html = sb.ToString();
            AboutWebView.Register = AboutViewModel.Instance.Response;
        }
    }
}
