using HandSchool.ViewModels;
using Xamarin.Forms;
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

            if (Core.Platform.RuntimeName == "Android")
            {
                Content.BackgroundColor = Color.FromRgb(241, 241, 241);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IndexViewModel.Instance.RefreshCommand.Execute(null);
        }
    }
}