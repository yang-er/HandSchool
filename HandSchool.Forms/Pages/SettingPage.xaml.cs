using HandSchool.Internal;
using HandSchool.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ViewPage
    {
        public SettingPage()
        {
            InitializeComponent();
            ViewModel = SettingViewModel.Instance;

#if __ANDROID__
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "关于",
                Command = new CommandAction(async () => await (new AboutPage()).ShowAsync(Navigation))
            });
#endif
        }

        public void ListView_ItemSelected(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }
    }
}
