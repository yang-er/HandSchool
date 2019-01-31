using System;
using System.ComponentModel;

namespace HandSchool.Views
{
    /// <summary>
    /// 基础的视图页面。
    /// </summary>
    public interface IViewPage : IViewResponse, IViewCore
    {
        /// <summary>
        /// 视图导航控制器
        /// </summary>
        INavigate Navigation { get; }

        /// <summary>
        /// 是否为模态框
        /// </summary>
        bool IsModal { get; }

        /// <summary>
        /// 页面显示的主要内容
        /// </summary>
        Xamarin.Forms.View Content { get; set; }
        
        /// <summary>
        /// 工具栏的入口点
        /// </summary>
        void AddToolbarEntry(MenuEntry item);

        /// <summary>
        /// 页面正在消失时
        /// </summary>
        event EventHandler Disappearing;

        /// <summary>
        /// 页面正在出现时
        /// </summary>
        event EventHandler Appearing;

        /// <summary>
        /// 注册当前的导航实现。仅供内部使用。
        /// </summary>
        /// <param name="navigate">导航工具</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void RegisterNavigation(INavigate navigate);
    }
}
