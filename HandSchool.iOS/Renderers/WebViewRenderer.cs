using Foundation;
using HandSchool.Controls;
using HandSchool.iOS.Renderers;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HSWebView), typeof(WebViewRenderer2))]

namespace HandSchool.iOS.Renderers
{
    public class WebViewRenderer2 : WkWebViewRenderer, IWKScriptMessageHandler
    {
        private const string NativeInvoker =
            "if(typeof(invokeNativeAction) == 'undefined') { var invokeNativeAction = function(data) { window.webkit.messageHandlers.invokeAction.postMessage(data); }; }";
        
        public WebViewRenderer2(WKWebViewConfiguration configuration) : base(configuration)
        {
        }

        public WebViewRenderer2()
        {
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement is HSWebView)
            {
                Configuration.UserContentController.RemoveScriptMessageHandler("invokeAction");
                Configuration.UserContentController.RemoveAllUserScripts();
            }

            base.OnElementChanged(e);
            if (e.NewElement is HSWebView)
            {
                var script = new WKUserScript(new NSString(NativeInvoker), WKUserScriptInjectionTime.AtDocumentStart, false);
                Configuration.UserContentController.AddUserScript(script);
                Configuration.UserContentController.AddScriptMessageHandler(this, "invokeAction");
            }
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            if (message.Name == "invokeAction")
            {
                (Element as HSWebView)?.SendReceivingJsData((NSString) message.Body);
            }
        }
    }
}