using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using System.Threading.Tasks;
using HandSchool.Controls;
using HandSchool.Internals;
using HandSchool.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class XykIos : ViewObject
    {
        private static int _pageCount;

        public XykIos()
        {
            InitializeComponent();
            ViewModel = YktViewModel.Instance;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_pageCount != 0) return;
            Task.Run(YktViewModel.Instance.FirstOpen);
            _pageCount = 1;
        }
        
        private async void LoadMoreInfo(System.Object sender, System.EventArgs e)
        {
            if (sender == null) return;
            await Navigation.PushAsync(typeof(XykIosMoreInfo), null);
        }
    }
}