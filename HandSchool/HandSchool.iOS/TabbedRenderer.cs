using HandSchool.iOS;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MainPage), typeof(MainPageRenderer))]
namespace HandSchool.iOS
{
    class MainPageRenderer : TabbedRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            HidesBottomBarWhenPushed = true;
        }
    }
}