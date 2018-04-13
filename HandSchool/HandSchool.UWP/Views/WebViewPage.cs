using System;
using Windows.UI.Xaml.Controls;

namespace HandSchool.UWP
{
    public class WebViewPage : ViewPage
    {
        public WebView WebView;

        private string _html;
        public string Html
        {
            get => _html;
            set { _html = value; WebView.NavigateToString(_html.Replace("{webview_base_url}", "ms-appx-web:///WebWrapper//")); }
        }

        private Action<string> _reg;
        public Action<string> Register
        {
            get => _reg;
            set => _reg = value;
        }

        public WebViewPage()
        {
            WebView = new WebView();
            Content = WebView;
            WebView.NavigationCompleted += OnWebViewNavigationCompleted;
            WebView.ScriptNotify += OnWebViewScriptNotify;
        }
        
        public async void InvokeScript(string eval)
        {
            await WebView.InvokeScriptAsync("eval", new[] { eval });
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
            _reg.Invoke(e.Value);
        }
    }
}
