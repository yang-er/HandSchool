using HandSchool.ViewModels;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IndexPage : ViewObject
	{
        public IndexPage()
        {
            InitializeComponent();
            ViewModel = IndexViewModel.Instance;
            //On<_iOS_>().UseSafeArea().HideFrameShadow();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IndexViewModel.Instance.RefreshCommand.Execute(null);
        }
    }
}