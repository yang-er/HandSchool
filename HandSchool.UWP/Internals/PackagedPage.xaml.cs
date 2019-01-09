using HandSchool.Internal;
using System.Diagnostics;
using Windows.UI.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using WNavigationEventArgs = Windows.UI.Xaml.Navigation.NavigationEventArgs;

namespace HandSchool.Views
{
    internal sealed partial class PackagedPage : ViewPage
    {
        public PackagedPage()
        {
            InitializeComponent();
            SizeChanged += FrameworkElement_SizeChanged;
        }
        
        public ViewPackager Packager { get; private set; }

        public ContentPage InternalPage { get; set; }

        protected override void OnPageLoaded(RoutedEventArgs args, MainPage mainPage)
        {
            if (Packager is null)
            {
                base.OnPageLoaded(args, mainPage);
            }
            else
            {
                ViewModel = Packager.ViewModel;
                Packager.RegisterNavigation(Navigation);
                foreach (var entry in Packager.MenuEntries)
                    AddToolbarEntry(entry);
                base.OnPageLoaded(args, mainPage);
                Packager.Pushed.Start();
            }
        }
        
        protected override void OnNavigatedTo(WNavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is ViewPackager packager)
            {
                Packager = packager;
                ProcessPackager();
            }
            else if (e.Parameter is IViewPresenter presenter)
            {
                Debug.Assert(presenter.PageCount == 1);
                Packager = (ViewPackager)presenter.GetAllPages()[0];
                ProcessPackager();
            }
        }
        
        private void ProcessPackager()
        {
            Appearing += Packager.RaiseAppearing;
            Disappearing += Packager.RaiseDisappearing;

            InternalPage = new ContentPage
            {
                Content = Packager.Content,
                BindingContext = Packager.ViewModel,
            };
            
            Content = new Windows.UI.Xaml.Controls.Grid
            {
                Children = { InternalPage.CreateFrameworkElement() }
            };
        }
        
        private void FrameworkElement_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (Content is FrameworkElement element)
            {
                InternalPage?.Layout(new Rectangle(0, 0, element.ActualWidth, element.ActualHeight));
            }
        }
    }
}
