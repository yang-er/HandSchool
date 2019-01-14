using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Views
{
    /// <summary>
    /// 一个支持JavaScript调用的WebView
    /// </summary>
    /// <example>
    /// hybridWebView.RegisterAction(data => DisplayAlert ("Alert", "Hello " + data, "OK"));
    /// hybridWebView.JavaScript("$('.table tbody').append('...');");
    /// </example>
    /// <see cref="https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/custom-renderer/hybridwebview" />
    /// <see cref="https://github.com/xamarin/xamarin-forms-samples/tree/master/CustomRenderers/HybridWebView" />
    public class HybridWebView : View
    {
        Func<string, Task> action;

        /// <summary>
        /// Uri属性的内部储存
        /// </summary>
        public static readonly BindableProperty UriProperty = 
            BindableProperty.Create(
                propertyName: nameof(Uri), 
                returnType: typeof(string), 
                declaringType: typeof(HybridWebView), 
                defaultValue: default(string));

        /// <summary>
        /// Html属性的内部储存
        /// </summary>
        public static readonly BindableProperty HtmlProperty = 
            BindableProperty.Create(
                propertyName: nameof(Html), 
                returnType: typeof(string), 
                declaringType: typeof(HybridWebView), 
                defaultValue: default(string));

        /// <summary>
        /// OpenWithPost属性的内部储存
        /// </summary>
        [Obsolete("This property does not support any more")]
        public static readonly BindableProperty OpenWithPostProperty = 
            BindableProperty.Create(
                propertyName: nameof(OpenWithPost),
                returnType: typeof(byte[]),
                declaringType: typeof(HybridWebView),
                defaultValue: null);

        /// <summary>
        /// 访问页面的地址
        /// </summary>
        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        /// <summary>
        /// 访问页面的Html值
        /// </summary>
        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        /// <summary>
        /// 访问页面的OpenWithPost值
        /// </summary>
        [Obsolete("This property does not support any more")]
        public byte[] OpenWithPost
        {
            get { return (byte[])GetValue(OpenWithPostProperty); }
            set { SetValue(OpenWithPostProperty, value); }
        }

        /// <summary>
        /// 使用的默认cookie值
        /// </summary>
        [Obsolete("This property does not support any more")]
        public List<string> Cookie { get; set; }

        /// <summary>
        /// 请求执行JavaScript的事件
        /// </summary>
        public event Action<string> JavaScriptRequested;

        /// <summary>
        /// 请求子网页的事件
        /// </summary>
        public event Action<string> SubUrlRequested;

        /// <summary>
        /// 加载完成
        /// </summary>
        public event Action LoadCompleted;

        /// <summary>
        /// 触发加载完成
        /// </summary>
        public void NotifyLoadComplete()
        {
            LoadCompleted?.Invoke();
        }

        /// <summary>
        /// 触发请求子网页
        /// </summary>
        /// <param name="sub">子网页地址</param>
        public void RaiseSubUrlRequest(string sub)
        {
            SubUrlRequested?.Invoke(sub);
        }

        /// <summary>
        /// 执行JavaScript
        /// </summary>
        /// <param name="str">JS代码</param>
        public void JavaScript(string str)
        {
            JavaScriptRequested?.Invoke(str);
        }

        /// <summary>
        /// 注册invokeCSharpAction回调函数
        /// </summary>
        /// <param name="callback">回调函数</param>
        public void RegisterAction(Func<string, Task> callback)
        {
            action = callback;
        }

        /// <summary>
        /// 清理回调函数
        /// </summary>
        public void Cleanup()
        {
            action = null;
        }

        /// <summary>
        /// 执行动作
        /// </summary>
        /// <param name="data">数据</param>
        public void InvokeAction(string data)
        {
            if (data == null) return;
            action?.Invoke(data);
        }
    }
}
