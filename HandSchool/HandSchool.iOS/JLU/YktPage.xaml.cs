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
            await Loader.Ykt.RequestLogin();
        }
    }
}