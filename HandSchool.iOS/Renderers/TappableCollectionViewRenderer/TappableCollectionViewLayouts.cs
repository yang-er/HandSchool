#nullable enable
using System;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace HandSchool.iOS.Renderers
{
    public interface ITappableItemsViewLayout
    {
        public void UpdateCellConstraints(CGSize size);
    }

    public class TappableListViewLayout : ListViewLayout, ITappableItemsViewLayout
    {
        public TappableListViewLayout(LinearItemsLayout itemsLayout, ItemSizingStrategy itemSizingStrategy) : base(
            itemsLayout, itemSizingStrategy)
        {
        }

        public void UpdateCellConstraints(CGSize size)
        {
            ConstrainTo(size);
            UpdateCellConstraints();
        }
    }

    public class TappableGridViewLayout : GridViewLayout, ITappableItemsViewLayout
    {
        public TappableGridViewLayout(GridItemsLayout itemsLayout, ItemSizingStrategy itemSizingStrategy) : base(
            itemsLayout, itemSizingStrategy)
        {
            ItemsLayout = itemsLayout;
        }

        private GridItemsLayout ItemsLayout { get; }

        private int _count;

        public static nfloat ReduceSpacing(nfloat available, nfloat requestedSpacing, int span)
        {
            if (span == 1)
            {
                return requestedSpacing;
            }

            var maxSpacing = (available - span) / (span - 1);

            if (maxSpacing < 0)
            {
                return 0;
            }

            return (nfloat) Math.Min(requestedSpacing, maxSpacing);
        }

        public void ConstrainTo2(CGSize size)
        {
            var availableSpace = ScrollDirection == UICollectionViewScrollDirection.Vertical
                ? size.Width
                : size.Height;

            var spacing = (nfloat) (ScrollDirection == UICollectionViewScrollDirection.Vertical
                ? ItemsLayout.HorizontalItemSpacing
                : ItemsLayout.VerticalItemSpacing);

            spacing = ReduceSpacing(availableSpace, spacing, ItemsLayout.Span) * (ItemsLayout.Span - 1);
            ConstrainedDimension = (availableSpace - spacing) / ItemsLayout.Span;
            ConstrainedDimension = ConstrainedDimension > 1 ? (int) ConstrainedDimension : ConstrainedDimension;
        }

        public void UpdateCellConstraints(CGSize size)
        {
            //直接调用ConstrainTo会导致死循环，原因未知
            ConstrainTo2(size);
            if (_count == ItemsLayout.Span - 1)
            {
                DetermineCellSize();
            }

            _count = (_count + 1) % ItemsLayout.Span;

            UpdateCellConstraints();
        }
    }
}