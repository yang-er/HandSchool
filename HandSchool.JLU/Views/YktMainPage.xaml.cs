using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YktMainPage : ViewObject
    {
        public YktMainPage()
        {
            InitializeComponent();
            ViewModel = YktViewModel.Instance;

            if (Core.Platform.RuntimeName == "iOS")
            {
                // ((ListView)Content).Intent = Settings
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(YktViewModel.Instance.FirstOpen);
        }
    }
}