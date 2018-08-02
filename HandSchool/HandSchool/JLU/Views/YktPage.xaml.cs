using HandSchool.Internal;
using HandSchool.JLU.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class YktPage : TabbedPage
	{
        private bool FirstOpen = true;

		public YktPage()
		{
			InitializeComponent();
            BindingContext = YktViewModel.Instance;
            YktViewModel.Instance.View = new ViewResponse(this);
		}

        private void ChargeRequested(object sender, EventArgs e)
        {
            YktViewModel.Instance.ChargeCreditCommand.Execute(ChargeCreditBox.Text);
        }

        private async void TabbedPage_Appearing(object sender, EventArgs e)
        {
            if (!FirstOpen) return;

            FirstOpen = false;
            if (await Loader.Ykt.RequestLogin())
            {
                await YktViewModel.Instance.GetPickCardInfo();
                await YktViewModel.Instance.ProcessQuery();
            }
        }
    }
}