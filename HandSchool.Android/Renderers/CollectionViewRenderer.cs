#nullable enable
using Android.Content;
using AndroidX.RecyclerView.Widget;
using HandSchool.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Rect = Android.Graphics.Rect;
using View = Android.Views.View;

[assembly:ExportRenderer(typeof(CollectionView), typeof(CollectionViewRenderer2))]

namespace HandSchool.Droid.Renderers
{
    public class CollectionViewRenderer2 : CollectionViewRenderer 
    {
        protected class ItemDecoration2 : ItemDecoration
        {
            public ItemDecoration2(Rect padding)
            {
                Padding = padding;
            }
            public Rect Padding { get; set; }
            public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, State state)
            {
                base.GetItemOffsets(outRect, view, parent, state);
                outRect.Left = Padding.Left;
                outRect.Right = Padding.Right;
                outRect.Top = Padding.Top;
                outRect.Bottom = Padding.Bottom;
            }
        }

        protected ItemDecoration2? Decoration;

        public CollectionViewRenderer2(Context context) : base(context)
        {
            SetClipChildren(false);
            SetClipToPadding(false);
            SetPadding(0, PlatformImplV2.Instance.Dip2Px(5), 0, PlatformImplV2.Instance.Dip2Px(3));
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            var p = (int) (w * 0.0243f + 0.5f);
            if (Decoration is { })
            {
                if (Decoration.Padding.Left == p && Decoration.Padding.Right == p) return;
                RemoveItemDecoration(Decoration);
            }
            AddItemDecoration(Decoration = new ItemDecoration2(new Rect(p, 0, p, 0)));
        }
    }
}