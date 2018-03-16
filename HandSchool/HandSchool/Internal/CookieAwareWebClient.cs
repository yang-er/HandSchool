using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Internal
{
    // thanks to zhleiyang for CookieAware
    public class AwaredWebClient : System.Net.WebClient
    {
        public CookieContainer Cookie { get; } = new CookieContainer();
        WebHeaderCollection protocolErrorResponses;
        public new WebHeaderCollection ResponseHeaders => base.ResponseHeaders is null ? protocolErrorResponses : base.ResponseHeaders;
        public string LastUri { get; private set; } = string.Empty;

        protected override WebRequest GetWebRequest(Uri address)
        {
            protocolErrorResponses = null;
            var request = base.GetWebRequest(address);
            if (request is HttpWebRequest req)
            {
                req.CookieContainer = Cookie;
                req.AllowAutoRedirect = false;
                req.Timeout = 15000;
            }
            return request;
        }

        public async Task<string> GetAsync(string address)
        {
            try
            {
                return await DownloadStringTaskAsync(address);
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
        }

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
        }

        public async Task<string> PostAsync(string script, string value, string type = "application/json", string accept = "application/json")
        {
            try
            {
                Headers.Set("Content-Type", type);
                var ret = await UploadStringTaskAsync(script, "POST", value);
                if (!ResponseHeaders["Content-Type"].StartsWith("application/json"))
                {
                    throw new NotImplementedException("Content unaccepted");
                }
                return ret;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse resp)
                {
                    protocolErrorResponses = new WebHeaderCollection { resp.Headers };
                    protocolErrorResponses["Status"] = ((int)resp.StatusCode).ToString();
                    throw new NotImplementedException("HTTP Code not dealt");
                }
                else
                {
                    throw ex;
                }
            }
        }
    }
}
