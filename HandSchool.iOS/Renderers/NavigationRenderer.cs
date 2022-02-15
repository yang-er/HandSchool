using HandSchool.Internal;
using HandSchool.iOS;
using System;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(NavigationPageRenderer))]

namespace HandSchool.iOS
{
    public class NavigationPageRenderer : NavigationRenderer
    {
        private NavigationPage Page => Element as NavigationPage;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.NewElement is { })
            {
                Page.OnThisPlatform().EnableTranslucentNavigationBar();
            }

            base.OnElementChanged(e);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (ViewControllers?.Any() != true)
                return;
            var parentingViewController = ViewControllers.Last();
            UpdateLeftBarButton(parentingViewController);
        }

        private static async void Close(object s, EventArgs e)
        {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PopModalAsync();
        }

        public void UpdateLeftBarButton(UIViewController controller)
        {
            if (!(Element is NavigationPage {CurrentPage: { } currentPage})) return;
            if (!(bool) currentPage.GetValue(PlatformExtensions.ShowLeftCancelProperty)) return;
            var cancelBtn = new UIBarButtonItem {Title = "返回"};
            cancelBtn.Clicked += Close;
            controller.NavigationItem.LeftBarButtonItem = cancelBtn;
        }
    }
}