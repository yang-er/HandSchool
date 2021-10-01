using Foundation;
using HandSchool.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(TextAtom), typeof(HandSchool.iOS.Renderers.TextAtomRenderer))]
[assembly: ExportRenderer(typeof(Frame), typeof(HandSchool.iOS.Renderers.FrameRender))]
[assembly: ExportRenderer(typeof(HandSchool.Internal.TouchableFrame), typeof(HandSchool.iOS.Renderers.TouchableFrameRenderer))]

namespace HandSchool.iOS.Renderers
{
    public class FrameRender : FrameRenderer
    {
        protected Frame target = null;
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            target = e.NewElement;
            if (target == null)
            {
                GestureRecognizers = null;
                return;
            }
            target.HasShadow = false;
            target.BorderColor = Color.LightGray;
        }
    }
    public class TextAtomRenderer : FrameRender
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (target == null) return;

            var ta = target as TextAtom;
            if (ta.ClickedCommand != null || ta.LongClickedCommand != null)
            {
                UserInteractionEnabled = true;
                if (ta.ClickedCommand != null)
                {
                    AddGestureRecognizer(new UITapGestureRecognizer(async () =>
                    {
                        await target.TappedAnimation(async () =>
                        {
                            await System.Threading.Tasks.Task.Yield();
                            ta.ClickedCommand.Execute(null);
                        });
                    }));
                }
                if (ta.LongClickedCommand != null)
                {
                    AddGestureRecognizer(new UILongPressGestureRecognizer(
                        async (s) =>
                        {
                            if (s.State == UIGestureRecognizerState.Began)
                            {
                                await target.LongPressAnimation(async () =>
                                {
                                    await System.Threading.Tasks.Task.Yield();
                                    ta.LongClickedCommand.Execute(null);
                                });
                            }
                        }));
                }
            }
        }
    }

    public class TouchableFrameRenderer : FrameRender
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (target == null) return;
            var tf = target as Internal.TouchableFrame;
            if (tf.ClickedCommand != null || tf.LongClickedCommand != null)
            {
                UserInteractionEnabled = true;
                if (tf.ClickedCommand != null)
                {
                    AddGestureRecognizer(new UITapGestureRecognizer(() => { tf.ClickedCommand.Execute(null); }));
                }
                if (tf.LongClickedCommand != null)
                {
                    AddGestureRecognizer(new UILongPressGestureRecognizer(
                        (s) =>
                        {
                            if (s.State == UIGestureRecognizerState.Began)
                                tf.LongClickedCommand.Execute(null);
                        }));
                }
            }
        }
    }

}