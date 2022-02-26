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
                set => SetMode(value);
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

            public string StringBaseAddress
            {
                get => _baseAddress?.OriginalString ?? "";
                set => _baseAddress = value.IsBlank() ? null : new Uri(value);
            }

            public Uri BaseAddress
            {
                get => _baseAddress;
                set => _baseAddress = value;
            }

            private Uri _baseAddress;

            public int Timeout
            {
                get => _innerClient.Timeout;
                set => _innerClient.Timeout = value;
            }

            private void SolveRequestMeta(WebRequestMeta meta)
            {
                var uri = WebVpnUtil.Combine(StringBaseAddress, meta.Url);
                meta.Url = UsingVpn ? Instance._util.ConvertUri(uri) : uri.OriginalString;
            }

            public async Task<IWebResponse> PostAsync(WebRequestMeta req, KeyValueDict value)
            {
                SolveRequestMeta(req);
                var resp = (HttpClientImpl.WebResponse) await _innerClient.PostAsync(req, value);
                SolveResponse(resp);
                return resp;
            }

            public async Task<IWebResponse> PostAsync(WebRequestMeta req, string value, string contentType)
            {
                SolveRequestMeta(req);
                var resp = (HttpClientImpl.WebResponse) await _innerClient.PostAsync(req, value, contentType);
                SolveResponse(resp);
                return resp;
            }

            public async Task<IWebResponse> GetAsync(WebRequestMeta req)
            {
                SolveRequestMeta(req);
                var resp = (HttpClientImpl.WebResponse) await _innerClient.GetAsync(req);
                SolveResponse(resp);
                return resp;
            }

            private void SolveResponse(HttpClientImpl.WebResponse response)
            {
                var location = response.Location;
                if (location.IsBlank()) return;
                if (StringBaseAddress.IsBlank()) return;
                var baseUrl = StringBaseAddress.Trim();
                if (UsingVpn) baseUrl = Instance._util.ConvertUri(BaseAddress);
                var root = UsingVpn ? "https://webvpn.jlu.edu.cn" : BaseAddress.GetRootUri();
                var res = (root + location).Replace(baseUrl, "");
                res = res.StartsWith("/") ? res : "/" + res;
                res = res.EndsWith("/") ? res.Substring(0, res.Length - 1) : res;
                response.Location = res;
            }

            private void SetMode(VpnHttpClientMode value)
            {
                if (_mode == value) return;
                var usedVpn = UsingVpn;
                _mode = value;
                if (usedVpn == UsingVpn) return;
                if (StringBaseAddress.IsBlank()) return;
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
    }
}