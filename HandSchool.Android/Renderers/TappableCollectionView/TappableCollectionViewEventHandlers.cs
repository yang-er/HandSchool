#nullable enable
using System;
using System.Collections.Generic;
using Android.Views;
using HandSchool.Droid.Internals;

namespace HandSchool.Droid.Renderers
{
    public sealed partial class TappableCollectionViewRenderer
    {
        private readonly Dictionary<TappableItemContentView, ValueTuple<IOnClickListener?, IOnLongClickListener?>> _managedListeners;
        
        private void OnChildClick(object s, EventArgs e)
        {
            if (!(((View) s).Parent is TappableItemContentView contentView)) return;
            if (_managedListeners.ContainsKey(contentView))
            {
                _managedListeners[contentView].Item1?.OnClick(contentView);
            }
        }

        private void OnChildLongClick(object s, EventArgs e)
        {
            if (!(((View) s).Parent is TappableItemContentView contentView)) return;
            if (_managedListeners.ContainsKey(contentView))
            {
                _managedListeners[contentView].Item2?.OnLongClick(contentView);
            }
        }

        private void ViewAddedHandler(View? view)
        {
            if (!(view is TappableItemContentView viewGroup)) return;
            if (_managedListeners.ContainsKey(viewGroup)) return;
            if (!(viewGroup.Child is { } target)) return;
            
            _managedListeners[viewGroup] = (viewGroup.CurrentClickListener, viewGroup.CurrentLongClickListener);
            (viewGroup.CurrentClickListener, viewGroup.CurrentLongClickListener) = (null, null);
            if (Element.HasLongPress)
            {
                target.LongClick += OnChildLongClick;
            }

            if (ItemTappable)
            {
                target.Click += OnChildClick;
            }

            target.TryAddRippleAnimation();
        }

        private void ViewRemovedHandler(View? view)
        {
            if (!(view is TappableItemContentView viewGroup)) return;
            if (!_managedListeners.ContainsKey(viewGroup)) return;
            if (!(viewGroup.Child is { } target)) return;

            if (ItemTappable)
            {
                target.Click -= OnChildClick;
            }

            if (Element.HasLongPress)
            {
                target.LongClick -= OnChildLongClick;
            }
            (viewGroup.CurrentClickListener, viewGroup.CurrentLongClickListener) = _managedListeners[viewGroup];
            _managedListeners.Remove(viewGroup);
        }
    }
}