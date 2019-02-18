using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GradePointPage : ViewObject
    {
        public GradePointPage()
        {
            InitializeComponent();
            ViewModel = GradePointViewModel.Instance;

            if (Core.Platform.RuntimeName == "Android")
            {
                var ListView = Content as ListView;
                ListView.SeparatorVisibility = SeparatorVisibility.None;
                ListView.Header = new StackLayout { HeightRequest = 4 };
                ListView.Footer = new StackLayout { HeightRequest = 4 };
            }
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var iGi = e.Item as IGradeItem;
            if (iGi is GPAItem) return;
            await GradePointViewModel.Instance.ShowGradeDetailAsync(iGi);
        }
    }
}