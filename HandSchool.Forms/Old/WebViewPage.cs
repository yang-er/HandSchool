using HandSchool.Internal;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Text;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class WebViewPage : ViewPage
	{
        private IWebEntrance InfoEntrance { get; }

        public HybridWebView WebView { get; }

		public WebViewPage(IWebEntrance entrance)
		{
            Content = WebView = new HybridWebView();
            On<_Each_>().ShowLoading();
            var meta = entrance.GetType().Get<EntranceAttribute>();
            Title = meta.Title;

            InfoEntrance = entrance;
            var baseController = InfoEntrance as BaseController;
            ViewModel = baseController;

            if (entrance is IInfoEntrance ie)
            {
                var sb = new StringBuilder();
                ie.HtmlDocument.ToHtml(sb);
                WebView.Html = sb.ToString();
            }
            else if (entrance is IUrlEntrance iu)
            {
                WebView.Uri = iu.HtmlUrl;
                WebView.SubUrlRequested += OnSubUrlRequested;

                if (WebView.Uri.Contains("://"))
                {
                    baseController.SetIsBusy(true);
                    WebView.LoadCompleted += () => baseController.SetIsBusy(false);
                }
            }

            foreach (var key in InfoEntrance.Menu)
            {
                ToolbarItems.Add(new ToolbarItem
                {
                    Text = key.Name,
                    Command = key.Command,
                    CommandParameter = (Action<IWebEntrance>)OnEntranceRequested
                });
            }

            entrance.Evaluate = WebView.JavaScript;
            WebView.RegisterAction(entrance.Receive);
        }

        protected virtual void OnSubUrlRequested(string req)
        {
            if (InfoEntrance is IUrlEntrance iu)
            {
                OnEntranceRequested(iu.SubUrlRequested(req));
            }
        }

        protected virtual void OnEntranceRequested(IWebEntrance ent)
        {
            //Navigation.PushAsync(new WebViewPage(ent));
        }
    }
}
