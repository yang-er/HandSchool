using HandSchool.Internals;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Text;
using WebKit;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class WebViewPage : ViewObject, IWebViewPage
	{
        public HybridWebView WebView { get; }
        
        public BaseController Controller { get; set; }

        public WebViewPage()
        {
            Content = WebView = new HybridWebView();
        }

        public override void SetNavigationArguments(object param)
        {
            Controller = param as BaseController;
            Title = Controller.GetType().Get<EntranceAttribute>().Title;
            
            if (Controller is IInfoEntrance ie)
            {
                var sb = new StringBuilder();
                ie.HtmlDocument.ToHtml(sb);
                WebView.Html = sb.ToString();
            }
            else if (Controller is IUrlEntrance iu)
            {
                WebView.Uri = iu.HtmlUrl;
                WebView.SubUrlRequested += OnSubUrlRequested;

                if (WebView.Uri.Contains("://"))
                {
                    Controller.IsBusy = true;
                    WebView.LoadCompleted += () => Controller.IsBusy = false;
                }
            }

            foreach (var key in Controller.Menu)
            {
                ToolbarItems.Add(new ToolbarItem
                {
                    Text = key.Title,
                    Command = key.Command
                });
            }

            Controller.Evaluate = WebView.JavaScript;
            WebView.RegisterAction(Controller.Receive);
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