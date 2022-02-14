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
            //charge.Command = YktViewModel.Instance.ChargeCreditCommand;
            //lokc.Command = YktViewModel.Instance.SetUpLostStateCommand;
           // unlock.Command = YktViewModel.Instance.CancelLostStateCommand;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_pageCount != 0) return;
            Task.Run(YktViewModel.Instance.FirstOpen);
            _pageCount = 1;
        }

        //点击之后啥也不干，就是玩
        void ItemTappedHandler(object sender, CollectionItemTappedEventArgs args)
        {
            return;
        }

        private async void LoadMoreInfo(System.Object sender, System.EventArgs e)
        {
            if (sender == null) return;
            await Navigation.PushAsync(typeof(XykIosMoreInfo), null);
        }
    }
}