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
        }

        private async void PushAllScorePage(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(typeof(AllGradePage), null);
        }
        private async void Handle_ItemTapped(object sender, System.EventArgs e)
        {
            var iGi = (sender as BindableObject)?.BindingContext as IGradeItem;
            if (iGi is null) return;
            if (iGi is GPAItem) return;
            await GradePointViewModel.Instance.ShowGradeDetailAsync(iGi);
        }
    }
}