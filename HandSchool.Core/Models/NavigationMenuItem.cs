using HandSchool.Internal;
using System;
using System.Reflection;

namespace HandSchool.Models
{
    /// <summary>
    /// 实现导航菜单的基类。
    /// </summary>
    /// <inheritdoc cref="NotifyPropertyChanged" />
    public class NavigationMenuItem : NotifyPropertyChanged
    {
        private string title;
        protected readonly string destType;

        /// <summary>
        /// 展示的标题
        /// </summary>
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        /// <summary>
        /// 目标页面类型元数据
        /// </summary>
        public Type PageType { get; protected set; }

        /// <summary>
        /// 创建一个系统导航项目，并提供页面延迟加载的功能。
        /// </summary>
        /// <param name="title">导航项目的名称。</param>
        /// <param name="dest">目标页面类型名。</param>
        /// <param name="category">类的父命名空间。</param>
        public NavigationMenuItem(string title, string dest, string category = "")
        {
            this.title = title;
            if (category != "")
            {
                destType = $"HandSchool.{category}.Views.{dest}";
                PageType = Core.App.Loader.GetType().Assembly.GetType(destType);
            }
            else
            {
                destType = $"HandSchool.Views.{dest}";
                PageType = GetType().Assembly.GetType(destType);
            }
        }

        public override string ToString() => title;
    }
}
