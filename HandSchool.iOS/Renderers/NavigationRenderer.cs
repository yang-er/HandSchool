using HandSchool.Internal;
using HandSchool.iOS;
using HandSchool.Views;
using System;
using System.Linq;
using System.Threading.Tasks;
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
        NavigationPage Page;
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.NewElement != null)
            {
                Page = e.NewElement as NavigationPage;
                Page.OnThisPlatform().EnableTranslucentNavigationBar();
            }
            else Page = null;

            base.OnElementChanged(e);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (!ViewControllers.Any())
                return;
            var parentingViewController = ViewControllers.Last();
            UpdateLeftBarButton(parentingViewController);
        }
        static async void Close(object s,EventArgs e)
        {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PopModalAsync();
        }
        public void UpdateLeftBarButton(UIViewController controller)
        {
            if (Element is NavigationPage navpg)
            {
                var cancelBtn = new UIBarButtonItem
                {
                    Title = "返回",
                };

                cancelBtn.Clicked += Close;
                controller.NavigationItem.LeftBarButtonItem = cancelBtn;

                if (navpg.CurrentPage is ViewObject pg)
                {
                    if (!(bool)pg.GetValue(PlatformExtensions.ShowLeftCancelProperty))
                    {
                        cancelBtn.Clicked -= Close;
                        controller.NavigationItem.LeftBarButtonItem = cancelBtn = null;
                    }
                }
            }
        }
    }
}