using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HandSchool.Internal;
using HandSchool.ViewModels;
using Xamarin.Forms;

[assembly: RegisterService(typeof(WebVpn))]

namespace HandSchool.JLU.Services
{
    [UseStorage("JLU", ConfigUsername, ConfigPassword, ConfigRemember, ConfigTicket)]
    public sealed class WebVpn : NotifyPropertyChanged, IWebLoginField
    {
        const string ConfigUsername = "jlu.vpn.username.txt";
        const string ConfigPassword = "jlu.vpn.password.txt";
        const string ConfigRemember = "jlu.vpn.remember_token.txt";
        const string ConfigTicket = "jlu.vpn.ticket.txt";
        public static WebVpn Instance => Lazy.Value;
        private static readonly Lazy<WebVpn> Lazy = new Lazy<WebVpn>(() => new WebVpn());

        #region Login Fields

        public string LoginUrl => "https://webvpn.jlu.edu.cn/logout";
        bool _isLogin;
        public TimeoutManager TimeoutManager { get; set; }
        public string Username { get; set; }
        public bool IsWeb => true;
        public string Password { get; set; }
        public string CaptchaCode { get; set; }
        public byte[] CaptchaSource { get; set; }

        public string Tips => "账号为吉林大学学生邮箱的用户名(不包含@mails.jlu.edu.cn）和密码。";
        public string FormName => "VPN";
        public bool NeedLogin => !IsLogin;
        public Task<TaskResp> BeforeLoginForm() => Task.FromResult(TaskResp.True);

        #region 处理登录所需的Cookie
        /// <summary>
        /// Cookie的缓存，获取的时候如果没有被更新过就直接返回
        /// </summary>
        private Cookie[] _cookieCache;
        
        /// <summary>
        /// 表示RememberToken和Ticket被更新了多少次
        /// </summary>
        private long _cookieGen;
        
        /// <summary>
        /// 表示_cookieCache的“代”数，与_cookieGen对比可以知道是否需要更新缓存
        /// </summary>
        private long _cookieCacheGen;
        
        private string _rememberToken;

        public string RememberToken
        {
            get => _rememberToken;
            private set
            {
                var trimmed = value?.Trim();
                if (_rememberToken == trimmed) return;
                _rememberToken = trimmed;
                _cookieGen++;
            }
        }
        
        private string _ticket;

        public string Ticket
        {
            get => _ticket;
            private set
            {
                var trimmed = value?.Trim();
                if (_ticket == trimmed) return;
                _ticket = trimmed;
                _cookieGen++;
            }
        }

        public Cookie[] GetLoginCookies()
        {
            if (_cookieCacheGen != _cookieGen)
            {
                _cookieCacheGen = _cookieGen;
                _cookieCache = new[]
                {
                    new Cookie("remember_token", RememberToken, "/", "webvpn.jlu.edu.cn"),
                    new Cookie("wengine_vpn_ticketwebvpn_jlu_edu_cn", Ticket, "/", "webvpn.jlu.edu.cn")
                };
            }

            return _cookieCache;
        }
        
        public void AddCookie(IWebClient webClient)
        {
            foreach (var cookie in GetLoginCookies())
            {
                webClient.Cookie.Add(cookie);
            }
        }

        #endregion

        public bool IsLogin
        {
            get => _isLogin;
            private set
            {
                if (_isLogin != value)
                {
                    if (value)
                    {
                        _proxyClients.ForEach(AddCookie);
                    }
                }

                SetProperty(ref _isLogin, value);
            }
        }

        private void AddClient(IWebClient client)
        {
            if (IsLogin)
            {
                AddCookie(client);
            }

            _proxyClients.Add(client);
        }

        private void RemoveClient(IWebClient client)
        {
            _proxyClients.Remove(client);
        }

        public bool AutoLogin
        {
            get => true;
            set { }
        }

        public bool SavePassword
        {
            get => true;
            set { }
        }

        public event EventHandler<LoginStateEventArgs> LoginStateChanged;

        private async Task<bool> CheckIsLogin()
        {
            try
            {
                AddCookie(WebClient);
                var response = await WebClient.GetAsync("https://webvpn.jlu.edu.cn");
                var res = await response.ReadAsStringAsync();
                return IsLogin = res.Contains("注销");
            }
            catch (WebsException ex)
            {
                Core.Logger.WriteException(ex);
                return IsLogin = false;
            }
        }

        public async Task<bool> CheckLogin()
        {
            if (TimeoutManager.NotInit || TimeoutManager.IsTimeout())
            {
                AddCookie(WebClient);
                try
                {
                    var response = await WebClient.GetAsync("https://webvpn.jlu.edu.cn");
                    var res = await response.ReadAsStringAsync();
                    if (res.Contains("注销"))
                    {
                        TimeoutManager.Refresh();
                        return IsLogin = true;
                    }
                }
                catch (WebsException e)
                {
                    Core.Logger.WriteException(e);
                }
            }
            else
            {
                if (!TimeoutManager.IsTimeout())
                {
                    return IsLogin = true;
                }
            }

            IsLogin = false;
            return IsLogin = await LoginViewModel.RequestAsync(this) == RequestLoginState.Success;
        }

