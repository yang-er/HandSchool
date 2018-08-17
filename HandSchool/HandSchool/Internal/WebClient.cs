using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SslPolicyErrors = System.Net.Security.SslPolicyErrors;
using X509Cert = System.Security.Cryptography.X509Certificates.X509Certificate;
using X509Chain = System.Security.Cryptography.X509Certificates.X509Chain;

namespace HandSchool.Internal
{
    /// <summary>
    /// 关注Cookie、自定义验证HTTPS的WebClient
    /// </summary>
    /// <remarks>thanks to zhleiyang for CookieAware</remarks>
    /// <see cref="https://blog.csdn.net/zhleiyang/article/details/7087045" />
    public class AwaredWebClient : WebClient
    {
        /// <summary>
        /// Cookie的容器
        /// </summary>
        public CookieContainer Cookie { get; } = new CookieContainer();

        /// <summary>
        /// Web响应的HTTP头集合
        /// </summary>
        public new WebHeaderCollection ResponseHeaders => base.ResponseHeaders ?? protocolErrorResponses;

        /// <summary>
        /// 是否允许自动跳转
        /// </summary>
        public bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// 进行Web请求的超时时间长度
        /// </summary>
        public int Timeout { get; set; } = 15000;

        /// <summary>
        /// 返回HTTP 302时定向到的网址
        /// </summary>
        public string Location
        {
            get
            {
                try
                {
                    var ret = ResponseHeaders["Location"];
                    if (ret.StartsWith(BaseAddress))
                        return ret.Replace(BaseAddress, string.Empty);
                    else
                        return ret;
                }
                catch (NullReferenceException)
                {
                    return "";
                }
            }
        }

        /// <param name="baseUrl">请求的基地址（WebClient.BaseAddress）</param>
        /// <param name="encoding">请求的编码（WebClient.Encoding）</param>
        /// <param name="redirect">是否自动跳转（AwaredWebClient.AllowAutoRedirect）</param>
        public AwaredWebClient(string baseUrl, Encoding encoding, bool redirect = false)
        {
            BaseAddress = baseUrl;
            Encoding = encoding;
            AllowAutoRedirect = redirect;
            
            // Use our CertValid function
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidate;
        }

        /// <summary>
        /// 检验SSL服务器证书是否有效
        /// </summary>
        /// <param name="sender">进行TLS握手的请求</param>
        /// <param name="certificate">X509证书</param>
        /// <param name="chain">X509证书链</param>
        /// <param name="poly">错误策略</param>
        /// <returns>是否进行握手</returns>
        public static bool CertificateValidate(object sender, X509Cert certificate, X509Chain chain, SslPolicyErrors poly)
        {
            return true;
        }

        /// <summary>
        /// 以GET形式发送数据
        /// </summary>
        /// <param name="address">获取的地址</param>
        /// <param name="accept">期望接收的数据类型</param>
        /// <returns>获取到的数据内容</returns>
        /// <exception cref="WebException" />
        /// <exception cref="ContentAcceptException" />
        public async Task<string> GetAsync(string address, string accept = json)
        {
            try
            {
                var ret = await DownloadStringTaskAsync(address);
                if (accept != "*/*" && ResponseHeaders["Content-Type"] is null && !ResponseHeaders["Content-Type"].StartsWith(accept))
                {
                    throw new ContentAcceptException(ret, ResponseHeaders["Content-Type"], accept);
                }
                return ret;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse resp)
                {
                    protocolErrorResponses = new WebHeaderCollection { resp.Headers };
                    protocolErrorResponses["Status"] = ((int)resp.StatusCode).ToString() + " " + resp.StatusCode.ToString();
                    return "";
                }
                else
                {
                    throw ex;
                }
            }
            catch (NotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return "";
            }
        }

        public async Task<byte[]> GetAsync(string address, string accept, string type)
        {
            try
            {
                var ret = await DownloadDataTaskAsync(address);
                if (accept != "*/*" && ResponseHeaders["Content-Type"] is null && !ResponseHeaders["Content-Type"].StartsWith(accept))
                {
                    throw new ContentAcceptException(ret.ToHexDigest(), ResponseHeaders["Content-Type"], accept);
                }
                return ret;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse resp)
                {
                    protocolErrorResponses = new WebHeaderCollection { resp.Headers };
                    protocolErrorResponses["Status"] = ((int)resp.StatusCode).ToString() + " " + resp.StatusCode.ToString();
                    return null;
                }
                else
                {
                    throw ex;
                }
            }
            catch (NotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// 以POST形式发送数据
        /// </summary>
        /// <param name="script">发送去的地址</param>
        /// <param name="value">发送的键值对，以application/x-www-form-urlencoded形式编码</param>
        /// <returns>获取到的数据内容</returns>
        /// <exception cref="WebException" />
        public async Task<string> PostAsync(string script, NameValueCollection value)
        {
            try
            {
                return Encoding.GetString(await UploadValuesTaskAsync(script, "POST", value));
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse resp)
                {
                    protocolErrorResponses = new WebHeaderCollection { resp.Headers };
                    protocolErrorResponses["Status"] = ((int)resp.StatusCode).ToString();
                    return "";
                }
                else
                {
                    throw ex;
                }
            }
            catch (NotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return "";
            }
        }

        /// <summary>
        /// 以POST形式发送数据
        /// </summary>
        /// <param name="script">发送去的地址</param>
        /// <param name="value">发送的数据内容，默认为application/json</param>
        /// <param name="type">发送的Content-Type</param>
        /// <param name="accept">发送的Accept，期望服务器返回</param>
        /// <returns>获取到的数据内容</returns>
        /// <exception cref="WebException" />
        /// <exception cref="ContentAcceptException" />
        public async Task<string> PostAsync(string script, string value, string type = json, string accept = json)
        {
            try
            {
                Headers.Set("Content-Type", type);
                var ret = await UploadStringTaskAsync(script, "POST", value);
                if (accept != "*/*" && !ResponseHeaders["Content-Type"].StartsWith(accept))
                {
                    throw new ContentAcceptException(ret, ResponseHeaders["Content-Type"], accept);
                }
                return ret;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse resp)
                {
                    protocolErrorResponses = new WebHeaderCollection { resp.Headers };
                    protocolErrorResponses["Status"] = ((int)resp.StatusCode).ToString();
                    return "";
                    // throw new NotImplementedException("HTTP Code not dealt");
                }
                else
                {
                    throw ex;
                }
            }
            catch (NotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return "";
            }
        }

        const string json = "application/json";

        WebHeaderCollection protocolErrorResponses;

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

    /// <summary>
    /// 内容类型不相容错误
    /// </summary>
    public class ContentAcceptException : Exception
    {
        /// <summary>
        /// 返回的内容本身
        /// </summary>
        public string Result { get; }

        /// <summary>
        /// 目前的返回内容类型
        /// </summary>
        public string Current { get; }

        /// <summary>
        /// 本应接收的内容类型
        /// </summary>
        public string Accept { get; }

        /// <summary>
        /// 内容类型不相容错误
        /// </summary>
        /// <param name="ret">返回的内容本身</param>
        /// <param name="cur">目前的返回内容类型</param>
        /// <param name="acc">本应接收的内容类型</param>
        public ContentAcceptException(string ret, string cur, string acc)
        {
            Result = ret;
            Current = cur;
            Accept = acc;
        }
    }
}
