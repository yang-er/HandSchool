using HandSchool.Internals;
using HandSchool.ViewModels;

namespace HandSchool.Views
{
    /// <summary>
    /// 核心视图页面，提供了基本的绑定内容。
    /// </summary>
    public interface IViewCore : IBusySignal
    {
        /// <summary>
        /// 页面的标题
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 与此页面沟通的视图模型
        /// </summary>
        BaseViewModel ViewModel { get; set; }
        
        /// <summary>
        /// 工具菜单内容
        /// </summary>
        ToolbarMenuTracker ToolbarTracker { get; }
    }

    public interface IViewCore<TViewModel> : IViewCore
        where TViewModel : BaseViewModel
    {
        new TViewModel ViewModel { get; set; }
    }
}