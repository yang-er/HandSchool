using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    public partial class XykIosMoreInfo : ViewObject
    {
        public XykIosMoreInfo()
        {
            ViewModel = YktViewModel.Instance;
            InitializeComponent();
        }
        private async void Refresh(object sender, System.EventArgs e)
        {
            if (sender == null) return;
            await ((YktViewModel)ViewModel).LoadTwoAsync();
        }
    }
}
