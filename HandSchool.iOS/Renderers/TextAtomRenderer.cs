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

namespace HandSchool.iOS.Renderers
{
    public class TextAtomRenderer : FrameRenderer
    {
        TextAtom target;
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null) return;
            target = e.NewElement as TextAtom;
            if (target.ClickedCommand != null || target.LongClickedCommand != null)
            {
                UserInteractionEnabled = true;
                if (target.ClickedCommand != null)
                {
                    AddGestureRecognizer(new UITapGestureRecognizer(() => { target.ClickedCommand.Execute(null); }));
                }
                if (target.LongClickedCommand != null)
                {
                    AddGestureRecognizer(new UILongPressGestureRecognizer(
                        (s) =>
                        {
                            if (s.State == UIGestureRecognizerState.Began)
                                target.LongClickedCommand.Execute(null);
                        }));
                }
            }
        }
    }

    
}