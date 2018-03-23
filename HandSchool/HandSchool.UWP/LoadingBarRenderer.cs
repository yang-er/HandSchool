using HandSchool.Views;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using WColor = Windows.UI.Color;

namespace HandSchool.UWP
{
    public class LoadingBarRenderer : ViewRenderer<LoadingBar, FormsProgressBar>
    {
        object _foregroundDefault;

        protected override void OnElementChanged(ElementChangedEventArgs<LoadingBar> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new FormsProgressBar { IsIndeterminate = true, Style = Windows.UI.Xaml.Application.Current.Resources["FormsProgressBarStyle"] as Windows.UI.Xaml.Style });

                    Control.Loaded += OnControlLoaded;
                }

                // UpdateColor() called when loaded to ensure we can cache dynamic default colors
                UpdateIsRunning();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == LoadingBar.IsRunningProperty.PropertyName || e.PropertyName == VisualElement.OpacityProperty.PropertyName)
                UpdateIsRunning();
            else if (e.PropertyName == LoadingBar.ColorProperty.PropertyName)
                UpdateColor();
        }

        void OnControlLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // _foregroundDefault = Control.GetForegroundCache();
            UpdateColor();
        }

        void UpdateColor()
        {
            Color color = Element.Color;

            if (color.IsDefault)
            {
                // Control.RestoreForegroundCache(_foregroundDefault);
            }
            else
            {
                Control.Foreground = new SolidColorBrush(
                    WColor.FromArgb(
                        (byte)(color.A * 255),
                        (byte)(color.R * 255),
                        (byte)(color.G * 255),
                        (byte)(color.B * 255)
                        ));
            }
        }

        void UpdateIsRunning()
        {
            Control.ElementOpacity = Element.IsRunning ? Element.Opacity : 0;
        }
    }
}
