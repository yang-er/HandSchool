using HandSchool.Internal;
using HandSchool.UWP;
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
            Loaded += (sender, e) => OnPageLoaded(e);
            viewWrapper = new Lazy<XView>(() => new NativeViewWrapper(this));
        }

        public BaseViewModel ViewModel
        {
            get => DataContext as BaseViewModel;
            set => DataContext = value;
        }
        
        public List<AppBarButton> PrimaryMenu { get; set; } = new List<AppBarButton>();

        public List<AppBarButton> SecondaryMenu { get; set; } = new List<AppBarButton>();

        protected virtual void OnPageLoaded(RoutedEventArgs args)
        {
            if (Window.Current.Content is Frame frame)
            {
                if (frame.Content is MainPage mainpg)
                {
                    OnPageLoaded(args, mainpg);
                }
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

        readonly Lazy<XView> viewWrapper;

        XView IViewPage.Content
        {
            get => viewWrapper.Value;
            set => throw new InvalidOperationException();
        }

        string IViewPage.Title
        {
            get => ViewModel.Title;
            set => this.WriteLog("Title change requested but ignored");
        }

        public event EventHandler Disappearing;

        public event EventHandler Appearing;

        public void AddToolbarEntry(MenuEntry item)
        {
            var btn = new AppBarButton
            {
                Command = item.Command,
                Label = item.Title,
                Icon = new FontIcon
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Glyph = item.UWPIcon
                },
            };

            if (item.Command is null && !string.IsNullOrEmpty(item.CommandBinding))
            {
                btn.SetBinding(AppBarButton.CommandProperty, item.CommandBinding);
            }

            if (item.Order == Xamarin.Forms.ToolbarItemOrder.Primary)
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
                Disappearing?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
                Appearing?.Invoke(this, EventArgs.Empty);
        }

        public virtual void RegisterNavigation(INavigate navigate) => Navigation = navigate;

        public INavigate Navigation { get; private set; }

        public virtual Task ShowAsync(INavigate parent = null) => throw new InvalidOperationException();

        public virtual Task CloseAsync() => Navigation.PopAsync();

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

        #endregion
    }
}
