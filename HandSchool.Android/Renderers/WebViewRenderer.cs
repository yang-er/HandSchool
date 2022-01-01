using System;
using System.Linq;
using System.Net;
using Android.Content;
using Android.Webkit;
using HandSchool.Controls;
using HandSchool.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

[assembly: ExportRenderer(typeof(HSWebView), typeof(HSWebViewRenderer))]

namespace HandSchool.Droid.Renderers
{
    public class HSWebViewClient : FormsWebViewClient
    {
        public  HSWebView WebView { get; set; }
        private readonly CookieManager _manager = CookieManager.Instance;
        public HSWebViewClient(WebViewRenderer renderer, HSWebView v):base(renderer)
        {
            WebView = v;
        }

        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            var domain = url;
            var str = _manager.GetCookie(url);
            if (string.IsNullOrWhiteSpace(str)) return;
            var cookies = str.Split(";").Select(s => s.Trim()).Select(s => s.Split("="));
            domain = domain.StartsWith("https://") ? domain[8..] : domain[7..];
            var index = domain.IndexOf("/", StringComparison.Ordinal);
            var path = index == -1 ? "/" : domain[index..];
            domain = index == -1 ? domain : domain[..index];
            foreach (var cookie in cookies)
            {
                WebView?.HSCookies?.Add(new Cookie(cookie[0], cookie[1], path, domain));
            }
            base.OnPageFinished(view, url);
        }
    }
    public class HSWebViewRenderer : WebViewRenderer
    {
        public HSWebViewRenderer(Context context):base(context){}

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement != null)
            {
                Control.Settings.JavaScriptEnabled = true;
                var wvClient = Control.WebViewClient;
                if (wvClient is HSWebViewClient hsClient)
                {
                    hsClient.WebView = null;
                }
            }

            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var hsWebView = (HSWebView) e.NewElement;
                Control.Settings.JavaScriptEnabled = true;
                var c = new HSWebViewClient(this, hsWebView);
                Control.SetWebViewClient(c);
            }
        }
    }
}