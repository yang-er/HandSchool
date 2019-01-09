using HandSchool.Views;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HandSchool.Models
{
    /// <summary>
    /// 实现Universal Windows Platform的导航项目。
    /// </summary>
    internal class NavigationMenuItemUWP : NavigationMenuItem
    {
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
        public NavigationMenuItemUWP(string title, string dest, string category, string icon) : base(title, dest, category)
        {
            Icon = new FontIcon
            {
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                Glyph = icon
            };
            
            if (typeof(IViewPresenter).IsAssignableFrom(PageType))
            {
                var pre = System.Activator.CreateInstance(PageType) as IViewPresenter;
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

            Lazy = new Lazy<NavigationViewItem>(Create);
        }

        /// <summary>
        /// 导航项目对象
        /// </summary>
        public NavigationViewItem Value => Lazy.Value;

        /// <summary>
        /// 延迟加载功能
        /// </summary>
        private Lazy<NavigationViewItem> Lazy { get; }
        
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

        public object NavigationParameter { get; set; }
    }
}