        public async Task Logout()
        {
            try
            {
                await WebClient.GetAsync("https://webvpn.jlu.edu.cn/logout");
                IsLogin = false;
            }
            catch (WebsException ex)
            {
                Core.Logger.WriteException(ex);
            }
        }

        public Task<TaskResp> PrepareLogin() => Task.FromResult(TaskResp.True);
        public Task<TaskResp> Login() => Events?.Result?.Task ?? Task.FromResult(TaskResp.False);

        public string GetProxyUrl(string ori)
        {
            if (_proxyUrl.ContainsKey(ori))
            {
                return _proxyUrl[ori];
            }

            return ori;
        }

        private WebVpn()
        {
            IsLogin = false;
            Username = Core.Configure.Read(ConfigUsername);
            if (Username != "") Password = Core.Configure.Read(ConfigPassword);
            RememberToken = Core.Configure.Read(ConfigRemember);
            Ticket = Core.Configure.Read(ConfigTicket);
            Events = new WebLoginPageEvents
            {
                WebViewEvents = new HSWebViewEvents()
            };
            Events.WebViewEvents.Navigated += OnNavigated;
            Events.WebViewEvents.Navigating += OnNavigating;
            TimeoutManager = new TimeoutManager(30);
            WebClient = new HttpClientImpl();

            Task.Run(async () =>
            {
                AddCookie(WebClient);
                IsLogin = await CheckIsLogin();
            });
        }

        #endregion

        public IWebClient WebClient { get; private set; }
        public WebLoginPageEvents Events { get; }

        private async void OnNavigating(object s, WebNavigatingEventArgs e)
        {
            if (e.Url == "https://webvpn.jlu.edu.cn/")
            {
                var uid = await Events.WebViewEvents.EvaluateJavaScriptAsync(
                    "document.getElementById('user_name').value");
                var pwd = await Events.WebViewEvents.EvaluateJavaScriptAsync(
                    "document.getElementsByName('password')[0].value");
                if (!string.IsNullOrWhiteSpace(uid) && !string.IsNullOrWhiteSpace(pwd))
                {
                    Username = uid;
                    Password = pwd;
                    Core.Configure.Write(ConfigUsername, Username);
                    Core.Configure.Write(ConfigPassword, Password);
                }
            }
        }

        private async void OnNavigated(object s, WebNavigatedEventArgs e)
        {
            if (e.Url == "https://webvpn.jlu.edu.cn/login")
            {
                await Events.WebViewEvents.EvaluateJavaScriptAsync(
                    "let a = document.getElementsByName('remember_cookie')[0]; " +
                    "a.checked=true; " +
                    "document.getElementsByClassName('remember-field')[0].hidden=true;");
                if (!string.IsNullOrWhiteSpace(Username))
                {
                    await Events.WebViewEvents.EvaluateJavaScriptAsync(
                        $"document.getElementById('user_name').value='{Username}'");
                    await Events.WebViewEvents.EvaluateJavaScriptAsync(
                        $"document.getElementsByName('password')[0].value='{Password}'");
                }
            }

            if (e.Url == "https://webvpn.jlu.edu.cn/")
            {
                var updated = false;
                var cookie = Events.WebViewEvents.WebView.HSCookies.GetCookies(new Uri("https://webvpn.jlu.edu.cn/"));
                for (var i = 0; i < cookie.Count; i++)
                {
                    if (cookie[i].Name == "remember_token")
                    {
                        if (RememberToken != cookie[i].Value)
                        {
                            RememberToken = cookie[i].Value;
                            updated = true;
                        }
                    }

                    if (cookie[i].Name == "wengine_vpn_ticketwebvpn_jlu_edu_cn")
                    {
                        if (Ticket != cookie[i].Value)
                        {
                            Ticket = cookie[i].Value;
                            updated = true;
                        }
                    }
                }

                if (updated)
                {
                    Core.Configure.Write(ConfigRemember, RememberToken);
                    Core.Configure.Write(ConfigTicket, Ticket);
                }

                await CheckIsLogin();
                if (IsLogin)
                {
                    await (Events?.Page?.CloseAsync() ?? Task.CompletedTask);
                    Events?.Result?.TrySetResult(TaskResp.True);
                }
            }
        }

        private readonly Dictionary<string, string> _proxyUrl = new Dictionary<string, string>();
        private readonly List<IWebClient> _proxyClients = new List<IWebClient>();

        private static void CheckUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new UriFormatException("url cannot be null or blank");
            }

            if (!url.StartsWith("https://") && !url.StartsWith("http://"))
            {
                throw new UriFormatException("url must start with \"https://\" or \"http://\"");
            }
        }

        public void RegisterUrl(string oriUrl, string proxyUrl)
        {
            CheckUrl(oriUrl);
            CheckUrl(proxyUrl);
            _proxyUrl[oriUrl] = proxyUrl;
        }

        public enum VpnHttpClientMode
        {
            VpnAuto,
            VpnOff,
            VpnOn
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
                Loader.UseVpn && Mode != VpnHttpClientMode.VpnOff;

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