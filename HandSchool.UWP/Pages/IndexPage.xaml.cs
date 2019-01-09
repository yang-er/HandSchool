using HandSchool.UWP;
using HandSchool.ViewModels;

namespace HandSchool.Views
{
    public sealed partial class IndexPage : ViewPage
    {
        public IndexPage()
        {
            InitializeComponent();
            ViewModel = IndexViewModel.Instance;
            GridView.ItemsSource = IndexPageDataAdapter.Instance.Collection;
        }
    }
}
