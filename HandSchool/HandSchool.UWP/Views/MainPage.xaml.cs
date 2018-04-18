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
    public sealed partial class TestMainPage : Page
    {
        public CommandBar CommandBar { get; set; }
        private bool _isSettingsInvoked = false;

        private List<NavigationViewItem> NavMenuItems;

        public TestMainPage()
        {
            InitializeComponent();
            
            Core.Initialize();
            
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
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = 
                ContentFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;

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
                if (page.Tag is FeedItem)
                {
                    selected = NavMenuItems.Find((item) => item.Tag as Type == typeof(FeedPage));
                }
                else if (page.Tag is IMessageItem)
                {
                    selected = NavMenuItems.Find((item) => item.Tag as Type == typeof(MessagePage));
                }
            }
            else
            {
                selected = NavMenuItems.Find((item) => item.Tag as Type == e.SourcePageType);
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
