using System;
using System.Collections.Generic;
using System.Text;
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
        public static string GetWebExceptionMessage(System.Net.WebException e)
        {
            switch (e.Status)
            {
                case System.Net.WebExceptionStatus.NameResolutionFailure:
                    return "域名解析失败，未连接到互联网";
                case System.Net.WebExceptionStatus.ConnectFailure:
                    return "连接服务器失败，未连接到校内网络";
                case System.Net.WebExceptionStatus.ReceiveFailure:
                case System.Net.WebExceptionStatus.SendFailure:
                case System.Net.WebExceptionStatus.PipelineFailure:
                case System.Net.WebExceptionStatus.RequestCanceled:
                case System.Net.WebExceptionStatus.ConnectionClosed:
                    return "数据包传输出现错误";
                case System.Net.WebExceptionStatus.TrustFailure:
                case System.Net.WebExceptionStatus.SecureChannelFailure:
                    return "SSL证书错误";
                case System.Net.WebExceptionStatus.ServerProtocolViolation:
                case System.Net.WebExceptionStatus.KeepAliveFailure:
                    return "网络沟通出现错误";
                case System.Net.WebExceptionStatus.Pending:
                case System.Net.WebExceptionStatus.Timeout:
                    return "连接超时，可能是您的网络不太好";
                default:
                    return e.Status.ToString() + "\n" + e.StackTrace;
            }
        }
    }
}