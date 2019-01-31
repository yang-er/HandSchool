using HandSchool.ViewModels;

namespace HandSchool.Views
{
    /// <summary>
    /// 核心视图页面，提供了基本的绑定内容。
    /// </summary>
    public interface IViewCore
    {
        /// <summary>
        /// 页面的标题
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 与此页面沟通的视图模型
        /// </summary>
        BaseViewModel ViewModel { get; set; }
    }
}