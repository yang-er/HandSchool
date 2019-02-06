using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.Views;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HandSchool.UWP
{
    /// <summary>
    /// 实现Universal Windows Platform的导航项目。
    /// </summary>
    internal class NavigationMenuItemImpl : NavigationMenuItem
    {
        public static readonly string[] IconList = new[]
        {
            "\xE10F",
            "\xECA5",
            "\xEB50",
            "\xE946",
            "\xE715",
            "\xE9D2",
            "\xE8C7",
            "?",
            "?",
        };

        /// <summary>
        /// UWP上的图标
        /// </summary>
        public IconElement Icon { get; }
        
        /// <summary>
        /// 创建一个系统导航项目，并提供页面延迟加载的功能。
        /// </summary>
        /// <param name="title">导航项目的名称</param>
        /// <param name="dest">目标页面类型名</param>
        /// <param name="category">类的父命名空间</param>
        /// <param name="icon">UWP上的图标</param>
        public NavigationMenuItemImpl(string title, string dest, string category, MenuIcon icon)
            : this(title, dest, category)
        {
            Icon = new FontIcon
            {
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                Glyph = IconList[(int)icon]
            };

            Lazy = new Lazy<NavigationViewItem>(Create);
        }

        /// <summary>
        /// 创建一个系统导航项目，并提供页面延迟加载的功能。
        /// </summary>
        /// <param name="title">导航项目的名称</param>
        /// <param name="dest">目标页面类型名</param>
        /// <param name="category">类的父命名空间</param>
        private NavigationMenuItemImpl(string title, string dest, string category = "")
            : base(title, dest, category)
        {
            if (PageType is null) PageType = typeof(GradePointPage);

            if (typeof(IViewPresenter).IsAssignableFrom(PageType))
            {
                var pre = Core.Reflection.CreateInstance<IViewPresenter>(PageType);
                if (pre.PageCount == 1)
                {
                    NavigationParameter = pre;
                    PageType = typeof(PackagedPage);
                }
                else
                {
                    NavigationParameter = pre;
                    PageType = typeof(TabbedPage);
                }
            }

            else if (typeof(ViewObject).IsAssignableFrom(PageType))
            {
                NavigationParameter = new ValueTuple<Type, object>(PageType, null);
                PageType = typeof(PackagedPage);
            }
        }

        /// <summary>
        /// 导航项目对象
        /// </summary>
        public NavigationViewItem Value => Lazy.Value;

        /// <summary>
        /// 延迟加载功能
        /// </summary>
        private Lazy<NavigationViewItem> Lazy { get; set; }
        
        /// <summary>
        /// 创建系统导航项目。
        /// </summary>
        /// <returns>系统导航</returns>
        private NavigationViewItem Create()
        {
            return new NavigationViewItem
            {
                Icon = Icon,
                Content = this,
            };
        }

        /// <summary>
        /// 页面导航使用的参数
        /// </summary>
        public object NavigationParameter { get; set; }

        /// <summary>
        /// 从设置项创建菜单项。
        /// </summary>
        /// <param name="item">设置项</param>
        /// <returns>菜单项</returns>
        public static NavigationMenuItemImpl CreateSettingItem(NavigationViewItem item)
        {
            return new NavigationMenuItemImpl("设置", "SettingPresenter")
            {
                Lazy = new Lazy<NavigationViewItem>(() => item)
            };
        }
    }
}
