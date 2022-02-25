using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HandSchool.Internals;

namespace HandSchool.JLU.Services
{
    public partial class WebVpn
    {
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
                if (UsingVpn)
                {
                    Instance.AddClient(_innerClient);
                }
            }

            public VpnHttpClientMode Mode
            {
                get => _mode;
                set
                {
                    if (_mode != value)
                    {
                        var vpnState = UsingVpn;
                        _mode = value;
                        if (vpnState == UsingVpn) return;
                        if (string.IsNullOrWhiteSpace(BaseAddress)) return;
                        if (vpnState)
                        {
                            Instance.RemoveClient(_innerClient);
                        }

                        var address = _oriBaseUrl;
                        _innerClient.Dispose();
                        _innerClient = new HttpClientImpl();
                        Instance.AddClient(_innerClient);
                        BaseAddress = address;
                    }
                }
            }

            private string _oriBaseUrl;
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
                get => _innerClient.BaseAddress;
                set
                {
                    if (UsingVpn)
                    {
                        if (Instance._proxyUrl.ContainsKey(value))
                        {
                            _innerClient.BaseAddress = Instance._proxyUrl[value];
                        }
                        else
                        {
                            throw new UriFormatException($"You should register \"{value}\" in WebVpn before");
                        }
                    }
                    else
                    {
                        _innerClient.BaseAddress = value;
                    }

                    _oriBaseUrl = value;
                }
            }

            public int Timeout
            {
                get => _innerClient.Timeout;
                set => _innerClient.Timeout = value;
            }

            public Task<IWebResponse> PostAsync(WebRequestMeta req, KeyValueDict value) =>
                _innerClient.PostAsync(req, value);

            public Task<IWebResponse> PostAsync(WebRequestMeta req, string value, string contentType)
                => _innerClient.PostAsync(req, value, contentType);

            public Task<IWebResponse> GetAsync(WebRequestMeta req)
                => _innerClient.GetAsync(req);
        }
    }
}