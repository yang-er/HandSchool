using System;
using System.Collections;
using System.Linq;
using Xamarin.Forms;

namespace HandSchool.Controls
{
    public class TappableCollectionView : CollectionView
    {
        public TappableCollectionView()
        {
            _lastSelectOn = SelectionOn;
        }
        
        public event EventHandler<ItemTappedEventArgs> ItemTapped
        {
            add
            {
                var before = _itemTapped is null;
                _itemTapped += value;
                if(before) OnPropertyChanged(nameof(HasTap));
            }

            remove
            {
                var before = _itemTapped is null;
                _itemTapped -= value;
                if(_itemTapped is null != before) OnPropertyChanged(nameof(HasTap));
            }
        }
        
        private EventHandler<ItemTappedEventArgs> _itemTapped;
        
        public event EventHandler<ItemTappedEventArgs> ItemLongPress
        {
            add
            {
                var before = _itemLongPress is null;
                _itemLongPress += value;
                if(before) OnPropertyChanged(nameof(HasLongPress));
            }

            remove
            {
                var before = _itemLongPress is null;
                _itemLongPress -= value;
                if(_itemLongPress is null != before) OnPropertyChanged(nameof(HasLongPress));
            }
        }
        
        private EventHandler<ItemTappedEventArgs> _itemLongPress;

        public bool HasLongPress => _itemLongPress is { };
        
        public bool HasTap => _itemTapped is { };

        public bool SelectionOn => SelectionMode != SelectionMode.None;

        private bool _lastSelectOn;
        
        public bool UseScaleAnimation
        {
            get => (bool)GetValue(UseScaleAnimationProperty);
            set => SetValue(UseScaleAnimationProperty, value);
        }

        public static readonly BindableProperty UseScaleAnimationProperty = BindableProperty.Create(
                propertyName: nameof(UseScaleAnimation),
                returnType: typeof(bool),
                declaringType: typeof(TappableCollectionView),
                defaultValue: false);

        private ItemTappedEventArgs FindItem(object item, int index)
        {
            return IsGrouped switch
            {
                true => new ItemTappedEventArgs(ItemsSource, item, index),
                _ => new ItemTappedEventArgs(ItemsSource.Cast<object>()
                    .FirstOrDefault(o =>
                    {
                        if (!(o is IEnumerable enumerable)) return false;
                        return enumerable.Cast<object>().Any(i => ReferenceEquals(i, item));
                    }), item, index)
            };
        }

        public void CallOnItemTapped(object item, int index)
        {
            _itemTapped?.Invoke(this, FindItem(item, index));
        }

        public void CallOnItemLongPress(object item, int index)
        {
            _itemLongPress?.Invoke(this, FindItem(item, index));
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(SelectionMode):
                {
                    if (_lastSelectOn == SelectionOn) return;
                    _lastSelectOn = SelectionOn;
                    OnPropertyChanged(nameof(SelectionOn));
                    break;
                }
            }
        }
    }
}