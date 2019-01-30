using HandSchool.Internal;
using HandSchool.UWP;
using Microcharts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Forms.Platform.UWP;
using BaseViewModel = HandSchool.ViewModels.BaseViewModel;
using WPage = Windows.UI.Xaml.Controls.Page;
using XView = Xamarin.Forms.View;

namespace HandSchool.Views
{
    public class ViewPage : WPage, IViewPage
    {
        public ViewPage() : base()
        {
            Loaded += OnPageLoaded;
        }

        public BaseViewModel ViewModel
        {
            get => DataContext as BaseViewModel;
            set => DataContext = value;
        }
        
        public List<AppBarButton> PrimaryMenu { get; set; } = new List<AppBarButton>();

        public List<AppBarButton> SecondaryMenu { get; set; } = new List<AppBarButton>();

        private void OnPageLoaded(object sender, RoutedEventArgs args)
        {
            if (Window.Current.Content is Frame frame && frame.Content is MainPage mainpg)
            {
                if (ViewModel is null) return;
                OnPageLoaded(args, mainpg);
            }
        }

        protected virtual void OnPageLoaded(RoutedEventArgs args, MainPage mainPage)
        {
            mainPage.DataContext = DataContext;
            ViewModel.View = this;
            
            if (mainPage.CommandBar != null)
            {
                mainPage.CommandBar.SecondaryCommands.Clear();
                SecondaryMenu.ForEach((obj) => mainPage.CommandBar.SecondaryCommands.Add(obj));
                mainPage.CommandBar.PrimaryCommands.Clear();
                PrimaryMenu.ForEach((obj) => mainPage.CommandBar.PrimaryCommands.Add(obj));
            }
        }

        #region IViewPage Impl

        public bool IsModal => false;
        
        XView IViewPage.Content { get; set; }

        string IViewPage.Title { get; set; }

        public event EventHandler Disappearing;

        public event EventHandler Appearing;

        public void AddToolbarEntry(MenuEntry item)
        {
            if (item.HiddenForPull) item.Title = "刷新";

            var icon = new FontIcon
            {
                FontFamily = new FontFamily("Segoe MDL2 Assets")
            };

            icon.SetBinding(FontIcon.GlyphProperty, "UWPIcon",
                item, Windows.UI.Xaml.Data.BindingMode.OneWay);
            
            var btn = new AppBarButton { Icon = icon };
            btn.SetBinding(AppBarButton.CommandProperty, "Command",
                item, Windows.UI.Xaml.Data.BindingMode.OneWay);
            btn.SetBinding(AppBarButton.LabelProperty, "Title",
                item, Windows.UI.Xaml.Data.BindingMode.OneWay);

            if (item.Order != Xamarin.Forms.ToolbarItemOrder.Secondary || item.HiddenForPull)
            {
                PrimaryMenu.Add(btn);
            }
            else
            {
                SecondaryMenu.Add(btn);
            }
        }

        #endregion

        #region INavigate Impl
        
        protected virtual void OnDisappearing()
        {
            Disappearing?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnAppearing()
        {
            Appearing?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            OnDisappearing();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            OnAppearing();
        }

        public virtual void RegisterNavigation(INavigate navigate) => Navigation = navigate;

        public INavigate Navigation { get; private set; }

        public virtual Task ShowAsync(INavigate parent = null) => throw new InvalidOperationException();

        public virtual Task CloseAsync() => throw new InvalidOperationException();

        #endregion

        #region IViewResponse Impl

        public Task RequestMessageAsync(string title, string message, string button)
        {
            return ViewResponseImpl.ShowMessageAsync1(title, message, button);
        }

        public Task<bool> RequestAnswerAsync(string title, string description, string cancel, string accept)
        {
            return ViewResponseImpl.ShowAskAsync1(title, description, cancel, accept);
        }

        public Task<string> RequestActionAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            return ViewResponseImpl.DisplayActionSheet(Frame, title, cancel, destruction, buttons);
        }

        public Task<string> RequestInputAsync(string title, string description, string cancel, string accept)
        {
            return ViewResponseImpl.ShowInputAsync1(title, description, cancel, accept);
        }

        public Task RequestChartAsync(Chart chart, string title = "", string close = "关闭")
        {
            return new ChartDialog(chart, title).ShowAsync().AsTask();
        }

        #endregion
    }
}
