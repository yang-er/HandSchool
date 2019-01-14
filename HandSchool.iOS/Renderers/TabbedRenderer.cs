using HandSchool.iOS;
using HandSchool.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MainPage), typeof(MainPageRenderer))]
namespace HandSchool.iOS
{
    public class MainPageRenderer : TabbedRenderer
    {
        public MainPageRenderer()
        {
            MessagingCenter.Subscribe<object, UIAlertController>(this, PlatformImpl.UIViewControllerRequest, UIViewControllerRequested);
            MessagingCenter.Subscribe<MainPage, bool>(this, MainPage.SelectPageSignal, SelectPageChanged);
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            HidesBottomBarWhenPushed = true;
            TabBar.Translucent = true;
            
            if ((e.NewElement as MainPage).Children.Count == 1)
            {
                TabBar.Hidden = true;
            }
        }

        private void SelectPageChanged(MainPage sender, bool args)
        {
            TabBar.Hidden = args;
        }
        
        private void UIViewControllerRequested(object sender, UIAlertController args)
        {
            PresentViewController(args, true, null);
        }
    }
}