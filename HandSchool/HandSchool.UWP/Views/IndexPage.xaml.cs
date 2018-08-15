using HandSchool.ViewModels;
using Windows.UI.Xaml;

namespace HandSchool.Views
{
    public sealed partial class IndexPage : ViewPage
    {
        public int TextSize=20;
        public int LineLenth = 500;

        public IndexPage()
        {
            InitializeComponent();
            ViewModel = IndexViewModel.Instance;
        }
    }
}
