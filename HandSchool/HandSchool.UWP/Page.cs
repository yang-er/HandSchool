using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPage = Windows.UI.Xaml.Controls.Page;
using BaseViewModel = HandSchool.ViewModels.BaseViewModel;

namespace HandSchool.UWP
{
    public class ViewPage : WPage
    {
        public BaseViewModel BindingContext
        {
            get => DataContext as BaseViewModel;
            set => DataContext = value;
        }

    }
}
