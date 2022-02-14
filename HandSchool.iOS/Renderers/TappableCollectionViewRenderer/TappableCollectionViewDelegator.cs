#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using HandSchool.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using IndexRecognizer =
    System.ValueTuple<Foundation.NSIndexPath, UIKit.UITapGestureRecognizer?, UIKit.UILongPressGestureRecognizer?>;

namespace HandSchool.iOS.Renderers
{
    public class TappableCollectionViewDelegator : GroupableItemsViewDelegator<GroupableItemsView,
        GroupableItemsViewController<GroupableItemsView>>
    {
        public TappableCollectionViewDelegator(ItemsViewLayout itemsViewLayout,
            GroupableItemsViewController<GroupableItemsView> itemsViewController) : base(itemsViewLayout,
            itemsViewController)
        {
            _cellGestureRecognizers = new Dictionary<UICollectionViewCell, IndexRecognizer>();
            if (ItemsView is { })
            {
                ItemsView.PropertyChanged += ItemsViewPropertyChanged;
            }
        }

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            if (ItemsView is { })
            {
                ItemsView.PropertyChanged -= ItemsViewPropertyChanged;
            }

            base.Dispose(disposing);
        }

        private void ItemsViewPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (!(sender is TappableCollectionView v)) return;
            switch (args.PropertyName)
            {
                case "SelectionOn":
                {
                    if (v.SelectionOn)
                    {
                        _cellGestureRecognizers.Keys.ToList().ForEach(CellWillDisappearHandler);
                    }
                    else
                    {
                        _cellGestureRecognizers.Keys.ToList().ForEach(c => CellWillDisplayHandler(c));
                    }

                    break;
                }

                case "HasTap":
                {
                    if (v.HasTap)
                    {
                        _cellGestureRecognizers.ToList().ForEach(c =>
                        {
                            var (index, tap, longPress) = c.Value;
                            c.Key.AddGestureRecognizer(tap = new UITapGestureRecognizer(OnItemTapped));
                            _cellGestureRecognizers[c.Key] = (index, tap, longPress);
                        });
                    }
                    else
                    {
                        _cellGestureRecognizers.ToList().ForEach(c =>
                        {
                            var (index, tap, longPress) = c.Value;
                            c.Key.RemoveGestureRecognizer(tap!);
                            tap = null;
                            _cellGestureRecognizers[c.Key] = (index, tap, longPress);
                        });
                    }

                    break;
                }

                case "HasLongPress":
                {
                    if (v.HasLongPress)
                    {
                        _cellGestureRecognizers.ToList().ForEach(c =>
                        {
                            var (index, tap, longPress) = c.Value;
                            c.Key.AddGestureRecognizer(longPress = new UILongPressGestureRecognizer(OnItemLongPress));
                            _cellGestureRecognizers[c.Key] = (index, tap, longPress);
                        });
                    }
                    else
                    {
                        _cellGestureRecognizers.ToList().ForEach(c =>
                        {
                            var (index, tap, longPress) = c.Value;
                            c.Key.RemoveGestureRecognizer(longPress!);
                            longPress = null;
                            _cellGestureRecognizers[c.Key] = (index, tap, longPress);
                        });
                    }

                    break;
                }
            }
        }

        private readonly Dictionary<UICollectionViewCell, IndexRecognizer> _cellGestureRecognizers;

        /// <summary>
        /// 当Cell出现时的处理步骤，为它附加点击和长按识别
        /// </summary>
        private void CellWillDisplayHandler(UICollectionViewCell cell, NSIndexPath? indexPath = null)
        {
            var (_, tap, longPress) = _cellGestureRecognizers[cell] =
                (indexPath ?? _cellGestureRecognizers[cell].Item1,
                    ItemsView?.HasTap == true ? new UITapGestureRecognizer(OnItemTapped) : null,
                    ItemsView?.HasLongPress == true ? new UILongPressGestureRecognizer(OnItemLongPress) : null);
            if (tap is { })
            {
                cell.AddGestureRecognizer(tap);
            }

            if (longPress is { })
            {
                cell.AddGestureRecognizer(longPress);
            }
        }

        /// <summary>
        /// 当Cell移出屏幕时的处理步骤，删除手势识别器
        /// </summary>
        /// <param name="cell"></param>
        private void CellWillDisappearHandler(UICollectionViewCell cell)
        {
            if (!_cellGestureRecognizers.ContainsKey(cell)) return;
            var (_, tap, longPress) = _cellGestureRecognizers[cell];
            if (tap is { })
            {
                cell.RemoveGestureRecognizer(tap);
            }

            if (longPress is { })
            {
                cell.RemoveGestureRecognizer(longPress);
            }
        }

        public override void CellDisplayingEnded(UICollectionView collectionView, UICollectionViewCell cell,
            NSIndexPath indexPath)
        {
            base.CellDisplayingEnded(collectionView, cell, indexPath);
            if (ItemsView is {SelectionOn: false})
            {
                CellWillDisappearHandler(cell);
            }

            _cellGestureRecognizers.Remove(cell);
        }

        public override void WillDisplayCell(UICollectionView collectionView, UICollectionViewCell cell,
            NSIndexPath indexPath)
        {
            if (_cellGestureRecognizers.ContainsKey(cell)) return;
            _cellGestureRecognizers[cell] = (indexPath, null, null);
            if (ItemsView is {SelectionOn: false})
            {
                CellWillDisplayHandler(cell, indexPath);
            }
        }

        private static IVisualElementRenderer? GetRendererFromCell(UIView? cell)
        {
            //硬编码
            if (cell is null) return null;
            var subViews = cell.Subviews;
            if (subViews.Length < 2) return null;
            var subView = subViews[1];
            if (subView.Subviews.Length < 1) return null;
            return subView.Subviews[0] as IVisualElementRenderer;
        }
        
        private void OnItemLongPress(UIGestureRecognizer recognizer)
        {
            if (recognizer.State != UIGestureRecognizerState.Began) return;
            if (ItemsView is null) return;
            if (ViewController is null) return;
            if (!(recognizer.View is UICollectionViewCell cell)) return;
            var indexPath = _cellGestureRecognizers[cell].Item1;
            var item = ViewController.GetItemFromIndex(indexPath);
            var act = new Action(() => ItemsView?.CallOnItemLongPress(item,
                new CollectionItemTappedEventArgs.IndexPath(indexPath.Section, (int) indexPath.Item)));
            if (ItemsView.UseScaleAnimation)
            {
                if (GetRendererFromCell(recognizer.View) is { } element)
                {
                    element.Element.LongPressAnimation(async () =>
                    {
                        await Task.Yield();
                        Device.BeginInvokeOnMainThread(act);
                    });
                }
            }
            else
            {
                act();
            }
        }

        private void OnItemTapped(UIGestureRecognizer recognizer)
        {
            if (ItemsView is null) return;
            if (ViewController is null) return;
            if (!(recognizer.View is UICollectionViewCell cell)) return;
            var indexPath = _cellGestureRecognizers[cell].Item1;
            var item = ViewController.GetItemFromIndex(indexPath);
            var act = new Action(() => ItemsView?.CallOnItemTapped(item,
                new CollectionItemTappedEventArgs.IndexPath(indexPath.Section, (int) indexPath.Item)));
            if (ItemsView.UseScaleAnimation)
            {
                if (GetRendererFromCell(recognizer.View) is { } element)
                {
                    element.Element.TappedAnimation(async () =>
                    {
                        await Task.Yield();
                        Device.BeginInvokeOnMainThread(act);
                    });
                }
            }
            else
            {
                act();
            }
        }

        private new TappableCollectionViewController? ViewController =>
            base.ViewController as TappableCollectionViewController;

        private TappableCollectionView? ItemsView => ViewController?.ItemsView as TappableCollectionView;
    }
}