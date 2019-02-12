using Android.Webkit;
using System;

namespace HandSchool.Views
{
    partial class WebViewPage
    {
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