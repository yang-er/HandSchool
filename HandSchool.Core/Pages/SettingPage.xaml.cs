using HandSchool.Internal;
using HandSchool.ViewModels;
using System;
using System.Threading.Tasks;
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

            if (Core.Platform.RuntimeName == "Android")
            {
                ToolbarMenu.Add(new MenuEntry
                {
                    Title = "关于",
                    Command = new CommandAction(ShowAboutPage),
                    UWPIcon = "\uE74C"
                });
            }
        }

        private async Task ShowAboutPage()
        {
            await Navigation.PushAsync("AboutPage", null);
        }

        private void ListView_ItemSelected(object sender, EventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}