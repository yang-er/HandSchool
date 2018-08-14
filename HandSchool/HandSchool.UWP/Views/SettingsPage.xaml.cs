using HandSchool.Internal;
using HandSchool.ViewModels;
using System.Text;
using Windows.UI.Xaml;

namespace HandSchool.Views
{
    public sealed partial class SettingsPage : ViewPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            ViewModel = SettingViewModel.Instance;

            AboutWebView.DataContext = AboutViewModel.Instance;
            var sb = new StringBuilder();
            AboutViewModel.Instance.HtmlDocument.ToHtml(sb);
            AboutWebView.Html = sb.ToString();
            AboutWebView.Register = AboutViewModel.Instance.Response;
        }
    }
}
