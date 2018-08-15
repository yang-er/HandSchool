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
        UIActivityIndicatorView Spinner;
        private PopContentPage ElementPage => Element as PopContentPage;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            var page = e.NewElement as PopContentPage;
            var view = NativeView;

            Spinner = new UIActivityIndicatorView(new CGRect(0, 0, 100, 100))
            {
                Center = NativeView.Center,
                ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge,
                BackgroundColor = UIColor.Gray
            };

            Spinner.Layer.CornerRadius = 10;
            Spinner.ToView();
            NativeView.AddSubview(Spinner);
            page.PropertyChanged += IsBusyChanged;
        }

        private void IsBusyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsBusy")
            {
                if (ElementPage.IsBusy)
                    Spinner.StartAnimating();
                else
                    Spinner.StopAnimating();
            }
        }
    }
}