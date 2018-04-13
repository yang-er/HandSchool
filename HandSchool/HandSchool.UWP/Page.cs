using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPage = Windows.UI.Xaml.Controls.Page;
using BaseViewModel = HandSchool.ViewModels.BaseViewModel;
using Windows.UI.Xaml.Controls;

namespace HandSchool.UWP
{
    public class ViewPage : WPage
    {
        public BaseViewModel BindingContext
        {
            get => DataContext as BaseViewModel;
            set => DataContext = value;
        }

        public List<AppBarButton> PrimaryMenu { get; set; } = new List<AppBarButton>();
        public List<AppBarButton> SecondaryMenu { get; set; } = new List<AppBarButton>();
    }
}
