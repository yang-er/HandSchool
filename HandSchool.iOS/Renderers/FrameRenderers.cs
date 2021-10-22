using Foundation;
using HandSchool.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using HandSchool.Internal;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(Frame), typeof(HandSchool.iOS.Renderers.FrameRender))]
[assembly: ExportRenderer(typeof(HandSchool.Internal.TouchableFrame), typeof(HandSchool.iOS.Renderers.TouchableFrameRenderer))]

namespace HandSchool.iOS.Renderers
{
    public class FrameRender : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement is null)
            {
                GestureRecognizers = null;
                return;
            }

            if (e.NewElement.HasShadow)
            {
                e.NewElement.HasShadow = false;
                e.NewElement.BorderColor = Color.LightGray;
            }
        }
    }
    
    public class TouchableFrameRenderer : FrameRender
    {
        private List<UIGestureRecognizer> _userRecognizers = new List<UIGestureRecognizer>();
        private void RefreshLongClick(TouchableFrame tf)
        {
            if (tf.HasLongClick)
            {
                UILongPressGestureRecognizer recognizer;
                if (tf is TextAtom)
                {
                    recognizer = new UILongPressGestureRecognizer(s =>
                    {
                        if (s.State == UIGestureRecognizerState.Began)
                        {
                            ((TextAtom) tf).LongPressAnimation(async () =>
                            {
                                await Task.Yield();
                                tf.OnLongClick();
                            });
                        }
                    });
                }
                else
                {
                    recognizer = new UILongPressGestureRecognizer((s) =>
                    {
                        if (s.State == UIGestureRecognizerState.Began)
                        {
                            tf.OnLongClick();
                        }
                    });
                }
                _userRecognizers.Add(recognizer);
                AddGestureRecognizer(recognizer);
            }
            else
            {
                if (_userRecognizers.Count == 0) return;
                foreach (var i in _userRecognizers.OfType<UILongPressGestureRecognizer>())
                {
                    RemoveGestureRecognizer(i);
                }
            }
        }
        private void RefreshClick(TouchableFrame tf)
        {
            if (tf.HasClick)
            {
                UITapGestureRecognizer recognizer;
                if (tf is TextAtom)
                {
                    recognizer = new UITapGestureRecognizer(() =>
                    {
                        ((TextAtom) tf).TappedAnimation(async () =>
                        {
                            await Task.Yield();
                            tf.OnClick();
                        });
                    });
                }
                else
                {
                    recognizer = new UITapGestureRecognizer(() => tf.OnClick());
                }
                AddGestureRecognizer(recognizer);
                _userRecognizers.Add(recognizer);
            }
            else
            {
                if (_userRecognizers.Count == 0) return;
                foreach (var i in _userRecognizers.OfType<UITapGestureRecognizer>())
                {
                    RemoveGestureRecognizer(i);
                }
            }
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            _userRecognizers.Clear();
            if (e.NewElement is null) return;
            var tf = (TouchableFrame)e.NewElement;
            RefreshClick(tf);
            RefreshLongClick(tf);
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