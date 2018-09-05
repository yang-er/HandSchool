using Android.Content;
using Android.Webkit;
using HandSchool.Droid;
using HandSchool.Views;
using Java.Interop;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AWebView = Android.Webkit.WebView;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace HandSchool.Droid
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, AWebView>
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";
        Context _context;

        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var webView = new AWebView(_context);
                webView.Settings.JavaScriptEnabled = true;
                SetNativeControl(webView);
            }
            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.Cleanup();
            }
            if (e.NewElement != null)
            {
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                if (Element.Html != string.Empty&& Element.Html!=null)
                {
                    Control.LoadDataWithBaseURL("file:///android_asset/", Element.Html.Replace("{webview_base_url}", "file:///android_asset/"), "text/html", "utf-8", null);
                }
                else
                {
                    Control.LoadUrl(string.Format("file:///android_asset/{0}", Element.Uri));
                }

                // InjectJS(JavaScriptFunction);
                Element.JavaScriptRequested += EvalJS;
            }
        }

        void InjectJS(string script)
        {
            if (Control != null)
            {
                Control.LoadUrl(string.Format("javascript: {0}", script));
            }
        }

        void EvalJS(string script)
        {
            Device.BeginInvokeOnMainThread(() => InjectJS(script));
        }
    }

    public class JSBridge : Java.Lang.Object
    {
        readonly WeakReference<HybridWebViewRenderer> hybridWebViewRenderer;

        public JSBridge(HybridWebViewRenderer hybridRenderer)
        {
            hybridWebViewRenderer = new WeakReference<HybridWebViewRenderer>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(string data)
        {
            if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out HybridWebViewRenderer hybridRenderer))
            {
                hybridRenderer.Element.InvokeAction(data);
            }
        }
    }
}