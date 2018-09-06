using Foundation;
using HandSchool.iOS;
using HandSchool.Views;
using System.IO;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace HandSchool.iOS
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>, IWKScriptMessageHandler
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";
        WKUserContentController userController;

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                userController = new WKUserContentController();
                var script = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
                userController.AddUserScript(script);
                userController.AddScriptMessageHandler(this, "invokeAction");

                var config = new WKWebViewConfiguration { UserContentController = userController };
                var webView = new WKWebView(Frame, config);
                SetNativeControl(webView);
            }

            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler("invokeAction");
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.Cleanup();
            }

            if (e.NewElement != null)
            {
                if (Element.Html == "" || Element.Html is null)
                {
                    if (Element.Uri.Contains("://"))
                    {
                        string fileName = Element.Uri;
                        Control.LoadRequest(new NSUrlRequest(new NSUrl(fileName)));
                        Control.NavigationDelegate = new NavigationDelegate();
                    }
                    else
                    {
                        string fileName = Path.Combine(NSBundle.MainBundle.BundlePath, "WebWrapper", Element.Uri);
                        Control.LoadRequest(new NSUrlRequest(new NSUrl(fileName, false)));
                    }
                }
                else
                {
                    string fileName = Path.Combine(NSBundle.MainBundle.BundlePath, "WebWrapper");
                    Control.LoadHtmlString(Element.Html, new NSUrl(fileName, true));
                }

                Element.JavaScriptRequested += (eval) => Control.EvaluateJavaScript(eval, null);
                // Element.JavaScriptRequested += (eval) => userController.AddUserScript(new WKUserScript(new NSString(eval), WKUserScriptInjectionTime.AtDocumentEnd, false));
            }
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            Element.InvokeAction(message.Body as NSString);
        }

        class NavigationDelegate : WKNavigationDelegate
        {
            // override 
        }
    }
}
