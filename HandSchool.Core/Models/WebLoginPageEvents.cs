using System;
using System.Threading.Tasks;
using HandSchool.Controls;
using HandSchool.Pages;
using Xamarin.Forms;

namespace HandSchool.Models
{
    /// <summary>
    /// 用来XF WebView导航事件的注入
    /// </summary>
    public class HSWebViewEvents
    {
        public event EventHandler<WebNavigatedEventArgs> Navigated;
        public event EventHandler<WebNavigatingEventArgs> Navigating;
        public event Action<string> ReceivingJsData; 
        public HSWebView WebView { get; set; }
        public Task<string> EvaluateJavaScriptAsync(string script) =>
            WebView?.EvaluateJavaScriptAsync(script) ?? Task.FromResult<string>(null);
        public void OnNavigated(object sender, WebNavigatedEventArgs args) => Navigated?.Invoke(sender, args);
        public void OnNavigating(object sender, WebNavigatingEventArgs args) => Navigating?.Invoke(sender, args);
        public void OnReceivingJsData(string data) => ReceivingJsData?.Invoke(data);
    }
    
    /// <summary>
    /// 连接WebLoginPage和WebView的交互
    /// </summary>
    public class WebLoginPageEvents
    {
        /// <summary>
        /// 用来标识登录的结果（成功与否）
        /// </summary>
        public TaskCompletionSource<TaskResp> Result { get; set; }
        /// <summary>
        /// 登录页面
        /// </summary>
        public WebLoginPage Page { get; set; }
        public HSWebViewEvents WebViewEvents { get; set; }
    }
}