using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HandSchool.Internal;
using HandSchool.ViewModels;
using Newtonsoft.Json;
using Xamarin.Forms;

[assembly: RegisterService(typeof(WebVpn))]

namespace HandSchool.JLU.Services
{
    [UseStorage("JLU")]
    public sealed class WebVpn : NotifyPropertyChanged, IWebLoginField
    {
        /// <summary>
        /// 设置是否使用Vpn
        /// </summary>
        [Settings("使用WebVPN", "使用WebVPN连接各种系统，建议在内网时关闭此选项。切换后需要重启APP。")]
        public static bool UseVpn { get; set; }

        private const string ServerName = "WebVpn";
        const string ConfigCookies = "webvpn.logincookies";
        public static WebVpn Instance => UseVpn ? Lazy.Value : null;
        private static readonly Lazy<WebVpn> Lazy = new Lazy<WebVpn>(() => new WebVpn());
        private bool _pageClosed;

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
        public string FormName => "WebVPN";
        public bool NeedLogin => !IsLogin;
        public Task<TaskResp> BeforeLoginForm() => Task.FromResult(TaskResp.True);

        #region 处理登录所需的Cookie
        private const string TokenName = "remember_token";
        private const string TicketName = "wengine_vpn_ticketwebvpn_jlu_edu_cn";

        /// <summary>
        /// 用来方便序列化Cookie
        /// </summary>
        private class CookieLite
        {
            private readonly Cookie _innerCookie;
            public CookieLite(Cookie c)
            {
                _innerCookie = c;
            }

            public string Domain => _innerCookie?.Domain;
            public string Path => _innerCookie?.Path;
            public string Name => _innerCookie?.Name;
            public string Value => _innerCookie?.Value;
        }

        /// <summary>
        /// 用来标识WebVpn的WebClient中的Cookie是不是最新的
        /// </summary>
        private bool _loginCookiesChanged;
        public Cookie RememberToken
        {
            get => _rememberToken;
            set
            {
                if (_rememberToken?.Value == value?.Value) return;
                _rememberToken = value;
                _loginCookiesChanged = true;
            }
        }
        private Cookie _rememberToken;

        public Cookie Ticket
        {
            get => _ticket;
            set
            {
                if (_ticket?.Value == value?.Value) return;
                _ticket = value;
                _loginCookiesChanged = true;
            }
        }
        private Cookie _ticket;

        public IEnumerable<Cookie> GetLoginCookies()
        {
            if (Ticket != null)
                yield return Ticket;

            if (RememberToken != null)
                yield return RememberToken;
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
                if (_loginCookiesChanged)
                {
                    WebClient?.Dispose();
                    WebClient = new HttpClientImpl();
                    AddCookie(WebClient);
                }
                if (WebClient == null) return IsLogin = false;
                _loginCookiesChanged = false;
                var response = await WebClient.GetAsync("https://webvpn.jlu.edu.cn");
                var res = await response.ReadAsStringAsync();
                response.Dispose();
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
                if (await CheckIsLogin()) return true;
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
        public Task<TaskResp> Login()
        {
            _pageClosed = false;
            return Events?.Result?.Task ?? Task.FromResult(TaskResp.False);
        }

        public string GetProxyUrl(string ori)
        {
            if (_proxyUrl.ContainsKey(ori))
            {
                return _proxyUrl[ori];
            }

            throw new KeyNotFoundException($"url {ori} has not registered");
        }

        private WebVpn()
        {
            IsLogin = false;
            var acc = Core.App.Loader.AccountManager.GetItemWithPrimaryKey(ServerName);
            if (acc != null)
            {
                Username = acc.UserName;
                Password = acc.Password;
            }

            Core.App.Loader.JsonManager
                .GetItemWithPrimaryKey(ConfigCookies)
                ?.ToObject<List<Cookie>>()
                ?.ForEach(c =>
                {
                    switch (c.Name)
                    {
                        case TicketName:
                            Ticket = c;
                            break;
                        case TokenName:
                            RememberToken = c;
                            break;
                    }
                });
            
            Events = new WebLoginPageEvents
            {
                WebViewEvents = new HSWebViewEvents()
            };
            Events.WebViewEvents.Navigated += OnNavigated;
            Events.WebViewEvents.Navigating += OnNavigating;
            TimeoutManager = new TimeoutManager(30);

            Task.Run(async () =>
            {
                await CheckIsLogin();
            });
        }

        #endregion

        public IWebClient WebClient { get; private set; }
        public WebLoginPageEvents Events { get; }

        private async void OnNavigating(object s, WebNavigatingEventArgs e)
        {
            if (!e.Url.Contains("webvpn.jlu.edu.cn"))
            {
                Events.WebViewEvents.WebView.Source = LoginUrl;
            }
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
                    Core.App.Loader.AccountManager.InsertOrUpdateTable(new UserAccount
                    {
                        ServerName = ServerName,
                        UserName = Username,
                        Password = Password
                    });
                }
            }
        }

        private async void OnNavigated(object s, WebNavigatedEventArgs e)
        {
            if (!_pageClosed)
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

                if (e.Url == "https://webvpn.jlu.edu.cn/" || e.Url == "https://webvpn.jlu.edu.cn/m/portal")
                {

                    var updated = false;
                    var cookie = Events.WebViewEvents.WebView.HSCookies.GetCookies(new Uri("https://webvpn.jlu.edu.cn/"));
                    for (var i = 0; i < cookie.Count; i++)
                    {
                        if (cookie[i].Name == TokenName)
                        {
                            if (RememberToken?.Value != cookie[i].Value)
                            {
                                RememberToken = cookie[i];
                                updated = true;
                            }
                        }

                        if (cookie[i].Name == TicketName)
                        {
                            if (Ticket?.Value != cookie[i].Value)
                            {
                                Ticket = cookie[i];
                                updated = true;
                            }
                        }
                    }

                    if (updated)
                    {
                        Core.App.Loader.JsonManager.InsertOrUpdateTable(new ServerJson
                        {
                            JsonName = ConfigCookies,
                            Json = GetLoginCookies()
                                .Select(c => new CookieLite(c))
                                .Serialize()
                        });
                    }

                    await CheckIsLogin();
                    if (IsLogin)
                    {
                        if (!_pageClosed)
                        {
                            _pageClosed = true;
                            Events?.Result?.TrySetResult(TaskResp.True);
                            await (Events?.Page?.CloseAsync() ?? Task.CompletedTask);
                        }
                    }
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