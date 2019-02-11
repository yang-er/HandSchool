using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.UWP;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using Windows.Phone.UI.Input;

namespace HandSchool.Views
{
    public sealed partial class MainPage : Page
    {
        private bool _isSettingsInvoked = false;
        
        private List<NavigationMenuItemImpl> NavMenuItems;

        private static bool winUIresAdded;
        
        public MainPage()
        {
            if (!winUIresAdded)
            {
                App.Current.Resources.MergedDictionaries.Add(new XamlControlsResources());
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("ms-appx:///Compact.xaml") });
                winUIresAdded = true;
            }

            InitializeComponent();
            NavMenuItems = PlatformImpl.Instance.NavigationItems;
            NavMenuItems.ForEach((i) => NavigationView.MenuItems.Add(i.Value));
            new NavigateImpl(ContentFrame);
            ContentFrame.Navigate(typeof(IndexPage));
            IndexViewModel.Instance.RefreshCommand.Execute(null);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
                MinimalMargin = new Thickness(52, -18, 0, -12);
                HeaderAreaGrid.FontSize = 16;
                ContentFrame.Margin = new Thickness(0);
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Disabled;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
                e.Handled = true;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is NavigationMenuItemImpl item)
            {
                if (item.PageType == null) Core.Logger.WriteLine("UWP", item.Title + " is not finished");
                if (item.PageType != ContentFrame.CurrentSourcePageType || item.NavigationParameter != currentNavigationParameter)
                    ContentFrame.Navigate(item.PageType, item.NavigationParameter);
            }
            else if (args.IsSettingsInvoked)
            {
                if (SettingItem is null)
                {
                    var wtf = NavigationView.SettingsItem as NavigationViewItem;
                    SettingItem = NavigationMenuItemImpl.CreateSettingItem(wtf);
                    wtf.Content = SettingItem;
                    NavMenuItems.Add(SettingItem);
                }
                
                if (!_isSettingsInvoked && currentNavigationParameter != SettingItem.NavigationParameter)
                {
                    _isSettingsInvoked = true;
                    ContentFrame.Navigate(SettingItem.PageType, SettingItem.NavigationParameter);
                    Task.Delay(1000).ContinueWith((task) => _isSettingsInvoked = false);
                }
            }
        }

        NavigationMenuItemImpl SettingItem;

        object currentNavigationParameter;

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ContentFrame.CanGoBack ? AppViewBackButtonVisibility.Visible
                                       : AppViewBackButtonVisibility.Disabled;
            currentNavigationParameter = e.Parameter;

            object selected = null;
            
            if (e.Content is WebViewPage)
            {
                selected = NavMenuItems.Find((item) =>
                    item.Title == "信息查询"
                )?.Value;
            }
            else if (e.Content is MessageDetailPage)
            {
                selected = NavigationView.SelectedItem;
            }
            else
            {
                selected = NavMenuItems.Find((item) =>
                    item.PageType == e.SourcePageType &&
                        item.NavigationParameter == e.Parameter
                )?.Value;
            }

            if (NavigationView.SelectedItem != selected)
                NavigationView.SelectedItem = selected;
        }
        
        private void ContentFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            CommandBar?.PrimaryCommands.Clear();
            CommandBar?.SecondaryCommands.Clear();
        }
        
        private void OnBackRequested(object sender, BackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack) ContentFrame.GoBack();
        }

        Thickness MinimalMargin { get; set; } = new Thickness(52, -12, 0, -12);
        Thickness OtherMargin { get; set; } = new Thickness(12, -12, 0, -12);

        private void NavigationView_DisplayModeChanged(NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            HeaderAreaGrid.Margin = sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal ? MinimalMargin : OtherMargin;
        }
    }
}
