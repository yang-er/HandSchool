using HandSchool.Internal;
using HandSchool.ViewModels;
using System.Text;
using Windows.UI.Xaml;

namespace HandSchool.Views
{
    public sealed partial class SettingsPage : ViewPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            ViewModel = SettingViewModel.Instance;
        }
    }
}
