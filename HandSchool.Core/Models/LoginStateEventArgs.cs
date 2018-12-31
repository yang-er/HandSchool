using System;
using System.Net;

namespace HandSchool.Models
{
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
}
