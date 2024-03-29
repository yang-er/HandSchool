﻿using Android.App;
using Android.Views;
using HandSchool.Internals;
using System;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;

namespace HandSchool.Droid
{
    /// <summary>
    /// 视图绑定自动化特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
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
            target.GetType().GetProperties().ForEach(prop =>
            {
                prop.Get<BindViewAttribute>(false)?.Let(attr =>
                {
                    prop.SetValue(target, view.FindViewById(attr.ResourceId));
                });
            });
            target.SolveBindings();
        }

        /// <summary>
        /// 从 <see cref="Dialog"/> 中解析 <see cref="IBindTarget"/> 的绑定视图。
        /// </summary>
        /// <param name="view">视图对象</param>
        /// <param name="target">目标绑定</param>
        public static void SolveView(this IBindTarget target, Dialog view)
        {
            target.GetType().GetProperties().ForEach(prop =>
            {
                prop.Get<BindViewAttribute>(false)?.Let(attr =>
                {
                    prop.SetValue(target, view.FindViewById(attr.ResourceId));
                });
            });
            target.SolveBindings();
        }

        /// <summary>
        /// 从 <see cref="Activity"/> 中解析 <see cref="IBindTarget"/> 的绑定视图。
        /// </summary>
        /// <param name="view">视图对象</param>
        /// <param name="target">目标绑定</param>
        public static void SolveView(this IBindTarget target, Activity view)
        {
            target.GetType().GetProperties().ForEach(prop =>
            {
                prop.Get<BindViewAttribute>(false)?.Let(attr =>
                {
                    prop.SetValue(target, view.FindViewById(attr.ResourceId));
                });
            });
            target.SolveBindings();
        }

        /// <summary>
        /// 从类型自身获取视图绑定源。
        /// </summary>
        /// <param name="target">绑定源</param>
        /// <returns>视图资源ID</returns>
        public static int SolveSelf(this IBindTarget target)
        {
            return target.GetType().Get<BindViewAttribute>(false)?.ResourceId ?? 0;
        }

        /// <summary>
        /// 给文本框赋值
        /// </summary>
        /// <param name="tw">文本框</param>
        /// <param name="text">文本</param>
        public static void SetText(this Android.Widget.TextView tw, string text)
        {
            tw.Text = text;
        }

        /// <summary>
        /// 给文本设置颜色
        /// </summary>
        /// <param name="tw">文本框</param>
        /// <param name="color">颜色</param>
        public static void SetColor(this Android.Widget.TextView tw, Xamarin.Forms.Color color)
        {
            tw.SetTextColor(color.ToAndroid());
        }

        /// <summary>
        /// 设置视图是否可见
        /// </summary>
        /// <param name="view">安卓视图</param>
        /// <param name="visibility">是否可见</param>
        public static void SetVisibility(this View view, bool visibility)
        {
            view.Visibility = visibility ? ViewStates.Visible : ViewStates.Gone;
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
                    prop.SetValue(target, null);
                }
            }
        }
    }
}