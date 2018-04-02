using HandSchool.UWP;
using HandSchool.Views;
using System;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace HandSchool.UWP
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WebView>
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){window.external.notify(data);}";

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            if (e.NewElement is null && !(Control is null))
            {
                Control.NavigationCompleted -= OnWebViewNavigationCompleted;
                Control.ScriptNotify -= OnWebViewScriptNotify;
                Element.OnExcuteJavaScript -= InvokeScript;
            }

            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new WebView());
                    Control.NavigationCompleted += OnWebViewNavigationCompleted;
                    Control.ScriptNotify += OnWebViewScriptNotify;
                    Element.OnExcuteJavaScript += InvokeScript;
                }

                if (Element.Html != string.Empty)
                {
                    Control.NavigateToString(Element.Html.Replace("{webview_base_url}", "ms-appx-web:///WebWrapper//"));
                }
                else
                {
                    Control.Source = new Uri(string.Format("ms-appx-web:///WebWrapper//{0}", Element.Uri));
                }
            }
        }
        
        async void InvokeScript(string eval)
        {
            await Control.InvokeScriptAsync("eval", new[] { eval });
        }

        async void OnWebViewNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                // Inject JS script
                await Control.InvokeScriptAsync("eval", new[] { JavaScriptFunction });
            }
        }

        void OnWebViewScriptNotify(object sender, NotifyEventArgs e)
        {
            Element.InvokeAction(e.Value);
        }
    }
}