using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace HandSchool.UWP
{
    // Thanks to arcsinW
    public sealed partial class InfoQueryPage : ViewPage
    {
        public List<InfoEntranceGroup> DataContent => Core.App.InfoEntrances;

        public InfoQueryPage()
        {
            InitializeComponent();
            BindingContext = new BaseViewModel { Title = "信息查询" };

        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
