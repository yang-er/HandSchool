using HandSchool.Internal;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Reflection;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EntAttr = HandSchool.Services.EntranceAttribute;

namespace HandSchool.Views
{
    /// <summary>
    /// 提供信息查询页面
    /// </summary>
    public sealed partial class WebViewPage : ViewPage
    {
        private IWebEntrance InfoEntrance { get; set; }

        public WebViewPage()
        {
            InitializeComponent();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.Assert(e.Parameter is IWebEntrance, "Error leading");
            InfoEntrance = e.Parameter as BaseController;

            var meta = InfoEntrance.GetType().GetCustomAttribute(typeof(EntAttr)) as EntAttr;
            ViewModel = e.Parameter as BaseController;
            ViewModel.Title = meta.Title;
            InfoEntrance.Evaluate = WebView.InvokeScript;
            InfoEntrance.View = ViewResponse;

            WebView.Register = InfoEntrance.Receive;
            foreach (var key in InfoEntrance.Menu)
            {
                PrimaryMenu.Add(new AppBarButton
                {
                    Label = key.Name,
                    Command = key.Command,
                    CommandParameter = (Action<IWebEntrance>)OnEntranceRequested,
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = key.Icon
                    }
                });
            }

            if (InfoEntrance is IInfoEntrance info)
            {
                var sb = new StringBuilder();
                info.HtmlDocument.ToHtml(sb);
                WebView.Html = sb.ToString();
            }
            else if (InfoEntrance is IUrlEntrance urle)
            {
                WebView.Url = urle.HtmlUrl;
                WebView.SubUrlRequested += OnSubUrlRequested;

                if (WebView.Url.Contains("://"))
                {
                    ViewModel.SetIsBusy(true);
                    WebView.LoadCompleted += () => ViewModel.SetIsBusy(false);
                }
            }
        }

        private void OnSubUrlRequested(string url)
        {
            if (InfoEntrance is IUrlEntrance iu)
            {
                OnEntranceRequested(iu.SubUrlRequested(url));
            }
        }

        private void OnEntranceRequested(IWebEntrance entrance)
        {
            Frame.Navigate(typeof(WebViewPage), entrance);
        }
    }
}
