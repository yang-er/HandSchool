using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using SslPolicyErrors = System.Net.Security.SslPolicyErrors;
using X509Cert = System.Security.Cryptography.X509Certificates.X509Certificate;
using X509Chain = System.Security.Cryptography.X509Certificates.X509Chain;

namespace HandSchool.Internals
{
    /// <summary>
    /// 关注Cookie、自定义验证HTTPS的WebClient
    /// </summary>
    /// <remarks>thanks to zhleiyang for CookieAware</remarks>
    /// <see cref="https://blog.csdn.net/zhleiyang/article/details/7087045" />
    public class AwaredWebClientImpl : WebClient, IWebClient
    {
        /// <summary>
        /// Cookie的容器
        /// </summary>
        public CookieContainer Cookie { get; private set; } = new CookieContainer();

        /// <summary>
        /// Web响应的HTTP头集合
        /// </summary>
        public new WebHeaderCollection ResponseHeaders => base.ResponseHeaders ?? protocolErrorResponses;

        /// <summary>
        /// 重置客户端，取消所有网络任务，清除所有Cookie。
        /// </summary>
        public void ResetClient()
        {
            CancelAsync();
            Cookie = new CookieContainer();
        }

        /// <summary>
        /// 是否允许自动跳转
        /// </summary>
        public bool AllowAutoRedirect { get; set; } = false;

        /// <summary>
        /// 进行Web请求的超时时间长度，以ms为单位
        /// </summary>
        public int Timeout { get; set; } = 15000;

        /// <summary>
        /// 返回HTTP 302时定向到的网址
        /// </summary>
        public string Location
        {
            get
            {
                if (ResponseHeaders is null) return "";
                else if (ResponseHeaders.Get("Location") is null) return "";

                var ret = ResponseHeaders["Location"];
                if (ret.StartsWith(BaseAddress))
                    ret = ret.Substring(BaseAddress.Length);
                return ret;
            }
        }

        /// <summary>
        /// 创建关注Cookie、自定义验证HTTPS的WebClient。
        /// </summary>
        public AwaredWebClientImpl()
        {
            Encoding = Encoding.UTF8;
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidate;
        }

        /// <summary>
        /// 检验SSL服务器证书是否有效。
        /// </summary>
        /// <param name="sender">进行TLS握手的请求。</param>
        /// <param name="certificate">X509证书。</param>
        /// <param name="chain">X509证书链。</param>
        /// <param name="poly">错误策略。</param>
        /// <returns>是否进行握手。</returns>
        public static bool CertificateValidate(object sender, X509Cert certificate, X509Chain chain, SslPolicyErrors poly)
        {
            return true;
        }

        private async Task<IWebResponse> WrapTry(WebRequestMeta req, Func<Task<byte[]>> func)
        {
            try
            {
                var ret = await func();

                if (ResponseHeaders.Get("Location") != null)
                    return new WebResponse(ret, req, WebStatus.Success, this, HttpStatusCode.Redirect);

                if (req.Accept != "*/*" && ResponseHeaders["Content-Type"] is null && !ResponseHeaders["Content-Type"].StartsWith(req.Accept))
                    throw new WebsException(new WebResponse(ret, req, WebStatus.MimeNotMatch, this, HttpStatusCode.OK));

                return new WebResponse(ret, req, WebStatus.Success, this, HttpStatusCode.OK);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse resp)
                {
                    protocolErrorResponses = new WebHeaderCollection { resp.Headers };
                    protocolErrorResponses["Status"] = ((int)resp.StatusCode).ToString() + " " + resp.StatusCode.ToString();

                    if ((int)resp.StatusCode >= 400)
                        throw new WebsException(new WebResponse(new byte[0], req, WebStatus.ProtocolError, this, resp.StatusCode));
                    return new WebResponse(new byte[0], req, WebStatus.Success, this, resp.StatusCode);
                }
                else
                {
                    throw new WebsException(new WebResponse(req, ex.Status.Convert()), ex);
                }
            }
            catch (NotSupportedException ex)
            {
                Core.Logger.WriteException(ex);
                throw new WebsException(WebStatus.UnknownError);
            }
        }

        public Task<IWebResponse> PostAsync(WebRequestMeta req, KeyValueDict value)
        {
            return WrapTry(req, () =>
            {
                return UploadValuesTaskAsync(req.Url, "POST", value);
            });
        }

        public Task<IWebResponse> PostAsync(WebRequestMeta req, string value, string contentType)
        {
            return WrapTry(req, () =>
            {
                Headers.Set("Content-Type", contentType);
                var innerValue = Encoding.GetBytes(value);
                return UploadDataTaskAsync(req.Url, "POST", innerValue);
            });
        }

        public Task<IWebResponse> GetAsync(WebRequestMeta req)
        {
            return WrapTry(req, () =>
            {
                return DownloadDataTaskAsync(req.Url);
            });
        }
        
        private class WebResponse : IWebResponse
        {
            public WebResponse(byte[] result, WebRequestMeta meta, WebStatus stat, AwaredWebClientImpl client, HttpStatusCode code)
            {
                respContent = result;
                Request = meta;
                StatusCode = code;
                Location = client.Location;
                string contentType = client.ResponseHeaders["Content-Type"];
                int lastIndex = contentType.LastIndexOf("; ");
                if (lastIndex == -1) lastIndex = contentType.Length;
                ContentType = contentType.Substring(0, lastIndex);
                encoding = client.Encoding;
                Status = stat;
            }

            public WebResponse(WebRequestMeta meta, WebStatus stat)
            {
                Request = meta;
                StatusCode = HttpStatusCode.InternalServerError;
                Location = "";
                ContentType = "*/*";
                Status = stat;
            }

            private readonly byte[] respContent;
            private readonly Encoding encoding;

            public WebRequestMeta Request { get; }

            public HttpStatusCode StatusCode { get; }

            public string Location { get; }

            public string ContentType { get; }

            public WebStatus Status { get; }

            public void Dispose() { }

            public Task<byte[]> ReadAsByteArrayAsync()
            {
                return Task.FromResult(respContent);
            }

            public Task<string> ReadAsStringAsync()
            {
                return Task.FromResult(encoding.GetString(respContent));
            }

            public Task WriteToFileAsync(string path)
            {
                return Task.Run(() => System.IO.File.WriteAllBytes(path, respContent));
            }
        }
        
        private WebHeaderCollection protocolErrorResponses;

        protected override WebRequest GetWebRequest(Uri address)
        {
            protocolErrorResponses = null;
            var request = base.GetWebRequest(address);

            if (request is HttpWebRequest req)
            {
                req.CookieContainer = Cookie;
                req.AllowAutoRedirect = AllowAutoRedirect;
                req.Timeout = Timeout;
            }

            return request;
        }
    }
}
