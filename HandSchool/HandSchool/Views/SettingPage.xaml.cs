using HandSchool.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : PopContentPage
    {
        public SettingPage()
        {
            InitializeComponent();
            ViewModel = SettingViewModel.Instance;
        }

        public void ListView_ItemSelected(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await (new WebViewPage(AboutViewModel.Instance)).ShowAsync(Navigation);
        }
    }
}
