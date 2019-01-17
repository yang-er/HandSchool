using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.UWP;
using HandSchool.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace HandSchool.Views
{
    public sealed partial class MainPage : Page
    {
        public CommandBar CommandBar { get; set; }
        public Grid HeaderAreaGrid { get; set; }
        private bool _isSettingsInvoked = false;
        
        private List<NavigationMenuItemImpl> NavMenuItems;
        
        public MainPage()
        {
            InitializeComponent();
            NavMenuItems = PlatformImpl.Instance.NavigationItems;
            NavMenuItems.ForEach((i) => NavigationView.MenuItems.Add(i.Value));
            new NavigateImpl(ContentFrame);
            ContentFrame.Navigate(typeof(IndexPage));
            IndexViewModel.Instance.RefreshCommand.Execute(null);
            Window.Current.SetTitleBar(titleBarBack);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
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
                if (!_isSettingsInvoked && !(ContentFrame.Content is SettingsPage))
                {
                    _isSettingsInvoked = true;
                    ContentFrame.Navigate(typeof(SettingsPage));
                    Task.Delay(1000).ContinueWith((task) => _isSettingsInvoked = false);
                }
            }
        }

        object currentNavigationParameter;

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            NavigationView.IsBackEnabled = ContentFrame.CanGoBack;
            currentNavigationParameter = e.Parameter;

            object selected = null;
            
            if (e.Content is SettingsPage)
            {
                selected = NavigationView.SettingsItem;
            }
            else if (e.Content is WebViewPage)
            {
                selected = NavMenuItems.Find((item) =>
                    item.PageType == typeof(InfoQueryPageF)
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
            HeaderAreaGrid.SetBinding(
                dp: MarginProperty,
                path: "DisplayMode",
                src: NavigationView,
                mode: BindingMode.OneWay,
                cvt: new NavigationViewStateConverter());
        }
    }
}
