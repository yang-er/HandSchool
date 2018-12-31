using HandSchool.Models;
using System.Threading.Tasks;

namespace HandSchool.Views
{
    /// <summary>
    /// 登录页面的基类，提供了跨平台的函数要求。
    /// </summary>
    public interface ILoginPage
    {
        /// <summary>
        /// 展示登录页面。
        /// </summary>
        Task ShowAsync();

        /// <summary>
        /// 要求窗体对登录状态改变发出响应。
        /// </summary>
        /// <param name="sender">发出者</param>
        /// <param name="e">登陆状态改变事件参数</param>
        void Response(object sender, LoginStateEventArgs e);
    }
}
