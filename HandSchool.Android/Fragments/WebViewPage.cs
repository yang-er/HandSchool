using Android.OS;
using Android.Views;
using Android.Webkit;
using HandSchool.Droid;
using HandSchool.Internals;
using HandSchool.Services;
using HandSchool.ViewModels;
using Java.Interop;
using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace HandSchool.Views
{
    public class WebViewPage : ViewFragment, IWebViewPage, INotifyPropertyChanged
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

        public BaseController Controller
        {
            get => ViewModel as BaseController;
            set => ViewModel = value;
        }

        public override bool IsBusy
        {
            get => base.IsBusy;
            set
            {
                if (base.IsBusy != value)
                {
                    base.IsBusy = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBusy)));
                }
            }
        }

        private void WeakBind(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(IsBusy)) IsBusy = Controller.IsBusy;
        }

        public override void SetNavigationArguments(object param)
        {
            Controller = param as BaseController;
            Controller.View = this;
            var meta = Controller.GetType().Get<EntranceAttribute>();
            Title = meta.Title;
            
            if (Controller is IInfoEntrance ie)
            {
                var sb = new StringBuilder();
                ie.HtmlDocument.ToHtml(sb);
                Html = sb.ToString();
            }
            else if (Controller is IUrlEntrance iu)
            {
                Uri = iu.HtmlUrl;

                if (Uri.Contains("://"))
                {
                    Controller.IsBusy = true;
                }
            }

            foreach (var key in Controller.Menu)
            {
                AddToolbarEntry(key);
            }

            Controller.Evaluate = JavaScript;
            Callback = Controller.Receive;
            Controller.SubEntranceRequested += OnEntranceRequested;
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

        public event PropertyChangedEventHandler PropertyChanged;

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
                    IsBusy = Controller.IsBusy = true;
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
            IsBusy = Controller.IsBusy = false;
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(string data)
        {
            Callback?.Invoke(data);
        }
        
        protected virtual void OnSubUrlRequested(string req)
        {
            if (Controller is IUrlEntrance iu)
            {
                OnEntranceRequested(iu.SubUrlRequested(req));
            }
        }

        protected virtual void OnEntranceRequested(IWebEntrance ent)
        {
            Navigation.PushAsync<WebViewPage>(ent);
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