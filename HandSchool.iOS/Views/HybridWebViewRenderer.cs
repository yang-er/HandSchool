using Foundation;
using HandSchool.iOS;
using HandSchool.Views;
using ObjCRuntime;
using System;
using System.IO;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Regex = System.Text.RegularExpressions.Regex;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace HandSchool.iOS
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>, IWKScriptMessageHandler
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";
        const string DomainPattern = @"(http|https)://(?<domain>[^(:|/]*)";
        WKUserContentController userController;
        WKHttpCookieStore cookieStore;

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
                cookieStore = config.WebsiteDataStore.HttpCookieStore;
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
                        Control.WeakNavigationDelegate = new NavigationDelegate(e.NewElement);
                        var urlReq = new NSMutableUrlRequest(new NSUrl(fileName));

                        if (Element.OpenWithPost != null)
                        {
                            urlReq.HttpMethod = "POST";
                            urlReq.Body = NSData.FromArray(Element.OpenWithPost);
                        }
                        else
                        {
                            urlReq.HttpMethod = "GET";
                        }

                        if (Element.Cookie != null)
                        {
                            var domain = Regex.Match(Element.Uri, DomainPattern).Groups["domain"].Value;

                            foreach (var cookieString in Element.Cookie)
                            {
                                var cookieSplit = cookieString.Split(new[] { '=' }, 2);
                                var cookieObj = new NSHttpCookie(cookieSplit[0], cookieSplit[1], "/", domain);
                                cookieStore.SetCookie(cookieObj, null);
                            }

                            cookieStore.GetAllCookies((obj) =>
                            {
                                foreach (var ob in obj)
                                {
                                    Core.Log(ob.ToString());
                                }
                            });

                            urlReq.ShouldHandleCookies = true;
                        }
                        
                        Control.LoadRequest(urlReq);
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
            WeakReference<HybridWebView> inner;

            public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
            {
                if (inner.TryGetTarget(out var target))
                {
                    target.NotifyLoadComplete();
                    Core.Log("Error: " + error.Description);
                }
            }

            public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
            {
                if (inner.TryGetTarget(out var target))
                {
                    if (webView.IsLoading == false) target.NotifyLoadComplete();
                }
            }

            public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
            {
                if (inner.TryGetTarget(out var target))
                {
                    target.NotifyLoadComplete();
                    Core.Log("Error: " + error.Description);
                }
            }

            public NavigationDelegate(HybridWebView target)
            {
                inner = new WeakReference<HybridWebView>(target);
            }

            public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
            {
                if (inner.TryGetTarget(out var target))
                {
                    var nav2 = navigationAction.Request.Url.AbsoluteString;
                    Core.Log(nav2);
                    if (navigationAction.NavigationType != WKNavigationType.LinkActivated)
                    {
                        decisionHandler(WKNavigationActionPolicy.Allow);
                    }
                    else
                    {
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        target.RaiseSubUrlRequest(nav2);
                    }
                }
                else
                {
                    decisionHandler(WKNavigationActionPolicy.Allow);
                }
            }
        }
    }
}
