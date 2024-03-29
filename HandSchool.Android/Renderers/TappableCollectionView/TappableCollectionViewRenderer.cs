﻿#nullable enable
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Views;
using HandSchool.Controls;
using HandSchool.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(TappableCollectionView), typeof(TappableCollectionViewRenderer))]

namespace HandSchool.Droid.Renderers
{
    public sealed class TappableItemContentView : ItemContentView
    {
        public View? Child => Content as View;

        public VisualElement? FormsView => ((IVisualElementRenderer?) Child)?.Element;

        public TappableItemContentView(Context context) : base(context)
        {
        }

        internal IOnClickListener? CurrentClickListener
        {
            get => _currentClickListener;
            set => SetOnClickListener(value);
        }

        private IOnClickListener? _currentClickListener;

        internal IOnLongClickListener? CurrentLongClickListener
        {
            get => _currentLongClickListener;
            set => SetOnLongClickListener(value);
        }

        private IOnLongClickListener? _currentLongClickListener;


        public override void SetOnClickListener(IOnClickListener? l)
        {
            base.SetOnClickListener(l);
            _currentClickListener = l;
        }

        public override void SetOnLongClickListener(IOnLongClickListener? l)
        {
            base.SetOnLongClickListener(l);
            _currentLongClickListener = l;
        }
    }

    /// <summary>
    /// 默认的CollectionViewRenderer会把ClickListener施加在ItemContentView上，而不是前景；
    /// 导致当前景启用Ripple特效时，Selection功能失效；
    /// </summary>
    public sealed partial class TappableCollectionViewRenderer : CollectionViewRenderer2
    {
        public TappableCollectionViewRenderer(Context context) : base(context)
        {
            _managedListeners = new Dictionary<TappableItemContentView, (IOnClickListener?, IOnLongClickListener?)>();
        }

        private new TappableCollectionView Element => (TappableCollectionView) base.Element;

        protected override GroupableItemsViewAdapter<GroupableItemsView, IGroupableItemsViewSource> CreateAdapter()
        {
            return new TappableCollectionViewAdapter(Element);
        }

        public override void AddView(View? child, int width, int height)
        {
            base.AddView(child, width, height);
            ViewAddedHandler(child);
        }

        public override void AddView(View? child, ViewGroup.LayoutParams? @params)
        {
            base.AddView(child, @params);
            ViewAddedHandler(child);
        }

        public override void AddView(View? child, int index, ViewGroup.LayoutParams? @params)
        {
            base.AddView(child, index, @params);
            ViewAddedHandler(child);
        }

        public override void AddView(View? child)
        {
            base.AddView(child);
            ViewAddedHandler(child);
        }

        public override void AddView(View? child, int index)
        {
            base.AddView(child, index);
            ViewAddedHandler(child);
        }

        public override void RemoveView(View? view)
        {
            ViewRemovedHandler(view);
            base.RemoveView(view);
        }

        public override void RemoveViews(int start, int count)
        {
            for (var i = 0; i < count; i++)
            {
                ViewRemovedHandler(GetChildAt(i + start));
            }

            base.RemoveViews(start, count);
        }

        public override void RemoveAllViews()
        {
            for (var i = 0; i < ChildCount; i++)
            {
                ViewRemovedHandler(GetChildAt(i));
            }

            base.RemoveAllViews();
        }

        public override void RemoveViewAt(int index)
        {
            ViewRemovedHandler(GetChildAt(index));
            base.RemoveViewAt(index);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ItemsView> elementChangedEvent)
        {
            base.OnElementChanged(elementChangedEvent);
            int ToPx(double d) => PlatformImplV2.Instance.Dip2Px((float) d);
            var ep = Element.Padding;
            SetPadding(ToPx(ep.Left), ToPx(ep.Top), ToPx(ep.Right), ToPx(ep.Bottom));
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
        {
            base.OnElementPropertyChanged(sender, changedProperty);
            switch (changedProperty.PropertyName)
            {
                case "SelectionOn":
                {
                    if (Element.SelectionOn)
                    {
                        _managedListeners.Keys.ToList().ForEach(v =>
                        {
                            if (!(v.Child is { } child)) return;
                            //由于Selection需要ClickListener实现
                            if (!Element.HasTap)
                            {
                                child.Click += OnChildClick;
                            }

                            if (Element.HasLongPress)
                            {
                                child.LongClick -= OnChildLongClick;
                            }
                        });
                    }
                    else
                    {
                        _managedListeners.Keys.ToList().ForEach(v =>
                        {
                            if (!(v.Child is { } child)) return;
                            if (!Element.HasTap)
                            {
                                child.Click -= OnChildClick;
                            }

                            if (Element.HasLongPress)
                            {
                                child.LongClick += OnChildLongClick;
                            }
                        });
                    }

                    break;
                }

                case "HasTap":
                {
                    if (Element.SelectionOn) return;
                    if (Element.HasTap)
                    {
                        _managedListeners.Keys.ToList().ForEach(v =>
                        {
                            if (v.Child is { } child)
                            {
                                child.Click += OnChildClick;
                            }
                        });
                    }
                    else
                    {
                        _managedListeners.Keys.ToList().ForEach(v =>
                        {
                            if (v.Child is { } child)
                            {
                                child.Click -= OnChildClick;
                            }
                        });
                    }

                    break;
                }

                case "HasLongPress":
                {
                    if (Element.SelectionOn) return;
                    if (Element.HasLongPress)
                    {
                        _managedListeners.Keys.ToList().ForEach(v =>
                        {
                            if (v.Child is { } child)
                            {
                                child.LongClick += OnChildLongClick;
                            }
                        });
                    }
                    else
                    {
                        _managedListeners.Keys.ToList().ForEach(v =>
                        {
                            if (v.Child is { } child)
                            {
                                child.LongClick -= OnChildLongClick;
                            }
                        });
                    }

                    break;
                }

                case "Padding":
                    int ToPx(double d) => PlatformImplV2.Instance.Dip2Px((float) d);
                    var ep = Element.Padding;
                    SetPadding(ToPx(ep.Left), ToPx(ep.Top), ToPx(ep.Right), ToPx(ep.Bottom));
                    break;
            }
        }
    }
}