using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HandSchool.Internal
{
    // thanks to zhleiyang for CookieAware
    public class AwaredWebClient : WebClient
    {
        public CookieContainer Cookie { get; } = new CookieContainer();
        WebHeaderCollection protocolErrorResponses;
        public new WebHeaderCollection ResponseHeaders => base.ResponseHeaders is null ? protocolErrorResponses : base.ResponseHeaders;
        public bool AllowAutoRedirect { get; set; }
        public int Timeout { get; set; } = 15000;

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

        public AwaredWebClient(string baseUrl, Encoding encoding)
        {
            BaseAddress = baseUrl;
            Encoding = encoding;
            AllowAutoRedirect = false;

            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        }

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

        public async Task<string> GetAsync(string address, string accept = "application/json")
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

        /// <summary>发送application/x-www-form-urlencoded数据</summary>
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

        /// <summary>发送application/json数据</summary>
        public async Task<string> PostAsync(string script, string value, string type = "application/json", string accept = "application/json")
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
    }

    public class ContentAcceptException : Exception
    {
        public string Result { get; }
        public string Current { get; }
        public string Accept { get; }
        public ContentAcceptException(string ret, string cur, string acc)
        {
            Result = ret;
            Current = cur;
            Accept = acc;
        }
    }
}
