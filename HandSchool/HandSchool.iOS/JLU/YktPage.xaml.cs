using HandSchool.Internal;
using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class YktPage : PopContentPage
	{
        private bool FirstOpen = true;

		public YktPage()
		{
			InitializeComponent();
            ViewModel = YktViewModel.Instance;
		}

        private void ChargeRequested(object sender, EventArgs e)
        {
            YktViewModel.Instance.ChargeCreditCommand.Execute(ChargeCreditBox.Text);
        }

        private void SetLostRequested(object sender, EventArgs e)
        {
            YktViewModel.Instance.SetUpLostStateCommand.Execute(null);
        }

        private async void TabbedPage_Appearing(object sender, EventArgs e)
        {
            if (!FirstOpen) return;

            FirstOpen = false;
            await Loader.Ykt.RequestLogin();
        }

        private async void PickCardInfoRequested(object sender, EventArgs e)
        {
            var page = new YktPickCardPage();
            var task = page.ShowAsync(Navigation);
            if (YktViewModel.Instance.PickCardInfo.Count == 0)
                await YktViewModel.Instance.GetPickCardInfo();
            await task;
        }

        private async void HistoryInfoRequested(object sender, EventArgs e)
        {
            var page = new YktHistoryPage();
            var task = page.ShowAsync(Navigation);
            if (YktViewModel.Instance.RecordInfo.Count == 0)
                await YktViewModel.Instance.ProcessQuery();
            await task;
        }
    }
}