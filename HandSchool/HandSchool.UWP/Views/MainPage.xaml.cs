using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace HandSchool.UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TestMainPage : Page
    {
        public static HandSchool.App app;
        public TestMainPage()
        {
            this.InitializeComponent();

            app = new HandSchool.App();
            HandSchool.Core.Initialize();
            HandSchool.ViewModels.NavigationViewModel.Instance.PrimaryItems.ForEach((item) => NavigationView.MenuItems.Add(new NavigationViewItem { Icon = new FontIcon { FontFamily = new FontFamily("Segoe MDL2 Assets"), Glyph = item.Icon }, Content = item }));
            //NavigationView.Content = new UWP.FeedPage();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationView.Content = new FeedPage();
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            
        }
    }
}
