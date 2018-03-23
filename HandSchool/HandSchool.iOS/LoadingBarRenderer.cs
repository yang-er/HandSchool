using HandSchool.Views;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace HandSchool.iOS
{
    public class LoadingBarRenderer : ViewRenderer<LoadingBar, UIActivityIndicatorView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<LoadingBar> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new UIActivityIndicatorView(RectangleF.Empty) { ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray });
                }

                UpdateColor();
                UpdateIsRunning();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == LoadingBar.ColorProperty.PropertyName)
                UpdateColor();
            else if (e.PropertyName == LoadingBar.IsRunningProperty.PropertyName)
                UpdateIsRunning();
        }

        void UpdateColor()
        {
            Control.Color = Element.Color == Color.Default ? null : Element.Color.ToUIColor();
        }

        void UpdateIsRunning()
        {
            if (Element.IsRunning)
                Control.StartAnimating();
            else
                Control.StopAnimating();
        }

        internal void PreserveState()
        {
            if (Control != null && !Control.IsAnimating && Element != null && Element.IsRunning)
                Control.StartAnimating();
        }
    }
}