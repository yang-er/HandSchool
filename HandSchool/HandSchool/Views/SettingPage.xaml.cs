using System;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HandSchool.Internal.HtmlObject;
using HandSchool.ViewModels;
using HandSchool.Internal;
using HandSchool.Models;
using System.Diagnostics;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {

        public SettingPage()
        {
            InitializeComponent();
            BindingContext = SettingViewModel.Instance;
        }
        public void itemseleted(object sender, EventArgs e)//unseleted the fucking item
        {
            ListView lv = sender as ListView;
            lv.SelectedItem = null;
        }
    }
}
