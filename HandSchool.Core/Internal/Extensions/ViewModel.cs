using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Views;
using System.Threading.Tasks;

namespace HandSchool.Internals
{
    /// <summary>
    /// 视图模型的拓展方法。
    /// </summary>
    public static class ViewModelExtensions
    {
        /// <summary>
        /// 对于表单请求登录。
        /// </summary>
        /// <param name="form">请求的表单。</param>
        /// <returns>登录是否成功。</returns>
        public static async Task<bool> RequestLogin(this ILoginField form)
        {
            if (form.AutoLogin && !form.IsLogin) await form.Login();
            if (!form.IsLogin) await LoginViewModel.RequestAsync(form);
            return form.IsLogin;
        }

        /// <summary>
        /// 通过弹出对话框，提示连接超时的信息。
        /// </summary>
        /// <param name="baseVm">应该显示的视图模型。</param>
        public static Task ShowTimeoutMessage(this BaseViewModel baseVm)
        {
            return baseVm.RequestMessageAsync("错误", "连接超时，请重试。", "知道了");
        }

        /// <summary>
        /// 将新的导航页推入。
        /// </summary>
        /// <param name="navigate">导航工具</param>
        public static Task PushAsync<T>(this INavigate navigate, object param = null)
        {
            return navigate.PushAsync(typeof(T), param);
        }

        /// <summary>
        /// 将新的导航页推入。
        /// </summary>
        /// <param name="navigate">导航工具</param>
        /// <param name="pageType">页面类型</param>
        /// <param name="param">参数</param>
        public static Task PushAsync(this INavigate navigate, string pageType, object param)
        {
            var type = Core.Reflection.TryGetType(pageType);

            if (type is null)
            {
                Core.Logger.WriteLine("NavImpl", pageType + " not found.");
                return Task.CompletedTask;
            }

            return navigate.PushAsync(type, param);
        }
    }
}