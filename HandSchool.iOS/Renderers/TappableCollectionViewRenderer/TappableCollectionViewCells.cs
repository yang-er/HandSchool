#nullable enable
using System;
using System.Reflection;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace HandSchool.iOS
{
    public struct MarginInfo
    {
        public Thickness Thickness { get; set; }
        public int Span { get; set; }
    }
    public abstract class MarginCell : TemplatedCell
    {
        private static readonly PropertyInfo? RendererInfo;

        static MarginCell()
        {
            RendererInfo = typeof(TemplatedCell).GetDeclaredProperty("VisualElementRenderer");
        }

        [Export("initWithFrame:")]
        [Xamarin.Forms.Internals.Preserve(Conditional = true)]
        protected MarginCell(CGRect frame) : base(frame)
        {
            RendererInfo?.Let(info => { GetRenderer = () => (IVisualElementRenderer) info.GetValue(this); });
        }

        protected Func<IVisualElementRenderer>? GetRenderer;

        public abstract void SetCollectionViewPadding(MarginInfo padding);
    }

    public class VerticalMarginCell : MarginCell
    {
        public static readonly NSString ReuseId = new NSString("HandSchool.iOS.VerticalMarginCell");

        [Export("initWithFrame:")]
        [Xamarin.Forms.Internals.Preserve(Conditional = true)]
        protected VerticalMarginCell(CGRect frame) : base(frame)
        {
        }

        public override void ConstrainTo(CGSize constraint)
        {
            ClearConstraints();
            ConstrainedDimension = constraint.Width;
        }

        protected override (bool, Size) NeedsContentSizeUpdate(Size currentSize)
        {
            if (GetRenderer is null) throw new NotSupportedException("Program cannot work");
            return GetRenderer()?.Element?.Let(e =>
            {
                var size = Size.Zero;
                var bounds = e.Bounds;

                if (bounds.Width <= 0 || bounds.Height <= 0)
                {
                    return (false, size);
                }

                var desiredBounds = e.Measure(bounds.Width, double.PositiveInfinity,
                    MeasureFlags.IncludeMargins);

                return Math.Abs(desiredBounds.Request.Height - currentSize.Height) < 1e-5
                    ? (false, size)
                    : (true, desiredBounds.Request);
            }) ?? (false, Size.Zero);
        }

        protected override bool AttributesConsistentWithConstrainedDimension(
            UICollectionViewLayoutAttributes attributes)
        {
            return attributes.Frame.Width == ConstrainedDimension;
        }

        private MarginInfo _padding;

        public override void SetCollectionViewPadding(MarginInfo padding)
        {
            _padding = padding;
        }

        public override CGSize Measure()
        {
            AddConstraint(NSLayoutConstraint.Create(ContentView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal,
                this, NSLayoutAttribute.CenterX, (nfloat) 1.0, (nfloat) 0.0));
            AddConstraint(NSLayoutConstraint.Create(ContentView.Subviews[0], NSLayoutAttribute.CenterX, NSLayoutRelation.Equal,
                ContentView, NSLayoutAttribute.CenterX, (nfloat) 1.0, (nfloat) 0.0));
            return GetRenderer?.Let(getRenderer =>
            {
                var deltaW = _padding.Thickness.HorizontalThickness / _padding.Span;
                var width = ConstrainedDimension - deltaW;
                var measure = GetRenderer().Element.Measure(width,
                    double.PositiveInfinity, MeasureFlags.IncludeMargins);
                return new CGSize(width, measure.Request.Height);
            }) ?? throw new NotSupportedException("Program cannot work");
        }
    }

    public class HorizontalMarginCell : MarginCell
    {
        public static readonly NSString ReuseId = new NSString("HandSchool.iOS.HorizontalMarginCell");

        [Export("initWithFrame:")]
        [Xamarin.Forms.Internals.Preserve(Conditional = true)]
        protected HorizontalMarginCell(CGRect frame) : base(frame)
        {
        }

        private MarginInfo _padding;

        public override void ConstrainTo(CGSize constraint)
        {
            ClearConstraints();
            ConstrainedDimension = constraint.Height;
        }

        protected override (bool, Size) NeedsContentSizeUpdate(Size currentSize)
        {
            return GetRenderer?.Let(getRenderer =>
            {
                return getRenderer()?.Element?.Let(element =>
                {
                    var size = Size.Zero;
                    var bounds = element.Bounds;

                    if (bounds.Width <= 0 || bounds.Height <= 0)
                    {
                        return (false, size);
                    }

                    var desiredBounds = element.Measure(double.PositiveInfinity, bounds.Height,
                        MeasureFlags.IncludeMargins);

                    return Math.Abs(desiredBounds.Request.Width - currentSize.Width) < 1e-6
                        ? (false, size)
                        : (true, desiredBounds.Request);
                }) ?? (false, Size.Zero);
            }) ?? throw new NotSupportedException("Program cannot work");
        }

        public override CGSize Measure()
        {
            return GetRenderer?.Let(getRenderer =>
            {
                var deltaY = _padding.Thickness.VerticalThickness / _padding.Span;
                var height = ConstrainedDimension - deltaY;
                var measure = getRenderer().Element.Measure(double.PositiveInfinity,
                    height, MeasureFlags.IncludeMargins);
                return new CGSize(measure.Request.Width, height);
            }) ?? throw new NotSupportedException("Program cannot work");
        }

        protected override bool AttributesConsistentWithConstrainedDimension(
            UICollectionViewLayoutAttributes attributes)
        {
            return attributes.Frame.Width == ConstrainedDimension;
        }

        public override void SetCollectionViewPadding(MarginInfo padding)
        {
            _padding = padding;
        }
    }
}