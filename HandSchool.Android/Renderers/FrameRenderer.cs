using Android.Content;
using Android.Graphics;
using Android.Views;
using AndroidX.CardView.Widget;
using HandSchool;
using HandSchool.Controls;
using HandSchool.Droid.Renderers;
using HandSchool.Internal;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using FrameRenderer = Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer;

[assembly: ExportRenderer(typeof(Frame), typeof(SuperFrameRenderer))]

[assembly: ExportRenderer(typeof(TouchableFrame), typeof(TouchableFrameRenderer))]

namespace HandSchool.Droid.Renderers
{
    public class SuperFrameRenderer : FrameRenderer
    {
        public SuperFrameRenderer(Context context) : base(context) { }
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            using (var tv = new Android.Util.TypedValue())
            {
                Context.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, tv, true);
                Foreground = Context.Theme.GetDrawable(tv.ResourceId);
                Clickable = true;
            }
        }
    }

    public class TouchableFrameRenderer : FrameRenderer
    {
        TouchableFrame TouchableElement;
        public TouchableFrameRenderer(Context context) : base(context) { }
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            using (var tv = new Android.Util.TypedValue())
            {
                Context.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, tv, true);
                Foreground = Context.Theme.GetDrawable(tv.ResourceId);
                Clickable = true;
            }
            TouchableElement = e.NewElement as TouchableFrame;
            var click = TouchableElement.ClickedCommand;
            var long_click = TouchableElement.LongClickedCommand;
            if (click != null)
            {
                if (TouchableElement is TextAtom textAtom)
                {
                    Click += async (s, e) =>
                    {
                        await textAtom.TappedAnimation(async () =>
                        {
                            await Task.Yield();
                            click.Execute(null);
                        });
                    };
                }
                else
                {
                    Click += (s, e) => click.Execute(null);
                }
            }
            else SetOnClickListener(null);
            if (long_click != null)
            {
                if (TouchableElement is TextAtom textAtom)
                {
                    LongClick += async (s, e) =>
                    {
                        await textAtom.LongPressAnimation(async () =>
                        {
                            await Task.Yield();
                            long_click.Execute(null);
                        });
                    };
                }
                else
                {
                    LongClick += (s, e) => long_click.Execute(null);
                }
            }
            else SetOnLongClickListener(null);
        }
    }
}