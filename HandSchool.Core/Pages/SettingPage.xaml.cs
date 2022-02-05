using HandSchool.Models;
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

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (Device.RuntimePlatform != Device.UWP)
            {
                var sw = e.Item as SettingWrapper;
                sw?.ExcuteAction?.Execute(null);
            }
        }
    }
}