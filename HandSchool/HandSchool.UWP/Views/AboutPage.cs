using HandSchool.ViewModels;
using System.Text;

namespace HandSchool.UWP
{
    public sealed class AboutPage : WebViewPage
    {
        public AboutPage()
        {
            BindingContext = AboutViewModel.Instance;
            AboutViewModel.Instance.BindingContext = new ViewResponse(this);
            var sb = new StringBuilder();
            AboutViewModel.Instance.HtmlDocument.ToHtml(sb);
            Html = sb.ToString();
            Register = AboutViewModel.Instance.Response;
        }
    }
}
