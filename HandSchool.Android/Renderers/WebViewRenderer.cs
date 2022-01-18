using System;
using System.IO;
using System.Linq;
using System.Net;
using Android.Content;
using Android.Webkit;
using HandSchool.Controls;
using HandSchool.Droid.Renderers;
using HandSchool.Internals;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;
using SQLite;
using Xamarin.Forms.Internals;
using Path = System.IO.Path;

[assembly: ExportRenderer(typeof(HSWebView), typeof(HSWebViewRenderer))]

namespace HandSchool.Droid.Renderers
{
    [Table("cookies")]
    internal class AndroidCookieEntity
    {
        [Column("host_key")]
        public string HostKey { get; set; }
        
        [Column("name")]
        public string Name { get; set; }
        
        [Column("value")]
        public string Value { get; set; }
        
        [Column("path")]
        public string Path { get; set; }
        
        [Column("expires_utc")]
        public string ExpiresUtc { get; set; }
        
        [Column("is_secure")]
        public bool IsSecure { get; set; }
        
        [Column("is_httponly")]
        public bool IsHttpOnly { get; set; }
        
        [Column("last_access_utc")]
        public string LastAccessUtc { get; set; }
        
        [Column("has_expires")]
        public bool HasExpires { get; set; }
        
        [Column("is_persistent")]
        public bool IsPersistent { get; set; }
        
        [Column("priority")]
        public string Priority { get; set; }
        
        [Column("samesite")]
        public string SameSite { get; set; }
        
        [Column("source_scheme")]
        public string SourceScheme { get; set; }
        
        [Column("creation_utc")]
        public string CreationUtc { get; set; }
    }

    public class HSWebViewClient : FormsWebViewClient
    {
        private CookieManager _manager = CookieManager.Instance; 
        private SQLiteTableManager<AndroidCookieEntity> _cookieSql;

        public HSWebView WebView { get; set; }

        public HSWebViewClient(WebViewRenderer renderer, HSWebView v) : base(renderer)
        {
            WebView = v;
            _cookieSql = new SQLiteTableManager<AndroidCookieEntity>(false, BaseActivity.InternalFileRootPath,
                "app_webview", "Default", "Cookies");
        }

        private DateTime? _lastUpdate;
        //默认的Cookie在安卓平台不能正常工作，进行Cookie同步
        private void SyncCookiesFromStorage()
        {
            //_manager?.RemoveExpiredCookie();
            _manager?.Flush();
            if (!_cookieSql.HasTable()) return;
            var updateTime = File.GetLastWriteTime(_cookieSql.DataBasePath);
            if (_lastUpdate == updateTime) return;
            _lastUpdate = updateTime;
            var list = _cookieSql.GetItems();
            list.Select(ac => new Cookie
            {
                Name = ac.Name,
                Value = ac.Value,
                Domain = ac.HostKey,
                Path = ac.Path,
                HttpOnly = ac.IsHttpOnly,
                Secure = ac.IsSecure,
            }).ForEach(c => WebView?.HSCookies?.Add(c));
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