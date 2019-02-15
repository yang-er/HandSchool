using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

namespace HandSchool.UWP.Renderers
{
    internal sealed class BackgroundTracker2<T> : VisualElementTracker<Page, T> where T : FrameworkElement
    {
        readonly DependencyProperty _backgroundProperty;
        bool _backgroundNeedsUpdate = true;

        public BackgroundTracker2(DependencyProperty backgroundProperty)
        {
            _backgroundProperty = backgroundProperty ?? throw new ArgumentNullException("backgroundProperty");
        }

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Page.BackgroundImageProperty.PropertyName)
            {
                UpdateBackground();
            }

            base.OnPropertyChanged(sender, e);
        }

        protected override void UpdateNativeControl()
        {
            base.UpdateNativeControl();

            if (_backgroundNeedsUpdate)
                UpdateBackground();
        }

        void UpdateBackground()
        {
            if (Element == null)
                return;

            FrameworkElement element = Control ?? Container;
            if (element == null)
                return;

            string backgroundImage = Element.BackgroundImage;
            if (backgroundImage != null)
            {
                if (!Uri.TryCreate(backgroundImage, UriKind.RelativeOrAbsolute, out Uri uri) || !uri.IsAbsoluteUri)
                    uri = new Uri("ms-appx:///" + backgroundImage);

                element.SetValue(_backgroundProperty, new ImageBrush { ImageSource = new BitmapImage(uri) });
            }

            _backgroundNeedsUpdate = false;
        }
    }
}