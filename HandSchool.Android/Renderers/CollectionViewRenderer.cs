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
        
        public CollectionViewRenderer2(Context context) : base(context)
        {
            SetClipChildren(false);
            SetClipToPadding(false);
            AddItemDecoration(new ItemDecoration2(new Rect(1, 1, 1, 1)));
        }
    }
}