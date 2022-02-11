using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class XykDroid : ViewObject
	{
		public XykDroid()
		{
			InitializeComponent();
            UserInfoFrame.SetDefaultFrameCornerRadius();
            QAndAFrame.SetDefaultFrameCornerRadius();
            ViewModel = YktViewModel.Instance;
            ChargeImg.Source = "money.png";
            LockImg.Source = "lock.png";
            UnlockImg.Source = "unlock.png";
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(YktViewModel.Instance.FirstOpen);
        }
    }
}