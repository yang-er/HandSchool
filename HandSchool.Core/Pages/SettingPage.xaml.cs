using HandSchool.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ViewObject
    {
        public SettingPage()
        {
            InitializeComponent();
            ViewModel = SettingViewModel.Instance;
        }
        
        private void ListView_ItemSelected(object sender, EventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}