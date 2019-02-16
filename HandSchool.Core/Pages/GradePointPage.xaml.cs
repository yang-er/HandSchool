using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GradePointPage : ViewObject
    {
        bool control = false;

        public GradePointPage()
        {
            InitializeComponent();
            ViewModel = GradePointViewModel.Instance;
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var iGi = e.Item as IGradeItem;
            if (iGi is GPAItem) return;
            await GradePointViewModel.Instance.ShowGradeDetailAsync(iGi);
        }
    }
}