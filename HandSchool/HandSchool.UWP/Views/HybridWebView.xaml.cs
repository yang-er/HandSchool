using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HandSchool.Views
{
    /// <summary>
    /// 混合式网页浏览器
    /// </summary>
    public sealed partial class HybridWebView : UserControl
    {
        public HybridWebView()
        {
            InitializeComponent();
            WebView.NavigationStarting += OnWebViewNavigating;
        }

        /// <summary>
        /// 触发脚本
        /// </summary>
        /// <param name="eval">脚本内容</param>
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
        
        /// <summary>
        /// 显示的HTML字符串
        /// </summary>
        public string Html { get; set; }
        
        /// <summary>
        /// 导航的目标网址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// invokeCSharpAction的回调函数
        /// </summary>
        public Action<string> Register { get; set; }

        /// <summary>
        /// 请求子网页的事件
        /// </summary>
        public event Action<string> SubUrlRequested;
        
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (Html != string.Empty && Html != null)
                WebView.NavigateToString(Html.Replace("{webview_base_url}", "ms-appx-web:///WebWrapper//"));
            else
                WebView.Navigate(new Uri(Url));
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
            Register?.Invoke(e.Value);
        }

        private void OnWebViewNavigating(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.OriginalString != Url)
            {
                args.Cancel = true;
                SubUrlRequested?.Invoke(args.Uri.OriginalString);
            }
        }
    }
}
