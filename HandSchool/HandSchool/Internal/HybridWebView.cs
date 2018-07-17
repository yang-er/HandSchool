using System;
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
        Action<string> action;

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
            BindableProperty.Create(propertyName: nameof(Html), 
                returnType: typeof(string), 
                declaringType: typeof(HybridWebView), 
                defaultValue: default(string));

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
        /// 请求执行JavaScript的事件
        /// </summary>
        public event Action<string> JavaScriptRequested;

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
        public void RegisterAction(Action<string> callback)
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
