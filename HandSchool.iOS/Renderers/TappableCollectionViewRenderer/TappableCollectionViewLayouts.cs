#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

namespace HandSchool.iOS.Renderers
{
    public class TappableGridViewLayout : GridViewLayout
    {
        public TappableGridViewLayout(
            GridItemsLayout itemsLayout,
            CollectionViewRenderer2 renderer,
            ItemSizingStrategy itemSizingStrategy) : base(
            itemsLayout, itemSizingStrategy)
        {
            _layout = itemsLayout;
            _renderer = renderer;
            _map = new Dictionary<nfloat, nfloat>();
        }

        private readonly CollectionViewRenderer2 _renderer;

        private readonly GridItemsLayout _layout;

        private readonly Dictionary<nfloat, nfloat> _map;

        private HashSet<nfloat>? _caches;

        private double _lastSpacing;

        private static readonly Func<nfloat, nfloat, int, nfloat>? FuncReduceSpacingToFitIfNeeded;

        static TappableGridViewLayout()
        {
            var mi = typeof(GridViewLayout).GetDeclaredMethod("ReduceSpacingToFitIfNeeded", typeof(nfloat),
                typeof(nfloat), typeof(int));
            if (mi is { })
            {
                FuncReduceSpacingToFitIfNeeded = (a, b, c) => (nfloat) mi.Invoke(null, new object[] {a, b, c});
            }
        }

        private UICollectionViewLayoutAttributes[] SolveHorizontal(UICollectionViewLayoutAttributes[] attributes)
        {
            var span = _layout.Span;
            if (attributes.Length <= span) return attributes;
            
            //获取真实的项目间距，横向排列应该注重纵向间距
            if (_layout.VerticalItemSpacing != 0 && FuncReduceSpacingToFitIfNeeded is null)
                throw new NotSupportedException("Program cannot work");

            var spacing = _layout.VerticalItemSpacing == 0
                ? 0
                : FuncReduceSpacingToFitIfNeeded!(_renderer.Frame.Height, (nfloat) _layout.VerticalItemSpacing, span);

            //判断间距是否发生改变，否则将重新计算坐标
            if (Math.Abs(_lastSpacing - spacing) > 1e-5)
            {
                _caches?.Clear();
                _caches = null;
                _map.Clear();
                _lastSpacing = spacing;
            }

            //计算坐标
            if (_caches is null)
            {
                var padding = ((IPaddingElement) ((IVisualElementRenderer) _renderer).Element).Padding;
                var viewHeight = _renderer.Frame.Height;
                var should = (viewHeight - padding.VerticalThickness - (span - 1) * spacing) / span;
                var cache = new nfloat[span];
                var start = cache[0] = (nfloat) padding.VerticalThickness / 2;
                for (var i = 1; i < span; i++)
                {
                    cache[i] = start + i * (nfloat) (should + spacing);
                }

                _caches = cache.ToHashSet();
            }

            //找出距离当前坐标最近的坐标，更新坐标
            attributes.ForEach(a =>
            {
                var y = a.Frame.Y;
                if (_caches.Contains(y)) return;
                if (!_map.TryGetValue(y, out var newY))
                {
                    var copy = _caches.ToArray();
                    Array.Sort(copy, (n, m) =>
                    {
                        var res = Math.Abs(n - y) - Math.Abs(m - y);
                        if (res == 0) return 0;
                        return res > 0 ? 1 : -1;
                    });
                    newY = copy[0];
                    _map.Add(y, newY);
                }

                var frame = a.Frame;
                frame.Y = newY;
                a.Frame = frame;
            });

            return attributes;
        }

        private UICollectionViewLayoutAttributes[] SolveVertical(UICollectionViewLayoutAttributes[] attributes)
        {
            var span = _layout.Span;
            if (attributes.Length <= span) return attributes;
            if (_layout.HorizontalItemSpacing != 0 && FuncReduceSpacingToFitIfNeeded is null)
                throw new NotSupportedException("Program cannot work");

            var spacing = _layout.HorizontalItemSpacing == 0
                ? 0
                : FuncReduceSpacingToFitIfNeeded!(_renderer.Frame.Width, (nfloat) _layout.HorizontalItemSpacing, span);

            if (Math.Abs(_lastSpacing - spacing) > 1e-5)
            {
                _caches?.Clear();
                _caches = null;
                _map.Clear();
                _lastSpacing = spacing;
            }

            if (_caches is null)
            {
                var padding = ((IPaddingElement) ((IVisualElementRenderer) _renderer).Element).Padding;
                var viewWidth = _renderer.Frame.Width;
                var should = (viewWidth - padding.HorizontalThickness - (span - 1) * spacing) / span;
                var cache = new nfloat[span];
                var start = cache[0] = (nfloat) padding.HorizontalThickness / 2;
                for (var i = 1; i < span; i++)
                {
                    cache[i] = start + i * (nfloat) (should + spacing);
                }

                _caches = cache.ToHashSet();
            }

            attributes.ForEach(a =>
            {
                var x = a.Frame.X;
                if (_caches.Contains(x)) return;
                if (!_map.TryGetValue(x, out var newX))
                {
                    var copy = _caches.ToArray();
                    Array.Sort(copy, (n, m) =>
                    {
                        var res = Math.Abs(n - x) - Math.Abs(m - x);
                        if (res == 0) return 0;
                        return res > 0 ? 1 : -1;
                    });
                    newX = copy[0];
                    _map.Add(x, newX);
                }

                var frame = a.Frame;
                frame.X = newX;
                a.Frame = frame;
            });

            return attributes;
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
        {
            return (_layout.Orientation ==
                    ItemsLayoutOrientation.Horizontal
                ? (Func<UICollectionViewLayoutAttributes[], UICollectionViewLayoutAttributes[]>) SolveHorizontal
                : SolveVertical).Invoke(base.LayoutAttributesForElementsInRect(rect));
        }
    }
}