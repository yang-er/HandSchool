using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Xamarin.Forms.Platform.UWP;
using ContentPage = Xamarin.Forms.ContentPage;
using Rectangle = Xamarin.Forms.Rectangle;

namespace HandSchool.Views
{
    internal sealed partial class TabbedPage : ViewPage
    {
        public TabbedPage()
        {
            InitializeComponent();
            Packagers = new List<ViewObject>();
        }

        private List<ViewObject> Packagers { get; }
        
        protected override void OnPageLoaded(RoutedEventArgs args, MainPage mainPage)
        {
            if (Packagers.Count == 0)
            {
                base.OnPageLoaded(args, mainPage);
            }
            else
            {
                ViewModel = Packagers[0].ViewModel;
                
                foreach (var packager in Packagers)
                {
                    packager.RegisterNavigation(Navigation);
                    foreach (var entry in packager.ToolbarMenu)
                    {
                        AddToolbarEntry(entry);
                    }
                }

                base.OnPageLoaded(args, mainPage);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is IViewPresenter presenter)
            {
                var pages = presenter.GetAllPages();

                foreach (ViewObject page in pages)
                {
                    Packagers.Add(page);
                    ProcessPackager(page);
                }
            }

            base.OnNavigatedTo(e);
        }

        private void ProcessPackager(ViewObject Packager)
        {
            Appearing += (s, e) => Packager.SendAppearing();
            Disappearing += (s, e) => Packager.SendDisappearing();
            
            var grid = new Grid
            {
                Children = { Packager.CreateFrameworkElement() },
                Tag = Packager,
            };

            var pi = new PivotItem
            {
                Header = Packager.Title,
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                Content = grid
            };
            
            Pivot.Items.Add(pi);
            grid.SizeChanged += FrameworkElement_SizeChanged;
        }

        private void FrameworkElement_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (sender is Grid element)
            {
                var intpage = element.Tag as ContentPage;
                var size = new Rectangle(0, 0, element.ActualWidth, element.ActualHeight);
                intpage.Layout(size);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            foreach (var packager in Packagers)
            {
                packager.SendDisappearing();
            }

            ViewModel.View = null;
            ViewModel = null;

            foreach (PivotItem item in Pivot.Items)
            {
                (item.Content as Grid).Children.Clear();
            }
            
            foreach (var packager in Packagers)
            {
                Platform.GetRenderer(packager).Dispose();
                Platform.SetRenderer(packager, null);
                packager.BindingContext = null;
            }
            
            Packagers.Clear();
            System.GC.Collect();
        }
    }
}
