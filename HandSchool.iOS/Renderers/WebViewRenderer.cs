using System;
using System.Reflection;
using System.Threading.Tasks;
using Foundation;
using HandSchool.Controls;
using HandSchool.iOS.Renderers;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HSWebView), typeof(WebViewRenderer2))]

namespace HandSchool.iOS.Renderers
{
    public class WebViewRenderer2 : WkWebViewRenderer, IWKScriptMessageHandler
    {
        private const string NativeInvoker = "window.webkit.messageHandlers.invokeAction.postMessage";

        private static readonly MethodInfo EvalJsMethod;

        static WebViewRenderer2()
        {
            EvalJsMethod = typeof(WkWebViewRenderer).GetDeclaredMethod("OnEvaluateJavaScriptRequested", typeof(string));
        }

        public WebViewRenderer2(WKWebViewConfiguration configuration) : base(configuration)
        {
            if (EvalJsMethod is null)
            {
                throw new NotSupportedException("Program cannot work because of reflection error");
            }
        }

        public WebViewRenderer2()
        {
            if (EvalJsMethod is null)
            {
                throw new NotSupportedException("Program cannot work because of reflection error");
            }
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement is HSWebView oldElement)
            {
                Configuration.UserContentController.RemoveScriptMessageHandler("invokeAction");
                oldElement.EvaluateJavaScriptRequested -= EvaluateJsAsync;
            }

            base.OnElementChanged(e);
            if (e.NewElement is HSWebView newElement)
            {
                Configuration.UserContentController.AddScriptMessageHandler(this, "invokeAction");
                newElement.EvaluateJavaScriptRequested -=
                    (EvaluateJavaScriptDelegate) EvalJsMethod.CreateDelegate(typeof(EvaluateJavaScriptDelegate), this);
                newElement.EvaluateJavaScriptRequested += EvaluateJsAsync;
            }
        }

        private async Task<string> EvaluateJsAsync(string script)
        {
            var realScript = script.Replace(HSWebView.NativeMethodInvoker, NativeInvoker);
            var result = await EvaluateJavaScriptAsync(realScript);
            return result.ToString();
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