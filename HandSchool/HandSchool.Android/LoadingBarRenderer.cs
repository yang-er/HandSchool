using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using HandSchool.Views;
using Xamarin.Forms.Platform.Android;
using AProgressBar = Android.Widget.ProgressBar;
using XColor = Xamarin.Forms.Color;

namespace HandSchool.Droid
{
    public class ActivityIndicatorRenderer : ViewRenderer<LoadingBar, AProgressBar>
    {
        public ActivityIndicatorRenderer(Context context) : base(context)
        {
            AutoPackage = false;
        }
        
        protected override AProgressBar CreateNativeControl()
        {
            return new AProgressBar(Context) { Indeterminate = true };
        }

        protected override void OnElementChanged(ElementChangedEventArgs<LoadingBar> e)
        {
            base.OnElementChanged(e);

            AProgressBar progressBar = Control;
            if (progressBar == null)
            {
                progressBar = CreateNativeControl();
                SetNativeControl(progressBar);
            }

            UpdateColor();
            UpdateVisibility();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == LoadingBar.IsRunningProperty.PropertyName)
                UpdateVisibility();
            else if (e.PropertyName == LoadingBar.ColorProperty.PropertyName)
                UpdateColor();
        }

        void UpdateColor()
        {
            if (Element == null || Control == null)
                return;

            XColor color = Element.Color;

            if (!color.IsDefault)
                Control.IndeterminateDrawable?.SetColorFilter(color.ToAndroid(), PorterDuff.Mode.SrcIn);
            else
                Control.IndeterminateDrawable?.ClearColorFilter();
        }

        void UpdateVisibility()
        {
            if (Element == null || Control == null)
                return;

            Control.Visibility = Element.IsRunning ? ViewStates.Visible : ViewStates.Invisible;
        }
    }
}