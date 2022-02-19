#nullable enable
using Foundation;
using HandSchool.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace HandSchool.iOS.Renderers
{
    public class TappableCollectionViewController : GroupableItemsViewController<GroupableItemsView>
    {
        public TappableCollectionViewController(GroupableItemsView groupableItemsView, ItemsViewLayout layout) : base(
            groupableItemsView, layout)
        {
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (UICollectionViewCell) collectionView.DequeueReusableCell(DetermineCellReuseId(), indexPath);
            var span = ItemsView!.ItemsLayout is GridItemsLayout gridItemsLayout ? gridItemsLayout.Span : 1;

            switch (cell)
            {
                case DefaultCell defaultCell:
                    UpdateDefaultCell(defaultCell, indexPath);
                    break;

                case MarginCell marginCell:
                    marginCell.SetCollectionViewPadding(new MarginInfo {Thickness = ItemsView!.Padding, Span = span});
                    UpdateTemplatedCell(marginCell, indexPath);
                    break;
            }

            return cell;
        }

        protected override void RegisterViewTypes()
        {
            base.RegisterViewTypes();
            CollectionView.RegisterClassForCell(typeof(VerticalMarginCell), VerticalMarginCell.ReuseId);
            CollectionView.RegisterClassForCell(typeof(HorizontalMarginCell), HorizontalMarginCell.ReuseId);
        }

        protected override string DetermineCellReuseId()
        {
            if (ItemsView?.ItemTemplate is null) return base.DetermineCellReuseId();
            return ItemsViewLayout.ScrollDirection == UICollectionViewScrollDirection.Horizontal
                ? HorizontalMarginCell.ReuseId
                : VerticalMarginCell.ReuseId;
        }

        private new TappableCollectionView? ItemsView => base.ItemsView as TappableCollectionView;

        protected override UICollectionViewDelegateFlowLayout CreateDelegator()
        {
            return new TappableCollectionViewDelegator(ItemsViewLayout, this);
        }

        internal object GetItemFromIndex(NSIndexPath indexPath)
        {
            return GetItemAtIndex(indexPath);
        }
    }
}