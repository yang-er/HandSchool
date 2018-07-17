using HandSchool.Services;
using HandSchool.ViewModels;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace HandSchool.UWP
{
    /// <summary>
    /// 提供信息查询页面
    /// </summary>
    public sealed partial class WebViewPage : ViewPage
    {
        private IInfoEntrance InfoEntrance { get; set; }

        public WebViewPage()
        {
            InitializeComponent();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.Assert(e.Parameter is IInfoEntrance, "Error leading");
            InfoEntrance = e.Parameter as IInfoEntrance;

            ViewModel = new BaseViewModel { Title = InfoEntrance.Name };
            InfoEntrance.Evaluate = WebView.InvokeScript;
            InfoEntrance.Binding = ViewResponse;
            var sb = new StringBuilder();
            InfoEntrance.HtmlDocument.ToHtml(sb);
            WebView.Html = sb.ToString();
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
        }
    }
}
