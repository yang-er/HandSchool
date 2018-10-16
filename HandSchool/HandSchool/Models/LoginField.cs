using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

namespace HandSchool.Models
{
    /// <summary>
    /// 基础的登录表单，提供了准备接口和登录接口。
    /// </summary>
    public interface ILoginField : INotifyPropertyChanged
    {
        /// <summary>
        /// 登录表单的用户名
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// 登录表单的密码
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// 登录表单的提示（例如默认密码）
        /// </summary>
        string Tips { get; }

        /// <summary>
        /// 是否已登录
        /// </summary>
        bool IsLogin { get; }

        /// <summary>
        /// 是否需要登录
        /// </summary>
        bool NeedLogin { get; }

        /// <summary>
        /// 表单的名称
        /// </summary>
        string FormName { get; }

        /// <summary>
        /// 登录操作，通过网络进行登录操作。
        /// </summary>
        /// <returns>是否登录成功。</returns>
        Task<bool> Login();

        /// <summary>
        /// 登录之前进行准备工作，例如加载验证码等内容。
        /// </summary>
        /// <returns>是否准备成功。</returns>
        Task<bool> PrepareLogin();

        /// <summary>
        /// 验证码内容
        /// </summary>
        string CaptchaCode { get; set; }

        /// <summary>
        /// 验证码源 image/jpeg
        /// </summary>
        byte[] CaptchaSource { get; }

        /// <summary>
        /// 是否自动登录
        /// </summary>
        bool AutoLogin { get; set; }

        /// <summary>
        /// 是否保存密码
        /// </summary>
        bool SavePassword { get; set; }
        
        /// <summary>
        /// 登录状态改变的事件
        /// </summary>
        event EventHandler<LoginStateEventArgs> LoginStateChanged;
    }

    /// <summary>
    /// 登录状态改变的事件参数。
    /// </summary>
    public class LoginStateEventArgs : EventArgs
    {
        /// <summary>
        /// 登录状态
        /// </summary>
        public LoginState State { get; set; }

        /// <summary>
        /// 内部错误
        /// </summary>
        public string InnerError { get; set; }

        /// <summary>
        /// 创建登录状态改变事件的参数。
        /// </summary>
        /// <param name="state">目前的登录状态。</param>
        /// <param name="error">可能的错误信息。</param>
        public LoginStateEventArgs(LoginState state, string error = "")
        {
            State = state;
            InnerError = error;
        }

        /// <summary>
        /// 创建在异常中登录失败的事件参数。
        /// </summary>
        /// <param name="ex">发生的网络异常，用于提供错误信息。</param>
        public LoginStateEventArgs(WebException ex)
        {
            State = LoginState.Failed;
            InnerError = GetWebExceptionMessage(ex);
        }

        /// <summary>
        /// 获得网络异常对应的字符串消息。
        /// </summary>
        /// <param name="e">网络异常信息。</param>
        /// <returns>表述异常的字符串。</returns>
        public static string GetWebExceptionMessage(WebException e)
        {
            switch (e.Status)
            {
                case WebExceptionStatus.NameResolutionFailure:
                    return "域名解析失败，未连接到互联网";
                case WebExceptionStatus.ConnectFailure:
                    return "连接服务器失败，未连接到校内网络";
                case WebExceptionStatus.ReceiveFailure:
                case WebExceptionStatus.SendFailure:
                case WebExceptionStatus.PipelineFailure:
                case WebExceptionStatus.RequestCanceled:
                case WebExceptionStatus.ConnectionClosed:
                    return "数据包传输出现错误";
                case WebExceptionStatus.TrustFailure:
                case WebExceptionStatus.SecureChannelFailure:
                    return "SSL证书错误";
                case WebExceptionStatus.ServerProtocolViolation:
                case WebExceptionStatus.KeepAliveFailure:
                    return "网络沟通出现错误";
                case WebExceptionStatus.Pending:
                case WebExceptionStatus.Timeout:
                    return "连接超时，可能是您的网络不太好";
                default:
                    return e.Status.ToString() + "\n" + e.StackTrace;
            }
        }
    }
    
    /// <summary>
    /// 表示目前登录状态的枚举。
    /// </summary>
    public enum LoginState
    {
        /// <summary>
        /// 正在登录
        /// </summary>
        Processing,

        /// <summary>
        /// 登录成功
        /// </summary>
        Succeeded,

        /// <summary>
        /// 登录失败
        /// </summary>
        Failed
    }
}
