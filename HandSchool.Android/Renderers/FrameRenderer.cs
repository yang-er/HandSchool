using Android.Content;
using HandSchool.Controls;
using HandSchool.Droid.Renderers;
using HandSchool.Internal;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
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
            using var tv = new Android.Util.TypedValue();
            Context?.Theme?.ResolveAttribute(Resource.Attribute.selectableItemBackground, tv, true);
            Foreground = Context?.Theme?.GetDrawable(tv.ResourceId);
            Clickable = true;
        }
    }

    public class TouchableFrameRenderer : FrameRenderer
    {
        private bool _disposed;
        public TouchableFrameRenderer(Context context) : base(context) { }
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (!(e.NewElement is TouchableFrame touchableElement))
            {
                SetOnClickListener(null);
                SetOnLongClickListener(null);
            }
            else
            {
                using (var tv = new Android.Util.TypedValue())
                {
                    Context?.Theme?.ResolveAttribute(Resource.Attribute.selectableItemBackground, tv, true);
                    Foreground = Context?.Theme?.GetDrawable(tv.ResourceId);
                    Clickable = true;
                }
                RefreshOnClickListener(touchableElement);
                RefreshOnLongClickListener(touchableElement);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            SetOnClickListener(null);
            SetOnLongClickListener(null);
            _disposed = true;
            base.Dispose(disposing);
        }
        private void RefreshOnClickListener(TouchableFrame touchableElement)
        {
            if (touchableElement is null) return;
            if (touchableElement.HasClick)
            {
                if (touchableElement is TextAtom)
                {
                    SetOnClickListener(new ClickListener(async v =>
                    {
                        await ((TextAtom) touchableElement).TappedAnimation(async () =>
                        {
                            await Task.Yield();
                            touchableElement.OnClick();
                        });
                    }));
                }
                else
                {
                    SetOnClickListener(new ClickListener(v => touchableElement.OnClick()));
                }
            }
            else
            {
                SetOnClickListener(null);
            }
        }

        private void RefreshOnLongClickListener(TouchableFrame touchableElement)
        {
            if (touchableElement is null) return;
            if (touchableElement.HasLongClick)
            {
                if (touchableElement is TextAtom)
                {
                    SetOnLongClickListener(new LongClickListener(
                        async v =>
                            await ((TextAtom) touchableElement).LongPressAnimation(
                                async () =>
                                {
                                    await Task.Yield();
                                    touchableElement.OnLongClick();
                                })));
                }
                else
                {
                    SetOnLongClickListener(new LongClickListener(v => touchableElement.OnLongClick()));
                }
            }
            else
            {
                SetOnLongClickListener(null);
            }
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case "HasClick":
                    RefreshOnClickListener(sender as TouchableFrame);
                    break;
                case "HasLongClick":
                    RefreshOnLongClickListener(sender as TouchableFrame);
                    break;
            }
        }
    }
}