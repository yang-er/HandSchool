using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Android.Content;
using Android.Graphics;
using Android.Webkit;
using HandSchool.Controls;
using HandSchool.Droid.Internals;
using HandSchool.Droid.Renderers;
using HandSchool.Models;
using Java.Interop;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;
using Xamarin.Forms.Internals;

[assembly: ExportRenderer(typeof(HSWebView), typeof(HSWebViewRenderer))]

namespace HandSchool.Droid.Renderers
{
    public class HSWebViewClient : FormsWebViewClient
    {
        private const string WebViewCookieStrategyConfig = "webview_cookie_strategy";

        private const string NativeInvoker =
            "if(typeof(invokeNativeAction) == 'undefined') { var invokeNativeAction = function(data) { jsBridge.invokeAction(data); }; }";

        private readonly WebViewRenderer _webViewRenderer;
        public HSWebView WebView => (HSWebView) _webViewRenderer.Element;

        public HSWebViewClient(WebViewRenderer renderer) : base(renderer)
        {
            _webViewRenderer = renderer;
            var strategy = Core.Configure.Configs.GetItemWithPrimaryKey(WebViewCookieStrategyConfig)?.Value;
            if (strategy is null)
            {
                _getCookiesStrategy = new SelectStrategy(this);
            }
            else
            {
                var type = Type.GetType(strategy);
                _getCookiesStrategy = type is null
                    ? new SelectStrategy(this)
                    : (IGetCookiesStrategy) Activator.CreateInstance(type);
            }
        }

        private IGetCookiesStrategy _getCookiesStrategy;

        public override void OnPageStarted(Android.Webkit.WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
            _webViewRenderer.Control.EvaluateJavascript(NativeInvoker, new StringAsyncResult());
        }

        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            CookieManager.Instance?.RemoveExpiredCookie();
            CookieManager.Instance?.Flush();
            _getCookiesStrategy.GetCookiesFromNative(url).ForEach(WebView.HSCookies.Add);
            base.OnPageFinished(view, url);
        }

        /// <summary>
        /// 此策略是第一次启动时的默认策略，通过判断来确定使用的策略
        /// </summary>
        private class SelectStrategy : IGetCookiesStrategy
        {
            private readonly HSWebViewClient _father;
            private readonly CookieManager _manager;

            public SelectStrategy(HSWebViewClient renderer)
            {
                _father = renderer;
                _manager = CookieManager.Instance;
            }

            public IEnumerable<Cookie> GetCookiesFromNative(string url)
            {
                string strategy = null;
                var db = new DataBaseStrategy();
                var dbRes = db.GetCookiesFromNative(url).ToList();
                IEnumerable<Cookie> res;

                //用数据库策略查询，若得到Cookie的个数为0，
                //可能是：
                //1、此时确实没有Cookie（先不选策略）；
                //2、本机的WebView实现没有将Cookie存储在已知路径（选择纯CookieManager策略）。
                if (dbRes.Count == 0)
                {
                    if (_manager.HasCookies)
                    {
                        strategy = typeof(CookieManagerStrategy).FullName;
                        res = (_father._getCookiesStrategy = new CookieManagerStrategy()).GetCookiesFromNative(url);
                    }
                    else
                    {
                        res = ArraySegment<Cookie>.Empty;
                    }
                }
                //若得到的Cookie数量不是0，则检查数据是否被加密；
                //若未加密，则选用数据库模式，否则选用CookieManager策略。
                else
                {
                    if (dbRes.TrueForAll(c => string.IsNullOrWhiteSpace(c.Value)))
                    {
                        strategy = typeof(CookieManagerStrategy).FullName;
                        res = (_father._getCookiesStrategy = new CookieManagerStrategy()).GetCookiesFromNative(url);
                    }
                    else
                    {
                        strategy = typeof(DataBaseStrategy).FullName;
                        _father._getCookiesStrategy = db;
                        res = dbRes;
                    }
                }

                if (strategy != null)
                {
                    Core.Configure.Configs.InsertOrUpdateTable(new Config
                    {
                        ConfigName = WebViewCookieStrategyConfig,
                        Value = strategy
                    });
                }

                return res;
            }
        }
    }

    public class HSWebViewRenderer : WebViewRenderer
    {
        public HSWebViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
            }

            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                Control.Settings.JavaScriptEnabled = true;
                Control.AddJavascriptInterface(this, "jsBridge");
            }
        }

        protected override WebViewClient GetWebViewClient()
        {
            return new HSWebViewClient(this);
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(string data)
        {
            (Element as HSWebView)?.SendReceivingJsData(data);
        }
    }
}