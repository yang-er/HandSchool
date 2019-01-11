using HandSchool.iOS;
using HandSchool.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

/*
[assembly: ExportRenderer(typeof(TabletPageImpl), typeof(SplitPageRenderer), UIKit.UIUserInterfaceIdiom.Phone)]
[assembly: ExportRenderer(typeof(TabletPageImpl), typeof(SplitPageRenderer), UIKit.UIUserInterfaceIdiom.Pad)]
namespace HandSchool.iOS
{
    public class SplitPageRenderer : TabletMasterDetailRenderer
    {
        public SplitPageRenderer()
        {
            UISplitViewController spVc = this;
            Core.Log("SplitPageRenderer Created!");
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            this.ViewControllers[1].View.Frame = new CoreGraphics.CGRect(10, 0, 10, 0);
        }
    }
}
*/