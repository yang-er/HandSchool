using HandSchool.Models;
using HandSchool.ViewModels;
using System.Threading.Tasks;

namespace HandSchool.Views
{
    /// <summary>
    /// 登录页面的接口类。
    /// </summary>
    public interface ILoginPage : IViewPage
    {
        /// <summary>
        /// 登录的视图模型
        /// </summary>
        LoginViewModel LoginViewModel { get; }

        /// <summary>
        /// 要求窗体对登录状态改变发出响应。
        /// </summary>
        /// <param name="sender">发出者</param>
        /// <param name="e">登陆状态改变事件参数</param>
        void Response(object sender, LoginStateEventArgs e);

        /// <summary>
        /// 更新验证码信息。
        /// </summary>
        void UpdateCaptchaInfomation();

        /// <summary>
        /// 显示登录对话框。
        /// </summary>
        Task ShowAsync();
    }
}
