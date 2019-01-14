using HandSchool.iOS;
using HandSchool.Views;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewPage), typeof(ViewPageRenderer))]
namespace HandSchool.iOS
{
    public class ViewPageRenderer : PageRenderer
    {
        private UIActivityIndicatorView Spinner;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (Spinner == null)
            {
                Spinner = new UIActivityIndicatorView
                {
                    ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge,
                    BackgroundColor = UIColor.Gray,
                };

                Spinner.Layer.CornerRadius = 10;
                NativeView.AddSubview(Spinner);

                Spinner.TranslatesAutoresizingMaskIntoConstraints = false;
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    Spinner, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal,
                    NativeView, NSLayoutAttribute.CenterX, (nfloat)1.0, (nfloat)0.0));
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    Spinner, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal,
                    NativeView, NSLayoutAttribute.CenterY, (nfloat)1.0, (nfloat)0.0));
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    Spinner, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                    (nfloat)1.0, (nfloat)100));
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    Spinner, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                    (nfloat)1.0, (nfloat)100));
            }

            MessagingCenter.Unsubscribe<Page, bool>(this, Page.BusySetSignalName);

            if (e.NewElement is ViewPage page)
            {
                MessagingCenter.Subscribe<Page, bool>(this, Page.BusySetSignalName, SetIsBusy, page);
            }
        }
        
        private void SetIsBusy(Page page, bool isBusy)
        {
            if (Element is ViewPage pg && (bool)pg.GetValue(HandSchool.Internal.PlatformExtensions.ShowLoadingProperty) && isBusy)
            {
                Spinner.StartAnimating();
            }
            else
            {
                Spinner.StopAnimating();
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            }
        }
    }
}