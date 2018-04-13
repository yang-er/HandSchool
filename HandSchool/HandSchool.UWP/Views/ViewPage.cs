using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPage = Windows.UI.Xaml.Controls.Page;
using BaseViewModel = HandSchool.ViewModels.BaseViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace HandSchool.UWP
{
    public class ViewPage : WPage
    {
        public ViewPage() : base()
        {
            Loaded += (sender, e) => OnPageLoaded(e);
        }

        public BaseViewModel BindingContext
        {
            get => DataContext as BaseViewModel;
            set => DataContext = value;
        }

        public List<AppBarButton> PrimaryMenu { get; set; } = new List<AppBarButton>();
        public List<AppBarButton> SecondaryMenu { get; set; } = new List<AppBarButton>();

        protected virtual void OnPageLoaded(RoutedEventArgs args)
        {
            var mainpg = (Window.Current.Content as Frame).Content as TestMainPage;
            mainpg.DataContext = DataContext;
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
