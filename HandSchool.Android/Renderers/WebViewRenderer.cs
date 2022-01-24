using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Android.Content;
using Android.Webkit;
using HandSchool.Controls;
using HandSchool.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;
using Xamarin.Forms.Internals;
using Path = System.IO.Path;

[assembly: ExportRenderer(typeof(HSWebView), typeof(HSWebViewRenderer))]

namespace HandSchool.Droid.Renderers
{
    public class HSWebViewClient : FormsWebViewClient
    {
        private readonly CookieManager _manager = CookieManager.Instance;

        public HSWebView WebView { get; set; }

        public HSWebViewClient(WebViewRenderer renderer, HSWebView v) : base(renderer)
        {
            WebView = v;
        }

        private static string _cookiesDataBase;
        private DateTime? _lastUpdate;
        private string _lastUrl;
        //默认的Cookie在安卓平台不能正常工作，进行Cookie同步
        private void SyncCookiesFromStorage(string url)
        {
            if (_manager is null) return;
            if (!_manager.HasCookies) return;
            _manager.Flush();
            
            //首先检测Cookie是否被更新，此步骤可能不可用
            //因为这是安卓默认的WebView Cookie存放位置，若厂商有自己的WebView实现，则不可用
            _cookiesDataBase ??= Path.Combine(BaseActivity.InternalFileRootPath, "app_webview", "Default", "Cookies");
            if (System.IO.File.Exists(_cookiesDataBase))
            {
                var update = System.IO.File.GetLastWriteTime(_cookiesDataBase);
                if (update == _lastUpdate && _lastUrl == url) return;
                _lastUrl = url;
                _lastUpdate = update;
            }
            
            var uri = new Uri(url);
            
            //将路径拆开
            var ps = new List<string>{""};
            uri.AbsolutePath.Trim()
                .Split('/')
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ForEach(p => ps.Add($"{ps[^1]}/{p}"));
            
            //将域名拆开
            var hosts = uri.Host.Trim().Split('.');
            var dps = new List<(string, string)>();

            //将所有带有点前缀的父域名加入查询队列
            var superHost = hosts[^1];
            for (var i = hosts.Length - 2; i >= 0; i--)
            {
                superHost = $"{hosts[i]}.{superHost}";
                ps.ForEach(p => dps.Add(($".{superHost}", p)));
            }
            
            //将域名本身加入查询队列
            ps.ForEach(p => dps.Add(($"{uri.Host}", p)));
            var res = new Dictionary<string, List<Cookie>>();
            
            //查询所有的domain-path对，并进行合并处理
            dps.ForEach(dp =>
            {
                var (domain, path) = dp;
                ParseCookies(
                    domain, 
                    path == "" ? "/" : path,
                    _manager.GetCookie($"{uri.Scheme}://{domain}{path}"))
                    .ForEach(c => AddCookie(res, c));
            });
            
            //将结果同步到HSCookies
            res.Values.ForEach(cs => cs.ForEach(WebView.HSCookies.Add));
        }

#nullable enable
        private static void AddCookie(IDictionary<string, List<Cookie>> dic, Cookie c)
        {
            if (dic.ContainsKey(c.Name))
            {
                var cur = dic[c.Name];
                if (!cur.Any(cookie => c.Name == cookie.Name && c.Value == cookie.Value))
                {
                    cur.Add(c);
                }
            }
            else
            {
                dic[c.Name] = new List<Cookie> {c};
            }
        }
        private static IEnumerable<Cookie> ParseCookies(string domain, string path, string? str)
        {
            return str?.Split(";")
                ?.Select(s => s?.Trim().Split("="))
                .Where(ss =>
                {
                    if (ss is null) return false;
                    return ss.Length != 0 && ss.Length <= 2;
                })
                .Select(ss =>
                {
                    var res = new Cookie
                    {
                        Domain = domain,
                        Path = path
                    };
                    switch (ss!.Length)
                    {
                        case 1:
                            res.Name = ss[0];
                            break;
                        case 2:
                            res.Name = ss[0];
                            res.Value = ss[1];
                            break;
                    }

                    return res;
                }) ?? Array.Empty<Cookie>();
        }
#nullable disable
        
        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            SyncCookiesFromStorage(url);
            base.OnPageFinished(view, url);
        }
    }

    public class HSWebViewRenderer : WebViewRenderer
    {
        public HSWebViewRenderer(Context context) : base(context) { }

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
                var hsWebView = (HSWebView)e.NewElement;
                Control.Settings.JavaScriptEnabled = true;
                var c = new HSWebViewClient(this, hsWebView);
                Control.SetWebViewClient(c);
            }
        }
    }
}