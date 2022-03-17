using System;
using System.Collections;
using System.Linq;
using HandSchool.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using IPath = HandSchool.Models.CollectionItemTappedEventArgs.IndexPath;

namespace HandSchool.Controls
{
    public class TappableCollectionView : CollectionView, IPaddingElement
    {
        public TappableCollectionView()
        {
            _lastSelectOn = SelectionOn;
        }

        public event EventHandler<CollectionItemTappedEventArgs> ItemTapped
        {
            add
            {
                var before = _itemTapped is null;
                _itemTapped += value;
                if (before) OnPropertyChanged(nameof(HasTap));
            }

            remove
            {
                var before = _itemTapped is null;
                _itemTapped -= value;
                if (_itemTapped is null != before) OnPropertyChanged(nameof(HasTap));
            }
        }

        private EventHandler<CollectionItemTappedEventArgs> _itemTapped;

        public event EventHandler<CollectionItemTappedEventArgs> ItemLongPress
        {
            add
            {
                var before = _itemLongPress is null;
                _itemLongPress += value;
                if (before) OnPropertyChanged(nameof(HasLongPress));
            }

            remove
            {
                var before = _itemLongPress is null;
                _itemLongPress -= value;
                if (_itemLongPress is null != before) OnPropertyChanged(nameof(HasLongPress));
            }
        }

        private EventHandler<CollectionItemTappedEventArgs> _itemLongPress;

        public bool HasLongPress => _itemLongPress is { };

        public bool HasTap => _itemTapped is { };
        
        public bool SelectionOn => SelectionMode != SelectionMode.None;

        private bool _lastSelectOn;

        /// <summary>
        /// 使用缩放动画，当执行ItemTapped或ItemLongPress时会附加一个缩放的动画效果以提示用户点击所在之处
        /// </summary>
        public bool UseScaleAnimation
        {
            get => (bool) GetValue(UseScaleAnimationProperty);
            set => SetValue(UseScaleAnimationProperty, value);
        }

        /// <summary>
        /// 动画互斥，当UseScaleAnimation为True时，互斥ItemTapped及其附加动画的执行
        /// </summary>
        public bool AnimationMutex
        {
            get => (bool) GetValue(AnimationMutexProperty);
            set => SetValue(AnimationMutexProperty, value);
        }

        public static readonly BindableProperty UseScaleAnimationProperty = BindableProperty.Create(
            propertyName: nameof(UseScaleAnimation),
            returnType: typeof(bool),
            declaringType: typeof(TappableCollectionView),
            defaultValue: false);

        public static readonly BindableProperty AnimationMutexProperty = BindableProperty.Create(
            propertyName: nameof(AnimationMutex),
            returnType: typeof(bool),
            declaringType: typeof(TappableCollectionView),
            defaultValue: false);

        private CollectionItemTappedEventArgs FindItem(object item, CollectionItemTappedEventArgs.IndexPath? path)
        {
            if (IsGrouped)
            {
                var groupIndex = -1;
                var indexInGroup = -1;
                var group = (path is { } && ItemsSource is IList list
                    ? list[path.Value.GroupIndex]
                    : ItemsSource.Cast<object>()
                        .FirstOrDefault(o =>
                        {
                            groupIndex++;
                            if (!(o is IEnumerable enumerable)) return false;
                            indexInGroup = enumerable.Cast<object>().IndexOf(item);
                            return indexInGroup != -1;
                        })) as IEnumerable;

                return new CollectionItemTappedEventArgs
                {
                    Item = item,
                    Group = group,
                    Path = path ?? new IPath(groupIndex, indexInGroup)
                };
            }
            else
            {
                return new CollectionItemTappedEventArgs
                {
                    Item = item,
                    Group = ItemsSource,
                    Path = path ?? new IPath(0, ItemsSource.Cast<object>().IndexOf(item))
                };
            }
        }

        public void CallOnItemTapped(object item, IPath? index)
        {
            _itemTapped?.Invoke(this, FindItem(item, index));
        }

        public void CallOnItemLongPress(object item, IPath? index)
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

        public void OnPaddingPropertyChanged(Thickness oldValue, Thickness newValue)
        {
            InvalidateMeasureNonVirtual(InvalidationTrigger.MeasureChanged);
        }

        public Thickness PaddingDefaultValueCreator()
        {
            return (Thickness) PaddingProperty.DefaultValue;
        }

        public Thickness Padding
        {
            get => (Thickness) GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        private static readonly BindableProperty PaddingProperty = BindableProperty.Create(
            propertyName: nameof(Padding),
            returnType: typeof(Thickness),
            declaringType: typeof(TappableCollectionView),
            defaultValue: new Thickness(10, 6, 10, 6));
    }
}