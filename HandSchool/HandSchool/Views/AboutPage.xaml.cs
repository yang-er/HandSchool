using System;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HandSchool.Internal.HtmlObject;
using HandSchool.ViewModels;
using HandSchool.Internal;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            BindingContext = AboutViewModel.Instance;
            AboutViewModel.Instance.BindingContext = new ViewResponse(this);
            var sb = new StringBuilder();
            AboutViewModel.Instance.HtmlDocument.ToHtml(sb);
            WebView.Html = sb.ToString();
            WebView.RegisterAction(AboutViewModel.Instance.Response);
        }
    }
}
