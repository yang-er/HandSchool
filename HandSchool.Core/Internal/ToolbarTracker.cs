using HandSchool.Views;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace HandSchool.Internals
{
    /// <summary>
    /// 用于跟踪任务栏菜单改变。
    /// </summary>
    public class ToolbarMenuTracker
    {
        ObservableCollection<MenuEntry> _inner;

        private void SubItemChanged(object sender, PropertyChangedEventArgs args)
        {
            Changed?.Invoke(sender, args);
        }

        private void ListItemListening(object sender, NotifyCollectionChangedEventArgs args)
        {
            Changed?.Invoke(sender, args);

            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (MenuEntry toAdd in args.NewItems)
                    toAdd.PropertyChanged += SubItemChanged;
            }
            else
            {
                foreach (MenuEntry toDel in args.OldItems)
                    toDel.PropertyChanged -= SubItemChanged;
            }
        }

        /// <summary>
        /// 保存的列表
        /// </summary>
        public ObservableCollection<MenuEntry> List
        {
            get
            {
                return _inner;
            }

            set
            {
                if (_inner != null)
                {
                    _inner.CollectionChanged -= ListItemListening;
                    foreach (var item in _inner)
                        item.PropertyChanged -= SubItemChanged;
                }

                _inner = value;

                if (value != null)
                {
                    value.CollectionChanged += ListItemListening;
                    foreach (var item in _inner)
                        item.PropertyChanged += SubItemChanged;
                }

                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 菜单改变时发生。
        /// </summary>
        public event EventHandler Changed;
    }
}