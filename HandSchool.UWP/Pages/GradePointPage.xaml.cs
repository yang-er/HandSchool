using HandSchool.Models;
using HandSchool.ViewModels;
using Windows.UI.Xaml.Controls;

namespace HandSchool.Views
{
    public sealed partial class GradePointPage : ViewPage
    {
        public GradePointPage()
        {
            InitializeComponent();
            ViewModel = GradePointViewModel.Instance;
        }
        
        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is IGradeItem iGi)
            {
                await GradePointViewModel.Instance.ShowGradeDetailAsync(iGi);
            }
        }
    }
}
