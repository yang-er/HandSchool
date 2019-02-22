using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using HandSchool.Droid.Renderers;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using FrameRenderer = Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer;

[assembly: ExportCell(typeof(Frame), typeof(CardViewRenderer))]
namespace HandSchool.Droid.Renderers
{
    public class CardViewRenderer : FrameRenderer
    {
        public CardViewRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
        }
    }
}