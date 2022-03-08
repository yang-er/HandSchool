#nullable enable
using System;
using System.ComponentModel;
using Android.Content;
using HandSchool.Droid.Renderers;
using HandSchool.Controls;
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

    public class TouchableFrameRenderer : SuperFrameRenderer
    {
        private bool _disposed;
        public TouchableFrameRenderer(Context context) : base(context) { }

        private new TouchableFrame? Element => (TouchableFrame?) base.Element;
        
        private void OnElementClick(object sender, EventArgs eventArgs)
        {
            Element?.OnClick(eventArgs);
        }
        
        private void OnElementLongPress(object sender, EventArgs eventArgs)
        {
            Element?.OnLongClick(eventArgs);
        }

        private bool _hasClick;
        private bool _hasLongPress;

        private void RefreshClick()
        {
            if (Element?.HasClick == true)
            {
                if (_hasClick) return;
                Click += OnElementClick;
                _hasClick = true;
            }
            else
            {
                if (!_hasClick) return;
                Click -= OnElementClick;
                _hasClick = false;
            }
        }
        
        private void RefreshLongPress()
        {
            if (Element?.HasLongClick == true)
            {
                if (_hasLongPress) return;
                LongClick += OnElementLongPress;
                _hasLongPress = true;
            }
            else
            {
                if (!_hasLongPress) return;
                LongClick -= OnElementLongPress;
                _hasLongPress = false;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            RefreshClick();
            RefreshLongPress();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            SetOnClickListener(null);
            SetOnLongClickListener(null);
            _disposed = true;
            base.Dispose(disposing);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case "HasClick":
                    RefreshClick();
                    break;
                case "HasLongClick":
                    RefreshLongPress();
                    break;
            }
        }
    }
}