using HandSchool.Internal;
using HandSchool.ViewModels;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IndexPage : ViewPage
	{
        public IndexPage()
        {
            InitializeComponent();
            ViewModel = IndexViewModel.Instance;
            this.On<iOS, ViewPage>().UseSafeArea().HideFrameShadow();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IndexViewModel.Instance.RefreshCommand.Execute(null);
        }
    }
}