using HandSchool.Services;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace HandSchool.UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public partial class WebViewPage : ViewPage
    {
        private IInfoEntrance InfoEntrance { get; set; }

        public WebViewPage()
        {
            InitializeComponent();
        }

        public async void InvokeScript(string eval)
        {
            try
            {
                await WebView.InvokeScriptAsync("eval", new[] { eval });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private string _html = string.Empty;
        public string Html
        {
            get => _html;
            set => _html = value;
        }

        private Action<string> _reg;
        public Action<string> Register
        {
            get => _reg;
            set => _reg = value;
        }
        
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            WebView.NavigateToString(_html.Replace("{webview_base_url}", "ms-appx-web:///WebWrapper//"));
        }

        private async void OnWebViewNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                // Inject JS script
                await WebView.InvokeScriptAsync("eval", new[] { "function invokeCSharpAction(data){window.external.notify(data);}" });
            }
        }

        private void OnWebViewScriptNotify(object sender, NotifyEventArgs e)
        {
            _reg?.Invoke(e.Value);
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is AboutViewModel vm)
            {
                BindingContext = AboutViewModel.Instance;
                AboutViewModel.Instance.BindingContext = new ViewResponse(this);
                var sb = new StringBuilder();
                AboutViewModel.Instance.HtmlDocument.ToHtml(sb);
                Html = sb.ToString();
                Register = AboutViewModel.Instance.Response;
            }
            else if (e.Parameter is IInfoEntrance entrance)
            {
                BindingContext = new BaseViewModel { Title = entrance.Name };
                entrance.Evaluate = InvokeScript;
                InfoEntrance = entrance;
                InfoEntrance.Binding = new ViewResponse(this);
                var sb = new StringBuilder();
                InfoEntrance.HtmlDocument.ToHtml(sb);
                Html = sb.ToString();
                Register = entrance.Receive;
                foreach (var key in InfoEntrance.Menu)
                    PrimaryMenu.Add(new AppBarButton { Label = key.Name, Command = key.Command, Icon = new FontIcon { FontFamily = new FontFamily("Segoe MDL2 Assets"), Glyph = key.Icon } });
            }
        }
    }
}
