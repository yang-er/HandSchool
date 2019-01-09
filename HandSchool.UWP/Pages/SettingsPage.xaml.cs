using HandSchool.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace HandSchool.Views
{
    public sealed partial class SettingsPage : ViewPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            ViewModel = SettingViewModel.Instance;
        }

        private void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Core.Platform.OpenUrl(Core.Platform.StoreLink);
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Core.Platform.OpenUrl("https://github.com/yang-er/HandSchool");
        }

        private void HyperlinkButton_Click2(object sender, RoutedEventArgs e)
        {
            Core.Platform.OpenUrl("https://github.com/yang-er/HandSchool/blob/master/PRIVACY.md");
        }

        private void HyperlinkButton_Click3(object sender, RoutedEventArgs e)
        {
            Core.Platform.OpenUrl("https://github.com/yang-er/HandSchool/blob/master/LICENSE");
        }
    }
}
