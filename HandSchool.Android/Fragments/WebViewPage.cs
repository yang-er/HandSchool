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
    [BindView(Resource.Layout.layout_webview)]
    public partial class WebViewPage : ViewFragment, IWebViewPage, INotifyPropertyChanged
    {
        const string injectJS = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";
        const string baseUrl = "file:///android_asset/";

        [BindView(Resource.Id.web_view)]
        public WebView WebView { get; set; }

        public BaseController Controller
        {
            get => ViewModel as BaseController;
            set => ViewModel = value;
        }

        public override bool IsBusy
        {
            get => Controller?.IsBusy ?? false;
            set => Controller.IsBusy = value;
        }
        
        public string Uri { get; set; }
        public string Html { get; set; }

        private void WeakBind(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(IsBusy))
                PropertyChanged?.Invoke(this, args);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override void SetNavigationArguments(object param)
        {
            Controller = param as BaseController;
            Controller.View = this;
            Controller.PropertyChanged += WeakBind;
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
                Controller.IsBusy = Uri.Contains("://");
            }

            foreach (var key in Controller.Menu)
            {
                AddToolbarEntry(key);
            }

            Controller.Evaluate = JavaScript;
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

        public override void SolveBindings()
        {
            base.SolveBindings();
            WebView.Settings.JavaScriptEnabled = true;
            WebView.AddJavascriptInterface(this, "jsBridge");

            if (!string.IsNullOrEmpty(Html))
            {
                var realHtml = Html.Replace("{webview_base_url}", baseUrl)
                                   .Replace("{invokeCSharpAction_script}", injectJS);
                WebView.LoadDataWithBaseURL(baseUrl, realHtml, "text/html", "utf-8", null);
            }
            else
            {
                if (Uri.Contains("://"))
                {
                    WebView.SetWebViewClient(new AwareWebClient(this));
                    WebView.LoadUrl(Uri);
                    Controller.IsBusy = true;
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
            Controller.IsBusy = false;
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(string data)
        {
            Controller?.Receive(data);
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
    }
}