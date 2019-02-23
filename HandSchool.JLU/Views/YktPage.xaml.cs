using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class YktPage : ViewObject
	{
		public YktPage()
		{
			InitializeComponent();
            ViewModel = YktViewModel.Instance;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(YktViewModel.Instance.FirstOpen);
        }
    }
}