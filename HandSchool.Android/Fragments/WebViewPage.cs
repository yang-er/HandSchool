using Android.OS;
using Android.Views;
using Android.Webkit;
using HandSchool.Droid;
using HandSchool.Internal;
using HandSchool.Services;
using HandSchool.ViewModels;
using Java.Interop;
using System;
using System.Text;
using System.Threading.Tasks;

namespace HandSchool.Views
{
    public class WebViewPage : ViewFragment
    {
        public WebViewPage()
        {
            FragmentViewResource = Resource.Layout.layout_webview;
        }

        [BindView(Resource.Id.web_view)]
        public WebView WebView { get; set; }

        private Func<string, Task> Callback { get; set; }
        public string Uri { get; set; }
        public string Html { get; set; }

        public BaseController Entrance
        {
            get => ViewModel as BaseController;
            set => ViewModel = value;
        }

        public override void SetNavigationArguments(object param)
        {
            Entrance = param as BaseController;
            var meta = Entrance.GetType().Get<EntranceAttribute>();
            Title = meta.Title;
            
            if (Entrance is IInfoEntrance ie)
            {
                var sb = new StringBuilder();
                ie.HtmlDocument.ToHtml(sb);
                Html = sb.ToString();
            }
            else if (Entrance is IUrlEntrance iu)
            {
                Uri = iu.HtmlUrl;

                if (Uri.Contains("://"))
                {
                    Entrance.SetIsBusy(true);
                }
            }

            foreach (var key in Entrance.Menu)
            {
                /* TODO:
                 * AddToolbarEntry(new MenuEntry
                {
                    Title = key.Name,
                    Command = key.Command,
                });
                ToolbarItems.Add(new ToolbarItem
                {
                    Text = key.Name,
                    Command = key.Command,
                    CommandParameter = (Action<IWebEntrance>)OnEntranceRequested
                });*/
            }

            Entrance.Evaluate = JavaScript;
            Callback = Entrance.Receive;
        }

        public void JavaScript(string script)
        {
            Core.Platform.EnsureOnMainThread(() => InjectJS(script));
        }

        private void InjectJS(string str)
        {
            WebView.LoadUrl(string.Format("javascript: {0}", str));
        }

        const string baseUrl = "file:///android_asset/";
        
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            WebView.Settings.JavaScriptEnabled = true;
            WebView.AddJavascriptInterface(this, "jsBridge");

            if (!string.IsNullOrEmpty(Html))
            {
                WebView.LoadDataWithBaseURL(baseUrl,
                    Html.Replace("{webview_base_url}", baseUrl),
                    "text/html", "utf-8", null);
            }
            else
            {
                if (Uri.Contains("://"))
                {
                    WebView.SetWebViewClient(new AwareWebClient(this));
                    WebView.LoadUrl(Uri);
                }
                else
                {
                    WebView.LoadUrl(string.Format(baseUrl + "{0}", Uri));
                }
            }
        }

        private void NotifyLoadComplete()
        {
            // TODO
            Entrance.SetIsBusy(false);
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(string data)
        {
            Callback?.Invoke(data);
        }
        
        protected virtual void OnSubUrlRequested(string req)
        {
            if (Entrance is IUrlEntrance iu)
            {
                OnEntranceRequested(iu.SubUrlRequested(req));
            }
        }

        protected virtual void OnEntranceRequested(IWebEntrance ent)
        {
            Navigation.PushAsync(typeof(WebViewPage), ent);
        }

        private class AwareWebClient : WebViewClient
        {
            WeakReference<WebViewPage> inner;

            public AwareWebClient(WebViewPage view)
            {
                inner = new WeakReference<WebViewPage>(view);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                if (inner.TryGetTarget(out var target))
                {
                    target.NotifyLoadComplete();
                }
            }

            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                if (request.IsRedirect) return false;

                if (inner.TryGetTarget(out var target))
                {
                    target.OnSubUrlRequested(request.Url.ToString());
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}