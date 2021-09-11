using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using HandSchool.JLU.ViewModels;
using HandSchool.ViewModels;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    public partial class XykIOS_UserInfo : ViewObject
    {

        public XykIOS_UserInfo()
        {
            ViewModel = YktViewModel.Instance;
            InitializeComponent();
        }

        async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            if (sender == null) return;
            await (ViewModel as YktViewModel)?.LoadTwoAsync();
        }
    }
}
