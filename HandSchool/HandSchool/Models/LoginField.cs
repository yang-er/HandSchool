using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

namespace HandSchool.Models
{
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

    public class LoginStateEventArgs : EventArgs
    {
        public LoginState State { get; set; }
        public string InnerError { get; set; }

        public LoginStateEventArgs(LoginState state, string error = "")
        {
            State = state;
            InnerError = error;
        }

        public LoginStateEventArgs(WebException ex)
        {
            State = LoginState.Failed;
            InnerError = GetWebExceptionMessage(ex);
        }

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
    
    public enum LoginState
    {
        Processing,
        Succeeded,
        Failed
    }
}
