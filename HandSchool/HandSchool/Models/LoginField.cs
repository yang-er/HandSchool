using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Models
{
    /// <summary>
    /// 登录表单
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
        /// 登录函数
        /// </summary>
        /// <returns>是否登录成功</returns>
        Task<bool> Login();

        /// <summary>
        /// 登录之前的准备
        /// </summary>
        /// <returns>是否准备成功</returns>
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
    /// 登录状态改变的事件
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
        /// 登录状态改变事件
        /// </summary>
        /// <param name="state">登录状态</param>
        /// <param name="error">错误信息</param>
        public LoginStateEventArgs(LoginState state, string error = "")
        {
            State = state;
            InnerError = error;
        }

        /// <summary>
        /// 登录状态改变事件
        /// </summary>
        /// <param name="ex">网络异常</param>
        public LoginStateEventArgs(WebException ex)
        {
            State = LoginState.Failed;
            InnerError = GetWebExceptionMessage(ex);
        }

        /// <summary>
        /// 获得异常的消息
        /// </summary>
        /// <param name="e">网络异常信息</param>
        /// <returns>表述字符串</returns>
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
    /// 登录状态
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
