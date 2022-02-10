#nullable enable
using System.Collections.Generic;
using System.ComponentModel;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using HandSchool.Controls;
using HandSchool.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Rect = Android.Graphics.Rect;
using View = Android.Views.View;

[assembly:ExportRenderer(typeof(TappableCollectionView), typeof(TappableCollectionViewRenderer))]

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
    public sealed partial class TappableCollectionViewRenderer : CollectionViewRenderer
    {
        public TappableCollectionViewRenderer(Context context) : base(context)
        {
            AddItemDecoration(new TappableCollectionViewRendererItemDecoration());
            SetClipChildren(false);
            SetClipToPadding(false);
            SetPadding(0, PlatformImplV2.Instance.Dip2Px(5), 0, 0);
            _managedListeners = new Dictionary<TappableItemContentView, (IOnClickListener?, IOnLongClickListener?)>();
        }

        private new TappableCollectionView Element => (TappableCollectionView) base.Element;

        protected override GroupableItemsViewAdapter<GroupableItemsView, IGroupableItemsViewSource> CreateAdapter()
        {
            return new TappableCollectionViewAdapter(Element);
        }

        private class TappableCollectionViewRendererItemDecoration : ItemDecoration
        {
            public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, State state)
            {
                base.GetItemOffsets(outRect, view, parent, state);
                outRect.Left = outRect.Right = PlatformImplV2.Instance.Dip2Px(10);
            }
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

        private bool _lastTappable;

        private bool LastTappable
        {
            set
            {
                if (_lastTappable == value) return;
                _lastTappable = value;
                if (_lastTappable)
                {
                    _managedListeners.Keys.ForEach(v =>
                    {
                        if (v.Child is { } c)
                        {
                            c.Click += OnChildClick;
                        }
                    });
                }
                else
                {
                    _managedListeners.Keys.ForEach(v =>
                    {
                        if (v.Child is { } c)
                        {
                            c.Click -= OnChildClick;
                        }
                    });
                }
            }
        }

        private bool ItemTappable => Element.SelectionOn || Element.HasTap;

        protected override void OnElementChanged(ElementChangedEventArgs<ItemsView> elementChangedEvent)
        {
            base.OnElementChanged(elementChangedEvent);
            _lastTappable = ItemTappable;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
        {
            base.OnElementPropertyChanged(sender, changedProperty);
            switch (changedProperty.PropertyName)
            {
                case "HasLongPress":
                {
                    if (Element.HasLongPress)
                    {
                        _managedListeners.Keys.ForEach(v =>
                        {
                            if (v.Child is { } c)
                            {
                                c.LongClick += OnChildLongClick;
                            }
                        });
                    }
                    else
                    {
                        _managedListeners.Keys.ForEach(v =>
                        {
                            if (v.Child is { } c)
                            {
                                c.LongClick -= OnChildLongClick;
                            }
                        });
                    }

                    break;
                }
                case "HasTap":
                case "SelectionOn":
                {
                    LastTappable = ItemTappable;
                    break;
                }
            }
        }
    }
}