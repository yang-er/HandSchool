using HandSchool.ViewModels;
using Windows.UI.Xaml;

namespace HandSchool.UWP
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await LoginViewModel.RequestAsync(Core.App.Service);
        }
    }
}
