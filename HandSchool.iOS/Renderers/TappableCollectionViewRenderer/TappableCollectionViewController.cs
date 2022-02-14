#nullable enable
using System;
using CoreGraphics;
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

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            AddPadding();
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            AddPadding();
        }

        private void UpdatePadding(Thickness padding)
        {
            if (ItemsView is null) return;
            var top = new nfloat(padding.Top);
            var button = new nfloat(padding.Bottom);
            var left = new nfloat(padding.Left);
            var right = new nfloat(padding.Right);
            CollectionView.ContentInset = new UIEdgeInsets(top, left, button, right);
        }

        /// <summary>
        /// 根据设置的Padding更新约束
        /// </summary>
        private void AddPadding()
        {
            // 约束不能提前设置，因为官方渲染器在ViewWillAppear和ViewWillLayoutSubviews都会重新计算约束
            if (ItemsView is null) return;
            var ei = CollectionView.ContentInset;
            var padding = ItemsView.Padding;
            var deltaX = new nfloat(padding.Left + padding.Right);

            //当Footer或Header存在时，如果设置Padding会导致死循环
            if (Math.Abs(ei.Top) < 1e-5 && Math.Abs(ei.Bottom) < 1e-5)
            {
                UpdatePadding(padding);
            }

            var itemsViewWidth = ItemsView.Width - deltaX;
            var itemsViewHeight = ItemsView.Height;

            CGSize size;
            if (itemsViewHeight < 0 || itemsViewWidth < 0)
            {
                size = CollectionView.Bounds.Size;
                size.Width -= ei.Left + ei.Right;
            }
            else
            {
                size = new CGSize(itemsViewWidth, itemsViewHeight);
            }

            if (ItemsViewLayout is ITappableItemsViewLayout layout)
            {
                layout.UpdateCellConstraints(size);
            }
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