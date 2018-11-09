using CoreGraphics;
using HandSchool.iOS;
using HandSchool.Views;
using System;
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

            if (e.NewElement is PopContentPage page)
            {
                page.PropertyChanged += IsBusyChanged;
                SetIsBusy();
            }

            if (e.OldElement is PopContentPage page2)
            {
                page2.PropertyChanged -= IsBusyChanged;
            }
        }

        private void IsBusyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsBusy") SetIsBusy();
        }

        private void SetIsBusy()
        {
            if (Element is PopContentPage pg && pg.ShowIsBusyDialog && pg.IsBusy)
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