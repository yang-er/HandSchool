using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HandSchool.Internals;

namespace HandSchool.JLU.Services
{
    public partial class WebVpn
    {
        private readonly WebVpnUtil _util;

        public string GetProxyUrl(string url)
        {
            return _util.ConvertUri(WebVpnUtil.Combine("", url));
        }

        public enum VpnHttpClientMode
        {
            VpnAuto,
            VpnOff
        }

        public class VpnHttpClient : IWebClient
        {
            private IWebClient _innerClient;

            public void Dispose() => _innerClient.Dispose();
            public CookieContainer Cookie => _innerClient.Cookie;

            public VpnHttpClient()
            {
                _innerClient = new HttpClientImpl();
                if (UsingVpn) Instance.AddClient(_innerClient);
            }

            public VpnHttpClientMode Mode
            {
                get => _mode;
                set
                {
                    if (_mode == value) return;
                    var usedVpn = UsingVpn;
                    _mode = value;
                    if (usedVpn == UsingVpn) return;
                    if (string.IsNullOrWhiteSpace(BaseAddress)) return;
                    if (usedVpn) Instance.RemoveClient(_innerClient);
                    _innerClient.Dispose();
                    _innerClient = new HttpClientImpl
                    {
                        AllowAutoRedirect = _innerClient.AllowAutoRedirect,
                        Timeout = _innerClient.Timeout,
                        Encoding = _innerClient.Encoding
                    };
                    if (UsingVpn) Instance.AddClient(_innerClient);
                }
            }

            private VpnHttpClientMode _mode;

            public bool UsingVpn =>
                UseVpn && Mode != VpnHttpClientMode.VpnOff;

            public bool AllowAutoRedirect
            {
                get => _innerClient.AllowAutoRedirect;
                set => _innerClient.AllowAutoRedirect = value;
            }

            public Encoding Encoding
            {
                get => _innerClient.Encoding;
                set => _innerClient.Encoding = value;
            }

            public string BaseAddress
            {
                get => _baseAddress;
                set
                {
                    if (!WebVpnUtil.IsAbsolute(value))
                        throw new ArgumentException("BaseAddress must be absolute! ");
                    _baseAddress = value;
                }
            }

            private string _baseAddress;

            public int Timeout
            {
                get => _innerClient.Timeout;
                set => _innerClient.Timeout = value;
            }

            private void SolveRequestMeta(WebRequestMeta meta)
            {
                var uri = WebVpnUtil.Combine(BaseAddress, meta.Url);
                meta.Url = UsingVpn ? Instance._util.ConvertUri(uri) : uri.OriginalString;
            }

            public Task<IWebResponse> PostAsync(WebRequestMeta req, KeyValueDict value)
            {
                SolveRequestMeta(req);
                return _innerClient.PostAsync(req, value);
            }

            public Task<IWebResponse> PostAsync(WebRequestMeta req, string value, string contentType)
            {
                SolveRequestMeta(req);
                return _innerClient.PostAsync(req, value, contentType);
            }

            public Task<IWebResponse> GetAsync(WebRequestMeta req)
            {
                SolveRequestMeta(req);
                return _innerClient.GetAsync(req);
            }
        }
    }
}