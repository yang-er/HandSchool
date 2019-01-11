using CoreGraphics;
using HandSchool.iOS;
using HandSchool.Views;
using System;
using System.ComponentModel;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using static Xamarin.Forms.PlatformConfiguration.iOSSpecific.NavigationPage;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(NavigationPageRenderer))]
namespace HandSchool.iOS
{
    class NavigationPageRenderer : NavigationRenderer
    {
        public static double NavigationBarHeight = 64;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.NewElement is NavigationPage page)
            {
                page.OnThisPlatform().SetIsNavigationBarTranslucent(true);
                page.SizeChanged += HeightReset;
            }

            if (e.OldElement is NavigationPage page2)
            {
                page2.SizeChanged -= HeightReset;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (!ViewControllers.Any())
                return;
            var parentingViewController = ViewControllers.Last();
            UpdateLeftBarButton(parentingViewController);
        }

        public void UpdateLeftBarButton(UIViewController controller)
        {
            if (Element is NavigationPage navpg)
            {
                if (navpg.CurrentPage is PopContentPage pg)
                {
                    if (pg.ShowCancel)
                    {
                        var cancelBtn = new UIBarButtonItem
                        {
                            Title = "取消",
                        };

                        cancelBtn.Clicked += async (sender, e) => await pg.CloseAsync();
                        controller.NavigationItem.LeftBarButtonItem = cancelBtn;
                    }
                }
            }
        }

        private void HeightReset(object sender, EventArgs args)
        {
            NavigationBarHeight = (double)(NavigationBar.Frame.Height + AppDelegate.SharedApplication.StatusBarFrame.Height);
        }
    }
}