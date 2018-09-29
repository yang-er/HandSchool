using CoreGraphics;
using HandSchool.iOS;
using HandSchool.Views;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PopContentPage), typeof(PopContentPageRenderer))]
namespace HandSchool.iOS
{
    class PopContentPageRenderer : PageRenderer
    {
        private UIActivityIndicatorView Spinner;
        private PopContentPage ElementPage => Element as PopContentPage;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.NewElement is PopContentPage page)
                page.PropertyChanged += IsBusyChanged;
            if (e.OldElement is PopContentPage page2)
                page2.PropertyChanged -= IsBusyChanged;
            if (Spinner != null) return;

            Spinner = new UIActivityIndicatorView(new CGRect(0, 0, 100, 100))
            {
                ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge,
                BackgroundColor = UIColor.Gray,
            };

            Spinner.Layer.CornerRadius = 10;
            NativeView.AddSubview(Spinner);
        }

        private void IsBusyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsBusy" && ElementPage.ShowIsBusyDialog)
            {
                if (ElementPage.IsBusy)
                {
                    Spinner.Center = NativeView.Center;
                    Spinner.StartAnimating();
                }
                else
                {
                    Spinner.StopAnimating();
                }
            }
        }
    }
}