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

#if __ANDROID__
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "关于",
                Command = new Command(async () => await (new AboutPage()).ShowAsync(Navigation))
            });
#endif
        }

        public void ListView_ItemSelected(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Core.App.Service.AutoLogin = false;
            Core.App.Service.RequestLogin();
        }
    }
}
