using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GradePointPage : ViewPage
    {
        public GradePointPage()
        {
            InitializeComponent();
            ViewModel = GradePointViewModel.Instance;
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            var iGi = e.Item as IGradeItem;
            await GradePointViewModel.Instance.ShowGradeDetailAsync(iGi);
            // Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
