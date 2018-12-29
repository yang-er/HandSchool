using HandSchool.Internal;
using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using Windows.UI.Xaml;

namespace HandSchool.JLU.Views
{
    public sealed partial class YktPage : ViewPage
    {
        public YktPage()
        {
            InitializeComponent();
            ViewModel = YktViewModel.Instance;
        }

        private async void ViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (await Loader.Ykt.RequestLogin())
            {
                await YktViewModel.Instance.GetPickCardInfo();
                await YktViewModel.Instance.ProcessQuery();
            }
        }

        private void ChargeRequested(object sender, RoutedEventArgs e)
        {
            YktViewModel.Instance.ChargeCreditCommand.Execute(ChargeCreditBox.Text);
        }
    }
}
