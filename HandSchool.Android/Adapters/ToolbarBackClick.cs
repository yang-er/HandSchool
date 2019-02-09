using Android.Support.V4.App;
using Android.Views;
using System;
using JObject = Java.Lang.Object;

namespace HandSchool.Droid
{
    /// <summary>
    /// 工具栏返回点击事件监听
    /// </summary>
    public class ToolbarBackListener : JObject, View.IOnClickListener
    {
        /// <summary>
        /// 工具栏所在的 <see cref="FragmentActivity"/>
        /// </summary>
        public WeakReference<FragmentActivity> Activity { get; }

        /// <summary>
        /// 构造工具栏返回点击事件监听器。
        /// </summary>
        public ToolbarBackListener(FragmentActivity activity)
        {
            Activity = new WeakReference<FragmentActivity>(activity);
        }

        /// <summary>
        /// Java内部的事件接口。
        /// </summary>
        public void OnClick(View v)
        {
            if (!Activity.TryGetTarget(out var activity)) return;

            if (activity.SupportFragmentManager.BackStackEntryCount > 0)
            {
                activity.SupportFragmentManager.PopBackStack();
            }
            else
            {
                activity.Finish();
            }
        }
    }
}