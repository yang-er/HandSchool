using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewGradePage : ViewObject
    {
        public NewGradePage()
        {
            InitializeComponent();
            ViewModel = GradePointViewModel.Instance;
        }

        private async void ShowDetail(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is IGradeItem iGi)) return;
            if (iGi is GPAItem) return;
            await GradePointViewModel.Instance.ShowGradeDetailAsync(iGi);
        }
    }
}