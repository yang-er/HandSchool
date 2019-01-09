using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.Views
{
    /// <summary>
    /// 基础的视图页面。
    /// </summary>
    public interface IViewPage : IViewResponse
    {
        /// <summary>
        /// 显示此视图页面。
        /// </summary>
        /// <param name="parent">浏览导航控制器。</param>
        Task ShowAsync(INavigate parent = null);

        /// <summary>
        /// 关闭此视图页面。
        /// </summary>
        Task CloseAsync();

        /// <summary>
        /// 与此页面沟通的视图模型
        /// </summary>
        BaseViewModel ViewModel { get; set; }

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
        /// 页面的标题
        /// </summary>
        string Title { get; set; }

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
        void RegisterNavigation(INavigate navigate);
    }
}
