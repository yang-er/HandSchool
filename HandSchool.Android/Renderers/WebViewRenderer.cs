using System;
using System.IO;
using System.Linq;
using System.Net;
using Android.Content;
using Android.Webkit;
using HandSchool.Controls;
using HandSchool.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;
using SQLite;
using Xamarin.Forms.Internals;
using Path = System.IO.Path;

[assembly: ExportRenderer(typeof(HSWebView), typeof(HSWebViewRenderer))]

namespace HandSchool.Droid.Renderers
{
    internal class AndroidCookieEntity
    {
        public string host_key { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string path { get; set; }
        public string expires_utc { get; set; }
        public string is_secure { get; set; }
        public string is_httponly { get; set; }
        public string last_access_utc { get; set; }
        public string has_expires { get; set; }
        public string is_persistent { get; set; }
        public string priority { get; set; }
        public string samesite { get; set; }
        public string source_scheme { get; set; }
        public string creation_utc { get; set; }
    }

    public class HSWebViewClient : FormsWebViewClient
    {
        private CookieManager _manager = CookieManager.Instance; 
        public HSWebView WebView { get; set; }

        public HSWebViewClient(WebViewRenderer renderer, HSWebView v) : base(renderer)
        {
            WebView = v;
        }

        private DateTime? _lastUpdate;
        //默认的Cookie在安卓平台不能正常工作，进行Cookie同步
        private void SyncCookiesFromStorage()
        {
            //_manager?.RemoveExpiredCookie();
            _manager?.Flush();
            var cookieDbPath = Path.Combine(BaseActivity.InternalFileRootPath, "app_webview", "Default", "Cookies");
            if (!File.Exists(cookieDbPath)) return;
            var updateTime = File.GetLastWriteTime(cookieDbPath);
            if (_lastUpdate == updateTime) return;
            _lastUpdate = updateTime;
            var connect = new SQLiteConnection(cookieDbPath);
            var list = connect.Query<AndroidCookieEntity>("select * from cookies");
            list.Select(ac => new Cookie
            {
                Name = ac.name,
                Domain = ac.host_key,
                Path = ac.path,
                HttpOnly = ac.is_httponly == "1",
                Secure = ac.is_secure == "1",
                Value = ac.value
            }).ForEach(c => WebView?.HSCookies?.Add(c));
            connect.Close();
        }
        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            SyncCookiesFromStorage();
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