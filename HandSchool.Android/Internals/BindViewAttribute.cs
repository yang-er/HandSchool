using Android.App;
using Android.Views;
using HandSchool.Internals;
using System;

namespace HandSchool.Droid
{
    /// <summary>
    /// 视图绑定自动化特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BindViewAttribute : Attribute
    {
        /// <summary>
        /// 资源ID
        /// </summary>
        public int ResourceId { get; }

        /// <summary>
        /// 标注属性的自动绑定。
        /// </summary>
        /// <param name="id">视图的资源ID</param>
        public BindViewAttribute(int id)
        {
            ResourceId = id;
        }
    }

    /// <summary>
    /// 绑定目标
    /// </summary>
    public interface IBindTarget
    {
        /// <summary>
        /// 目标加载后绑定数据。
        /// </summary>
        void SolveBindings();
    }

    /// <summary>
    /// 视图绑定的拓展方法类。
    /// </summary>
    public static class BindViewExtensions
    {
        /// <summary>
        /// 从 <see cref="View"/> 中解析 <see cref="IBindTarget"/> 的绑定视图。
        /// </summary>
        /// <param name="view">视图对象</param>
        /// <param name="target">目标绑定</param>
        public static void SolveView(this IBindTarget target, View view)
        {
            foreach (var prop in target.GetType().GetProperties())
            {
                if (prop.Has<BindViewAttribute>())
                {
                    var attr = prop.Get<BindViewAttribute>();
                    prop.SetValue(target, view.FindViewById(attr.ResourceId));
                }
            }

            target.SolveBindings();
        }

        /// <summary>
        /// 从 <see cref="Activity"/> 中解析 <see cref="IBindTarget"/> 的绑定视图。
        /// </summary>
        /// <param name="view">视图对象</param>
        /// <param name="target">目标绑定</param>
        public static void SolveView(this IBindTarget target, Activity view)
        {
            foreach (var prop in target.GetType().GetProperties())
            {
                if (prop.Has<BindViewAttribute>())
                {
                    var attr = prop.Get<BindViewAttribute>();
                    prop.SetValue(target, view.FindViewById(attr.ResourceId));
                }
            }

            target.SolveBindings();
        }

        /// <summary>
        /// 从绑定目标中清理数据。
        /// </summary>
        /// <param name="target">绑定目标</param>
        public static void CleanBind(this IBindTarget target)
        {
            foreach (var prop in target.GetType().GetProperties())
            {
                if (prop.Has<BindViewAttribute>())
                {
                    var attr = prop.Get<BindViewAttribute>();
                    prop.SetValue(target, null);
                }
            }
        }
    }
}