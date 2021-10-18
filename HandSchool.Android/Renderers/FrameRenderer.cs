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
        private TouchableFrame _touchableElement;
        public TouchableFrameRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            _touchableElement = e.NewElement as TouchableFrame;
            if (_touchableElement is null)
            {
                _touchableElement = null;
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
                RefreshOnClickListener();
                RefreshOnLongClickListener();
            }
        }

        private void RefreshOnClickListener()
        {
            if (_touchableElement.HasClick)
            {
                if (_touchableElement is TextAtom)
                {
                    SetOnClickListener(new ClickListener(async v =>
                    {
                        await ((TextAtom) _touchableElement).TappedAnimation(async () =>
                        {
                            await Task.Yield();
                            _touchableElement.OnClick();
                        });
                    }));
                }
                else
                {
                    SetOnClickListener(new ClickListener(v => _touchableElement.OnClick()));
                }
            }
            else
            {
                SetOnClickListener(null);
            }
        }

        private void RefreshOnLongClickListener()
        {
            if (_touchableElement.HasLongClick)
            {
                if (_touchableElement is TextAtom)
                {
                    SetOnLongClickListener(new LongClickListener(
                        async v =>
                            await ((TextAtom) _touchableElement).LongPressAnimation(
                                async () =>
                                {
                                    await Task.Yield();
                                    _touchableElement.OnLongClick();
                                })));
                }
                else
                {
                    SetOnLongClickListener(new LongClickListener(v => _touchableElement.OnLongClick()));
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
                    RefreshOnClickListener();
                    break;
                case "HasLongClick":
                    RefreshOnLongClickListener();
                    break;
            }
        }
    }
}