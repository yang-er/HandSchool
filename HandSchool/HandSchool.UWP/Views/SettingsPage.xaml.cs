using HandSchool.ViewModels;
using System.Text;

namespace HandSchool.UWP
{
    public sealed partial class SettingsPage : ViewPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = SettingViewModel.Instance;

            AboutWebView.DataContext = AboutViewModel.Instance;
            AboutViewModel.Instance.BindingContext = new ViewResponse(this);
            var sb = new StringBuilder();
            AboutViewModel.Instance.HtmlDocument.ToHtml(sb);
            AboutWebView.Html = sb.ToString();
            AboutWebView.Register = AboutViewModel.Instance.Response;
        }
    }
}
