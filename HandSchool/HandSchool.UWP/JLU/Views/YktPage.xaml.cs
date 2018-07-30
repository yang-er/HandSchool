using HandSchool.JLU;
using HandSchool.JLU.ViewModels;
using HandSchool.UWP.Views;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
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

namespace HandSchool.UWP.JLU.Views
{
    public sealed partial class YktPage : ViewPage
    {
        public YktPage()
        {
            InitializeComponent();
            ViewModel = YktViewModel.Instance;
        }

        private async void ViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Loader.Ykt.IsLogin) await LoginViewModel.RequestAsync(Loader.Ykt);
        }
    }
}
