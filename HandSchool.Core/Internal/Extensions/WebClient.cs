using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HandSchool.Internals
{
    public static class WebClientExtensions
    {
        public static Task<IWebResponse> GetAsync(this IWebClient webClient, string url, string accept = WebRequestMeta.All)
        {
            return webClient.GetAsync(new WebRequestMeta(url, accept));
        }

        public static async Task<string> GetStringAsync(this IWebClient webClient, string url, string accept = WebRequestMeta.All)
        {
            using (var resp = await webClient.GetAsync(url, accept))
            {
                return await resp.ReadAsStringAsync();
            }
        }

        public static void Add(this WebRequestMeta webRequestMeta, KeyValuePair<string, string> item)
        {
            webRequestMeta.SetHeader(item.Key, item.Value);
        }

        public static string ToDescription(this WebStatus status)
        {
            switch (status)
            {
                case WebStatus.Success:
                    return "请求成功完成";

                case WebStatus.NameResolutionFailure:
                    return "域名解析失败，未连接到互联网";

                case WebStatus.ConnectFailure:
                    return "连接服务器失败，未连接到校内网络";

                case WebStatus.ReceiveFailure:
                    return "数据包传输出现错误";

                case WebStatus.ProtocolError:
                    return "服务器返回信息有问题";

                case WebStatus.SecureChannelFailure:
                    return "SSL证书错误";

                case WebStatus.ServerProtocolViolation:
                    return "网络沟通出现错误";

                case WebStatus.Timeout:
                    return "连接超时，可能是服务器的网络不太好";

                case WebStatus.MimeNotMatch:
                    return "返回类型不匹配";

                default:
                    return "未知错误";
            }
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

        public static void AddCookie(this IWebClient webClient, Cookie cookie)
        {
            webClient.Cookie.Add(new Uri(webClient.BaseAddress), cookie);
        }
    }
}