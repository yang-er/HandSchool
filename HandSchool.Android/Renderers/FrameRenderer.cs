using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using HandSchool.Droid.Renderers;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportCell(typeof(Frame), typeof(AndroidCustomCardRenderer))]
namespace HandSchool.Droid.Renderers
{

    public class AndroidCustomCardRenderer : CardView, IVisualElementRenderer
    {
        public AndroidCustomCardRenderer() : base(PlatformImplV2.Instance.ContextStack.Peek()) { }

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
        public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

        ViewGroup packed;
        public void SetElement(VisualElement element)
        {
            var oldElement = this.Element;

            if (oldElement != null)
            {
                oldElement.PropertyChanged -= HandlePropertyChanged;
               
            }
            this.Element = element;
            if (this.Element != null)
            {

                this.Element.PropertyChanged += HandlePropertyChanged;
                this.Radius = Context.ToPixels(10);
                this.Elevation = Context.ToPixels(3);
                //SetCardBackgroundColor(Android.Graphics.Color.White);
            }

            ViewGroup.RemoveAllViews();
            Tracker = new VisualElementTracker(this);

            Packager = new VisualElementPackager(this);
            Packager.Load();

            UseCompatPadding = true;

            SetContentPadding((int)TheView.Padding.Left, (int)TheView.Padding.Top,
                   (int)TheView.Padding.Right, (int)TheView.Padding.Bottom);

            SetCardBackgroundColor(TheView.BackgroundColor.ToAndroid());

            if (ElementChanged != null)
                ElementChanged(this, new VisualElementChangedEventArgs(oldElement, this.Element));
        }

        public Frame TheView
        {
            get { return this.Element == null ? null : (Frame)Element; }
        }

        void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Content")
            {
                Tracker.UpdateLayout();
            }
            else if (e.PropertyName ==Frame.BackgroundColorProperty.PropertyName)
            {
                if (TheView.BackgroundColor != null)
                    SetCardBackgroundColor(TheView.BackgroundColor.ToAndroid());
                    //Background.SetColorFilter(TheView.BackgroundColor.ToAndroid(), PorterDuff.Mode.SrcAtop);
            }
        }

        public SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            packed.Measure(widthConstraint, heightConstraint);
            return new SizeRequest(new Size(packed.MeasuredWidth, packed.MeasuredHeight));
        }

        public void UpdateLayout()
        {
            if (Tracker == null)
                return;

            Tracker.UpdateLayout();
        }

        public void SetLabelFor(int? id)
        {
            throw new NotImplementedException();
        }

        public VisualElementTracker Tracker { get; private set; }

        public VisualElementPackager Packager { get; private set; }

        public Android.Views.ViewGroup ViewGroup { get { return this; } }

        public VisualElement Element { get; private set; }

        public Android.Views.View View => this;
    }
}