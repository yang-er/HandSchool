using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.Views
{
    /// <summary>
    /// 视图的导航模型。
    /// </summary>
    public interface INavigate
    {
        /// <summary>
        /// 导航视图栈
        /// </summary>
        IReadOnlyList<IViewPage> NavigationStack { get; }
        
        /// <summary>
        /// 弹出最上层页面。
        /// </summary>
        Task<IViewPage> PopAsync();
        
        /// <summary>
        /// 在导航栈内推入页面。
        /// </summary>
        /// <param name="page">推入栈内的页面</param>
        Task PushAsync(IViewPage page);
    }
}
