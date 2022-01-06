using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Android.Content;
using Android.Webkit;
using HandSchool.Controls;
using HandSchool.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

[assembly: ExportRenderer(typeof(HSWebView), typeof(HSWebViewRenderer))]

//默认的Cookie在安卓平台不能正常工作
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
        
        /// <summary>
        /// 获取WebView中指定url的Cookie
        /// </summary>
        /// <param name="url">获取Cookie的url</param>
        /// <returns></returns>
        /// <exception cref="UriFormatException">当url不以http或https开头时抛出</exception>
        static IList<Cookie> GetCookies(string url)
        {
            var trimmed = url.Trim();
            var lower = trimmed.ToLower();
            if (!lower.StartsWith("https://") && !lower.StartsWith("https://"))
            {
                throw new UriFormatException("url must starts with \"http://\" or \"https://\"");
            }

            var isHttps = lower.StartsWith("https://");
            var domain = trimmed;
            domain = isHttps ? domain[8..] : domain[7..];
            var index = domain.IndexOf('/');
            //当url没有子路径时，直接返回
            if (index == -1)
            {
                var u = $"{(isHttps ? "https://" : "http://")}{domain}";
                return ParseCookies(domain, "/", CookieManager.Instance?.GetCookie(u)).ToList();
            }
            //获取url上各个路径上的Cookie，进行合并
            else
            {
                var path = domain;
                domain = domain[..index];
                var u = $"{(isHttps ? "https://" : "http://")}{domain}";
                var paths = path[index..].Split('/');
                var p = new StringBuilder("/");
                var dic = ParseCookies(domain, p.ToString(), CookieManager.Instance?.GetCookie(u + p))
                    .ToDictionary(c => c.Name);
                foreach (var pa in paths)
                {
                    if(string.IsNullOrWhiteSpace(pa))continue;
                    p.Append(pa).Append('/');
                    var cs = ParseCookies(domain, p.ToString(), CookieManager.Instance?.GetCookie(u + p));
                    //如果字典中已经有同名的Cookie，若它们的值不一致，则更新Cookie
                    foreach (var c in cs)
                    {
                        if (dic.ContainsKey(c.Name))
                        {
                            if (dic[c.Name].Value == c.Value)
                            {
                                continue;
                            }
                        }

                        dic[c.Name] = c;
                    }
                }

                return dic.Values.ToList();
            }
        }

        /// <summary>
        /// 将从CookieManager中获取到的字符串解析为Cookie
        /// </summary>
        /// <param name="domain">Cookie的域名</param>
        /// <param name="path">Cookie的路径</param>
        /// <param name="cookies">从CookieManager中获取的Cookie字符串</param>
        /// <returns></returns>
        static IEnumerable<Cookie> ParseCookies(string domain, string path, string cookies)
        {
            if (string.IsNullOrWhiteSpace(cookies)) yield break;
            var cs =
                cookies.Split(';')
                    .Select(s => s.Trim().Split("="));
            foreach (var c in cs)
            {
                if (c.Length != 1 && c.Length != 2) continue;
                yield return new Cookie(c[0], c.Length == 2 ? c[1] : null, path, domain);
            }
        }
        
        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            foreach (var cookie in GetCookies(url))
            {
                WebView?.Cookies?.Add(cookie);
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