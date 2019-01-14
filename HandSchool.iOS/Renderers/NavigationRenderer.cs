using HandSchool.Internal;
using HandSchool.iOS;
using HandSchool.Views;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(NavigationPageRenderer))]
namespace HandSchool.iOS
{
    public class NavigationPageRenderer : NavigationRenderer
    {
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
                if (navpg.CurrentPage is ViewPage pg)
                {
                    if ((bool)pg.GetValue(PlatformExtensions.ShowLeftCancelProperty))
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
    }
}