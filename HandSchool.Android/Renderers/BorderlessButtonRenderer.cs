using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using HandSchool.Droid.Renderers;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using Xamarin.Forms.Platform.Android.FastRenderers;
using AView = Android.Views.View;

[assembly: ExportRenderer(typeof(BorderlessButton), typeof(BorderlessButtonRenderer))]
namespace HandSchool.Droid.Renderers
{
    public class BorderlessButtonRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<BorderlessButton, AppCompatButton>,
        AView.IOnClickListener, AView.IOnTouchListener
    {
        bool _isDisposed;

        public BorderlessButtonRenderer(Context context) : base(context)
        {
            AutoPackage = false;
        }
        
        global::Android.Widget.Button NativeButton => Control;
        
        protected override AppCompatButton CreateNativeControl()
        {
            return new AppCompatButton(Context, null, Android.Resource.Attribute.BorderlessButtonStyle);
        }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            if (disposing)
            {
                if (Control != null)
                {
                    Control.SetOnClickListener(null);
                    Control.SetOnTouchListener(null);
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BorderlessButton> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    AppCompatButton button = CreateNativeControl();
                    button.SetOnClickListener(this);
                    button.SetOnTouchListener(this);
                    SetNativeControl(button);
                    button.SetText(e.NewElement.Text);
                    button.SetTextColor(Color.Accent.ToAndroid());
                }

                UpdateEnabled();
            }
        }
        
        void UpdateEnabled()
        {
            Control.Enabled = Element.IsEnabled;
        }
        
        void IOnClickListener.OnClick(AView v) => ButtonElementManager.OnClick(Element, Element, v);

        bool IOnTouchListener.OnTouch(AView v, MotionEvent e) => ButtonElementManager.OnTouch(Element, Element, v, e);
    }
}