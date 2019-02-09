using Android.Views;
using HandSchool.Views;
using System;
using System.Windows.Input;

namespace HandSchool.Droid
{
    /// <summary>
    /// <see cref="ICommand"/> 点击的事件监听
    /// </summary>
    public class MenuEntryClickedListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
    {
        /// <summary>
        /// 为 <see cref="MenuEntry"/> 实例建立监听器。
        /// </summary>
        /// <param name="item"></param>
        public MenuEntryClickedListener(MenuEntry item)
        {
            Reference = new WeakReference<ICommand>(item.Command);
        }

        /// <summary>
        /// 为 <see cref="MenuEntry"/> 实例建立监听器。
        /// </summary>
        /// <param name="item"></param>
        public MenuEntryClickedListener(ICommand item)
        {
            Reference = new WeakReference<ICommand>(item);
        }

        /// <summary>
        /// 菜单项的弱引用
        /// </summary>
        private WeakReference<ICommand> Reference { get; }

        /// <summary>
        /// Java内部的事件接口。
        /// </summary>
        /// <param name="item">菜单项</param>
        /// <returns>是否处理成功</returns>
        public bool OnMenuItemClick(IMenuItem item)
        {
            if (Reference.TryGetTarget(out var target))
            {
                target?.Execute(null);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}