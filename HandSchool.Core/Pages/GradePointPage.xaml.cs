using HandSchool.Internal;
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

        private async void Handle_ItemTapped(object sender, System.EventArgs e)
        {
            var frame = sender as Controls.TextAtom;
            await frame.TappedAnimation(async () =>
            {
                var iGi = frame.BindingContext as IGradeItem;
                if (iGi is GPAItem) return;
                await GradePointViewModel.Instance.ShowGradeDetailAsync(iGi);
            });
            
        }
    }
}