using HandSchool.Services;
using HandSchool.ViewModels;
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
            InfoEntrance = e.Parameter as IWebEntrance;

            var meta = InfoEntrance.GetType().GetCustomAttribute(typeof(EntAttr)) as EntAttr;
            ViewModel = new BaseViewModel { Title = meta.Title };
            InfoEntrance.Evaluate = WebView.InvokeScript;
            InfoEntrance.Binding = ViewResponse;

            WebView.Register = InfoEntrance.Receive;
            foreach (var key in InfoEntrance.Menu)
                PrimaryMenu.Add(new AppBarButton
                {
                    Label = key.Name,
                    Command = key.Command,
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = key.Icon
                    }
                });

            if (InfoEntrance is IInfoEntrance info)
            {
                var sb = new StringBuilder();
                info.HtmlDocument.ToHtml(sb);
                WebView.Html = sb.ToString();
            }
            else if (InfoEntrance is IUrlEntrance urle)
            {
                WebView.Url = urle.HtmlUrl;
            }
        }
    }
}
