using System;
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
        /// 在导航栈内推入页面，但是数据与页面创建分离。
        /// </summary>
        /// <param name="pageType">页面类型</param>
        /// <param name="param">传入参数</param>
        Task PushAsync(string pageType, object param);

        /// <summary>
        /// 在导航栈内推入页面，但是数据与页面创建分离。
        /// </summary>
        /// <param name="pageType">页面类型</param>
        /// <param name="param">传入参数</param>
        Task PushAsync(Type pageType, object param);
    }
}
