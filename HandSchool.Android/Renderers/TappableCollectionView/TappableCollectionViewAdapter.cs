#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using HandSchool.Controls;
using HandSchool.Droid.Internals;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using ItemViewType = Xamarin.Forms.Platform.Android.ItemViewType;
using JavaObject = Java.Lang.Object;
using XFView = Xamarin.Forms.View;
using AView = Android.Views.View;

namespace HandSchool.Droid.Renderers
{
    public sealed class LongPressableViewHolder : TemplatedItemViewHolder, AView.IOnLongClickListener
    {
        public event EventHandler<int>? LongClicked;

        private void OnViewHolderLongClicked(int adapterPosition)
        {
            var longClicked = LongClicked;
            longClicked?.Invoke(this, adapterPosition);
        }

        public LongPressableViewHolder(ItemContentView itemView, DataTemplate dataTemplate,
            bool isSelectionEnabled = true) : base(itemView, dataTemplate, isSelectionEnabled)
        {
            if (itemView is TappableItemContentView)
            {
                itemView.SetOnLongClickListener(this);
            }
        }

        public bool OnLongClick(AView? v)
        {
            OnViewHolderLongClicked(BindingAdapterPosition);
            return true;
        }
    }

    public sealed class TappableCollectionViewAdapter :
        GroupableItemsViewAdapter<GroupableItemsView, IGroupableItemsViewSource>
    {
        internal TappableCollectionViewAdapter(GroupableItemsView groupableItemsView,
            Func<XFView, Context, ItemContentView>? createView = null)
            : base(groupableItemsView, createView ?? ((view, context) => new TappableItemContentView(context)))
        {
            _viewHolders = new HashSet<LongPressableViewHolder>();
            ItemsView.PropertyChanged += ItemsViewSelectionModeChanged;
            _createNativeContentView =
                (createView ??
                 GetType().GetRunTimeAllField("_createItemContentView")?.GetValue(this)
                     as Func<XFView, Context, ItemContentView>)
                ?? new Func<XFView, Context, ItemContentView>((view, context) => new TappableItemContentView(context));
        }

        private readonly Func<XFView, Context, ItemContentView> _createNativeContentView;

        private void ItemsViewSelectionModeChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                //选择功能与轻按功能不可同时开启，但与长按功能可以同时开启
                case "SelectionOn":
                {
                    if (ItemsView.SelectionOn)
                    {
                        _viewHolders.ForEach(v => v.Clicked -= ClickInvoker);
                    }
                    else
                    {
                        _viewHolders.ForEach(v => v.Clicked += ClickInvoker);
                    }

                    break;
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var baseAccess = new[]
            {
                ItemViewType.Header,
                ItemViewType.Footer,
                ItemViewType.TextItem
            };
            if (baseAccess.Any(t => t == viewType))
            {
                return base.OnCreateViewHolder(parent, viewType);
            }

            var context = parent.Context;
            if (viewType == ItemViewType.GroupHeader || viewType == ItemViewType.GroupFooter)
            {
                var itemContentView = new ItemContentView(context);
                return new LongPressableViewHolder(itemContentView, ItemsView.GroupHeaderTemplate, false);
            }
            else
            {
                var itemContentView = _createNativeContentView.Invoke(ItemsView, context!);
                ContentViews.Add(itemContentView);
                return new LongPressableViewHolder(itemContentView, ItemsView.ItemTemplate);
            }
        }

        private new TappableCollectionView ItemsView => (TappableCollectionView) base.ItemsView;

        private async void LongClickInvoker(object sender, int pos)
        {
            if (!ItemsView.HasLongPress) return;
            if (ItemsView.UseScaleAnimation)
            {
                await ((((RecyclerView.ViewHolder) sender).ItemView as TappableItemContentView)?.FormsView
                    ?.LongPressAnimation(async () =>
                    {
                        await Task.Yield();
                        ItemsView.CallOnItemLongPress(ItemsSource.GetItem(pos), pos);
                    }) ?? Task.CompletedTask);
            }
            else
            {
                ItemsView.CallOnItemLongPress(ItemsSource.GetItem(pos), pos);
            }
        }

        private async void ClickInvoker(object sender, int pos)
        {
            if (!ItemsView.HasTap) return;
            if (ItemsView.UseScaleAnimation)
            {
                await ((((RecyclerView.ViewHolder) sender).ItemView as TappableItemContentView)?.FormsView
                    ?.TappedAnimation(async () =>
                    {
                        await Task.Yield();
                        ItemsView.CallOnItemTapped(ItemsSource.GetItem(pos), pos);
                    }) ?? Task.CompletedTask);
            }
            else
            {
                ItemsView.CallOnItemTapped(ItemsSource.GetItem(pos), pos);
            }
        }


        private readonly HashSet<LongPressableViewHolder> _viewHolders;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            base.OnBindViewHolder(holder, position);
            if (!(holder is LongPressableViewHolder viewHolder)) return;
            if (_viewHolders.Contains(viewHolder)) return;
            _viewHolders.Add(viewHolder);
            if (!ItemsView.SelectionOn)
            {
                viewHolder.Clicked += ClickInvoker;
            }

            viewHolder.LongClicked += LongClickInvoker;
        }

        public override void OnViewRecycled(JavaObject holder)
        {
            base.OnViewRecycled(holder);
            if (!(holder is LongPressableViewHolder viewHolder)) return;
            if (!_viewHolders.Contains(viewHolder)) return;
            _viewHolders.Remove(viewHolder);
            if (!ItemsView.SelectionOn)
            {
                viewHolder.Clicked -= ClickInvoker;
            }

            viewHolder.LongClicked -= LongClickInvoker;
        }
    }
}