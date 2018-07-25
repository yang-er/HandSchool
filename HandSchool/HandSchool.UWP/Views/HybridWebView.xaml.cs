using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HandSchool.UWP.Views
{
    /// <summary>
    /// 混合式网页浏览器
    /// </summary>
    public sealed partial class HybridWebView : UserControl
    {
        public HybridWebView()
        {
            InitializeComponent();
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
        /// invokeCSharpAction的回调函数
        /// </summary>
        public Action<string> Register { get; set; }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            WebView.NavigateToString(Html.Replace("{webview_base_url}", "ms-appx-web:///WebWrapper//"));
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
    }
}
