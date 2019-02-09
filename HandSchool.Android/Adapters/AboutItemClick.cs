using DanielStone.MaterialAbout.Items;
using System;

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
}