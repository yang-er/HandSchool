using HandSchool.ViewModels;
using HandSchool.Views;
using System;
using Windows.UI.Xaml;
using Xamarin.Forms.Xaml;

namespace HandSchool.UWP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPageView : ViewObject
    {
        public AboutPageView()
        {
            InitializeComponent();
            versionBox.Text = AboutViewModel.Instance.Version;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Core.Platform.OpenUrl(Core.Platform.StoreLink);
        }

        private void HyperlinkButton_Click(object sender, EventArgs e)
        {
            Core.Platform.OpenUrl("https://github.com/yang-er/HandSchool");
        }

        private void HyperlinkButton_Click2(object sender, EventArgs e)
        {
            Core.Platform.OpenUrl("https://github.com/yang-er/HandSchool/blob/new-2/PRIVACY.md");
        }

        private void HyperlinkButton_Click3(object sender, EventArgs e)
        {
            Core.Platform.OpenUrl("https://github.com/yang-er/HandSchool/blob/new-2/LICENSE");
        }
    }
}