using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HandSchool.UWP
{
    public sealed partial class HybridWebView : UserControl
    {
        public HybridWebView()
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
    }
}
