using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NavDataItem = HandSchool.Models.MasterPageItem;

namespace HandSchool.UWP
{
    public sealed partial class MainPage : Page
    {
        public CommandBar CommandBar { get; set; }
        private bool _isSettingsInvoked = false;

        private List<NavigationViewItem> NavMenuItems;

        public MainPage()
        {
            InitializeComponent();
            
            NavMenuItems = (
                from item in NavigationViewModel.Instance.PrimaryItems
                select new NavigationViewItem
                {
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = item.Icon
                    },
                    Content = item,
                    Tag = item.DestinationPageType
                }
            ).ToList();

            NavMenuItems.ForEach((i) => NavigationView.MenuItems.Add(i));
            
            ContentFrame.Navigate(typeof(IndexPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
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
                if (!_isSettingsInvoked && !(ContentFrame.Content is SettingsPage))
                {
                    _isSettingsInvoked = true;
                    ContentFrame.Navigate(typeof(SettingsPage));
                    Task.Run(async()=> { await Task.Delay(1000); _isSettingsInvoked = false; });
                }
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            NavigationView.IsBackEnabled = ContentFrame.CanGoBack;

            object selected = null;

            if (e.SourcePageType == typeof(SettingsPage))
            {
                selected = NavigationView.SettingsItem;
            }
            else if (e.SourcePageType == typeof(WebViewPage))
            {
                selected = NavMenuItems.Find((item) => item.Tag as Type == typeof(InfoQueryPage));
            }
            else if (e.Content is MessageDetailPage page)
            {
                selected = NavigationView.SelectedItem;
            }
            else
            {
                selected = NavMenuItems.Find((item) => item.Tag as Type == e.SourcePageType);
            }

            NavigationView.SelectedItem = selected;
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

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack) ContentFrame.GoBack();
        }
    }
}
