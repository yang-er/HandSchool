using System;

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
        /// 页面正在消失时
        /// </summary>
        event EventHandler Disappearing;

        /// <summary>
        /// 页面正在出现时
        /// </summary>
        event EventHandler Appearing;
    }
}