using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using System.Threading.Tasks;
using HandSchool.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class XykDroid : ViewObject
	{
		public XykDroid()
		{
			InitializeComponent();
            charge.Source = "money.png";
            lokc.Source = "lock.png";
            unlock.Source = "unlock.png";
            ViewModel = YktViewModel.Instance;
            charge.Command = YktViewModel.Instance.ChargeCreditCommand;
            lokc.Command = YktViewModel.Instance.SetUpLostStateCommand;
            unlock.Command = YktViewModel.Instance.CancelLostStateCommand;
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(YktViewModel.Instance.FirstOpen);
        }
    }
}