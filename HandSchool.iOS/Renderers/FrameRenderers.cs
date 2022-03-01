using HandSchool.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using HandSchool.Internal;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(Frame), typeof(HandSchool.iOS.Renderers.FrameRender))]
[assembly: ExportRenderer(typeof(TouchableFrame), typeof(HandSchool.iOS.Renderers.TouchableFrameRenderer))]

namespace HandSchool.iOS.Renderers
{
    public class FrameRender : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement?.HasShadow == true)
            {
                Layer.ShadowRadius = 3;
                Layer.ShadowOpacity = 0.2f;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case "IsVisible":
                case "HasShadow":
                {
                    if (Element.HasShadow)
                    {
                        Layer.ShadowRadius = 3;
                        Layer.ShadowOpacity = 0.2f;
                    }
                    break;
                }
            }
        }
    }
    
    public class TouchableFrameRenderer : FrameRender
    {
        private readonly List<UIGestureRecognizer> _customRecognizers = new List<UIGestureRecognizer>();
        private void RefreshLongClick(TouchableFrame tf)
        {
            if (tf.HasLongClick)
            {
                var recognizer = new UILongPressGestureRecognizer(s =>
                {
                    if (s.State != UIGestureRecognizerState.Began) return;
                    if (tf is TextAtom ta)
                    {
                        ta.LongPressAnimation(() =>
                        {
                            tf.OnLongClick();
                            return Task.CompletedTask;
                        });
                    }
                    else
                    {
                        tf.OnLongClick();
                    }
                });
                _customRecognizers.Add(recognizer);
                AddGestureRecognizer(recognizer);
            }
            else
            {
                if (_customRecognizers.Count == 0) return;
                _customRecognizers.OfType<UILongPressGestureRecognizer>().ForEach(RemoveGestureRecognizer);
            }
        }
        private void RefreshClick(TouchableFrame tf)
        {
            if (tf.HasClick)
            {
                var recognizer = new UITapGestureRecognizer(() =>
                {
                    if (tf is TextAtom ta)
                    {
                        ta.TappedAnimation(() =>
                        {
                            tf.OnClick();
                            return Task.CompletedTask;
                        });
                    }
                    else
                    {
                        tf.OnClick();
                    }
                });
                AddGestureRecognizer(recognizer);
                _customRecognizers.Add(recognizer);
            }
            else
            {
                if (_customRecognizers.Count == 0) return;
                _customRecognizers.OfType<UITapGestureRecognizer>().ForEach(RemoveGestureRecognizer);
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            if (e.OldElement != null)
            {
                _customRecognizers.ForEach(RemoveGestureRecognizer);
                _customRecognizers.Clear();
            }
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                var tf = (TouchableFrame) e.NewElement;
                RefreshClick(tf);
                RefreshLongClick(tf);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case "HasClick":
                    RefreshClick(sender as TouchableFrame);
                    break;
                case "HasLongClick":
                    RefreshLongClick(sender as TouchableFrame);
                    break;
            }
        }
    }

}