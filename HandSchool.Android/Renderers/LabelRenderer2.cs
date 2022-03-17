using System.ComponentModel;
using Android.Content;
using HandSchool.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Label), typeof(LabelRenderer2))]

namespace HandSchool.Droid.Renderers
{
    public class LabelRenderer2 : LabelRenderer
    {
        public LabelRenderer2(Context context) : base(context)
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "Text")
            {
                Control.Post(() => { Control.Text = Element.Text; });
            }
        }
    }
}