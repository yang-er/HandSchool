using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NavDataItem = HandSchool.Models.MasterPageItem;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace HandSchool.UWP
{
    public sealed partial class TestMainPage : Page
    {
        public CommandBar CommandBar { get; set; }
        private bool _isSettingsInvoked = false;

        public TestMainPage()
        {
            InitializeComponent();
            
            Core.Initialize();
            NavigationViewModel.Instance.PrimaryItems.ForEach((item) => NavigationView.MenuItems.Add(new NavigationViewItem { Icon = new FontIcon { FontFamily = new FontFamily("Segoe MDL2 Assets"), Glyph = item.Icon }, Content = item }));
            SystemNavigationManager.GetForCurrentView().BackRequested += ContentFrame_BackRequested;
            ContentFrame.Navigate(typeof(IndexPage));
        }
        
        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is NavDataItem item)
            {
                if (item.DestinationPageType != null && item.DestinationPageType != ContentFrame.CurrentSourcePageType)
                    ContentFrame.Navigate(item.DestinationPageType);
                else
                    System.Diagnostics.Debug.WriteLine(item.Title + "is not finished");
            }
            else if (args.IsSettingsInvoked)
            {
                if (!_isSettingsInvoked && !((ContentFrame.Content as ViewPage).BindingContext is AboutViewModel))
                {
                    _isSettingsInvoked = true;
                    ContentFrame.Navigate(typeof(WebViewPage), AboutViewModel.Instance);
                    Task.Run(async()=> { await Task.Delay(1000); _isSettingsInvoked = false; });
                }
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = ContentFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            
            var selected = NavigationView.MenuItems.FirstOrDefault((obj) => ((obj as NavigationViewItem).Content as NavDataItem).DestinationPageType == e.SourcePageType);
            if (selected is null)
            {
                if (e.SourcePageType == typeof(WebViewPage) && !((ContentFrame.Content as ViewPage).BindingContext is AboutViewModel))
                {
                    selected = NavigationView.MenuItems.FirstOrDefault((obj) => ((obj as NavigationViewItem).Content as NavDataItem).DestinationPageType == typeof(InfoQueryPage));
                }
                else
                {
                    selected = NavigationView.SettingsItem;
                }
            }
            NavigationView.SelectedItem = selected;
        }

        private void ContentFrame_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (ContentFrame == null)
                return;
            if (ContentFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                ContentFrame.GoBack();
            }
        }

        private void CommandBar_Loaded(object sender, RoutedEventArgs e)
        {
            CommandBar = sender as CommandBar;
        }

        private void ContentFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            CommandBar?.PrimaryCommands.Clear();
            CommandBar?.SecondaryCommands.Clear();
        }
    }
}
