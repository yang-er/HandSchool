using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YktHistoryPage : PopContentPage
    {
        public YktHistoryPage()
        {
            InitializeComponent();
            BindingContext = YktViewModel.Instance;
        }
    }
}