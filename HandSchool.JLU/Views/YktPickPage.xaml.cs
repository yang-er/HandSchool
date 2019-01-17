using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YktPickPage : ViewObject
    {
        public YktPickPage()
        {
            InitializeComponent();
            ViewModel = YktViewModel.Instance;
        }
    }
}