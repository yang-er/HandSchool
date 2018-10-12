using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NavDataItem = HandSchool.Models.MasterPageItem;

namespace HandSchool.Views
{
    public sealed partial class MainPage : Page
    {
        public CommandBar CommandBar { get; set; }
        public Grid HeaderAreaGrid { get; set; }
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
            Window.Current.SetTitleBar(titleBarBack);
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
                    Core.Log(item.Title + "is not finished");
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

        private void HeaderAreaGrid_Loaded(object sender, RoutedEventArgs e)
        {
            HeaderAreaGrid = sender as Grid;
            HeaderAreaGrid.SetBinding(MarginProperty, new Binding
            {
                Source = NavigationView,
                Path = new PropertyPath("DisplayMode"),
                Converter = new NavigationViewStateConverter(),
                Mode = BindingMode.OneWay
            });
        }

        class NavigationViewStateConverter : IValueConverter
        {
            Thickness MinimalMargin { get; } = new Thickness(-72, 28, 0, -8);
            Thickness OtherMargin { get; } = new Thickness(12, 28, 0, -8);

            public object Convert(object value, Type targetType, object parameter, string language)
            {
                if (value is NavigationViewDisplayMode _value)
                {
                    return _value == NavigationViewDisplayMode.Minimal ? MinimalMargin : OtherMargin;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
