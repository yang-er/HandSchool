using HandSchool.Internal;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BaseViewModel = HandSchool.ViewModels.BaseViewModel;
using WPage = Windows.UI.Xaml.Controls.Page;

namespace HandSchool.Views
{
    public class ViewPage : WPage
    {
        protected ViewResponse ViewResponse { get; }

        public ViewPage() : base()
        {
            Loaded += (sender, e) => OnPageLoaded(e);
            ViewResponse = new ViewResponse(this);
        }

        public BaseViewModel ViewModel
        {
            get => DataContext as BaseViewModel;
            set=> DataContext = value;
        }
        
        public List<AppBarButton> PrimaryMenu { get; set; } = new List<AppBarButton>();
        public List<AppBarButton> SecondaryMenu { get; set; } = new List<AppBarButton>();

        protected virtual void OnPageLoaded(RoutedEventArgs args)
        {
            if (Window.Current.Content is Frame frame)
            {
                if (frame.Content is MainPage mainpg)
                {
                    mainpg.DataContext = DataContext;
                    ViewModel.View = ViewResponse;

                    if (mainpg.CommandBar != null)
                    {
                        mainpg.CommandBar.SecondaryCommands.Clear();
                        SecondaryMenu.ForEach((obj) => mainpg.CommandBar.SecondaryCommands.Add(obj));
                        mainpg.CommandBar.PrimaryCommands.Clear();
                        PrimaryMenu.ForEach((obj) => mainpg.CommandBar.PrimaryCommands.Add(obj));
                    }
                }
            }
        }
    }
}
