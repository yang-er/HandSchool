using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Views;
using System.Threading.Tasks;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 用于帮助填写表单登录的视图模型。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    public class LoginViewModel : BaseViewModel
    {
        /// <summary>
        /// 登录命令
        /// </summary>
        public CommandAction LoginCommand { get; set; }

        /// <summary>
        /// 操作表单
        /// </summary>
        public ILoginField Form { get; }

        /// <summary>
        /// 登录页面
        /// </summary>
        public ILoginPage Page { get; set; }
        
        /// <summary>
        /// 创建登录视图模型，并绑定参数。
        /// </summary>
        /// <param name="form"></param>
        private LoginViewModel(ILoginField form)
        {
            Form = form;
            Title = "登录" + form.FormName;
        }
        
        /// <summary>
        /// 异步地请求登录表单内容。
        /// </summary>
        /// <param name="form">需要登录的表单。</param>
        public static Task<bool> RequestAsync(ILoginField form)
        {
            return Core.Platform.EnsureOnMainThread(async () =>
            {
                var viewModel = new LoginViewModel(form);
                viewModel.LoginCommand = new CommandAction(viewModel.Login);
                viewModel.Page = Core.Platform.CreateLoginPage(viewModel);
                await viewModel.Page.ShowAsync();
                return form.IsLogin;
            });
        }
        
        /// <summary>
        /// 执行登录操作，并设置状态。
        /// </summary>
        private async Task Login()
        {
            if (IsBusy)
            {
                Page.Response(this, new LoginStateEventArgs(LoginState.Processing));
                return;
            }

            IsBusy = true;
            Form.LoginStateChanged += Page.Response;

            try
            {
                await Form.Login();
            }
            finally
            {
                IsBusy = false;
                Form.LoginStateChanged -= Page.Response;
            }
        }
    }
}
