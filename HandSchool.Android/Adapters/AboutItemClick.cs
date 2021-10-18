using DanielStone.MaterialAbout.Items;
using System;
using System.Threading.Tasks;
using Android.Views;

namespace HandSchool.Droid
{
    /// <summary>
    /// 关于项的点击事件监听
    /// </summary>
    public class AboutMenuItemClick : Java.Lang.Object, IMaterialAboutItemOnClickAction
    {
        /// <summary>
        /// 点击事件的处理函数
        /// </summary>
        private Action Clicked { get; }

        /// <summary>
        /// 用 <see cref="Action"/> 构造关于项的点击事件监听器。
        /// </summary>
        /// <param name="action">执行函数</param>
        public AboutMenuItemClick(Action action = null)
        {
            Clicked = action;
        }

        /// <summary>
        /// Java内部的事件接口。
        /// </summary>
        public void OnClick()
        {
            Clicked?.Invoke();
        }
    }

    /// <summary>
        /// 安卓本机点击事件的包装类
        /// </summary>
        class ClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private readonly Action<View> _action;
            private readonly Func<View, Task> _asyncAction;
    
            public ClickListener(Action<View> action)
            {
                _action = action;
            }
    
            public ClickListener(Func<View, Task> asyncAction)
            {
                _asyncAction = asyncAction;
            }
            public async void OnClick(View v)
            {
                if (!(_asyncAction is null))
                {
                    await _asyncAction?.Invoke(v);
                }
                else
                {
                    if (!(_action is null))
                    {
                        _action.Invoke(v);
                    }
                    else return;
                }
            }
        }

    /// <summary>
    /// 安卓长按事件包装类
    /// </summary>
    class LongClickListener : Java.Lang.Object, View.IOnLongClickListener
    {
        private readonly Action<View> _action;
        private readonly Func<View, Task> _asyncAction;

        public LongClickListener(Action<View> action)
        {
            _action = action;
        }

        public LongClickListener(Func<View, Task> asyncAction)
        {
            _asyncAction = asyncAction;
        }

        public bool OnLongClick(View v)
        {
            if (!(_asyncAction is null))
            {
                _asyncAction?.Invoke(v);
                return true;
            }
            else
            {
                if (!(_action is null))
                {
                    _action?.Invoke(v);
                    return true;
                }
            }

            return false;
        }
    }
}