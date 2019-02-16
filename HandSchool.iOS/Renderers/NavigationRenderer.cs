using HandSchool.Internal;
using HandSchool.iOS;
using HandSchool.Views;
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
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.NewElement != null)
            {
                ((NavigationPage)e.NewElement).OnThisPlatform().EnableTranslucentNavigationBar();
            }

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

        public void UpdateLeftBarButton(UIViewController controller)
        {
            if (Element is NavigationPage navpg)
            {
                if (navpg.CurrentPage is ViewObject pg)
                {
                    if ((bool)pg.GetValue(PlatformExtensions.ShowLeftCancelProperty))
                    {
                        var cancelBtn = new UIBarButtonItem
                        {
                            Title = "取消",
                        };

                        // cancelBtn.Clicked += async (sender, e) => await pg.CloseAsync();
                        controller.NavigationItem.LeftBarButtonItem = cancelBtn;
                    }
                }
            }
        }
    }
}