using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using HandSchool.Internal;
using HandSchool.ViewModels;
using Newtonsoft.Json.Linq;
using Xamarin.Forms.Internals;

[assembly: RegisterService(typeof(WebVpn))]

namespace HandSchool.JLU.Services
{
    [UseStorage("JLU")]
    public sealed partial class WebVpn : NotifyPropertyChanged, IWebLoginField
    {
        /// <summary>
        /// 设置是否使用Vpn
        /// </summary>
        [Settings("使用WebVPN", "使用WebVPN连接各种系统，建议在内网时关闭此选项。切换后需要重启APP。")]
        public static bool UseVpn { get; set; }

        private WebVpn()
        {
            IsLogin = false;
            var acc = Core.App.Loader.AccountManager.GetItemWithPrimaryKey(ServerName);
            if (acc != null)
            {
                Username = acc.UserName;
                var info = acc.Password.Split(' ');
                if (info.Length > 0)
                {
                    if (info[0].IsNotBlank())
                    {
                        Password = info[0];
                        if (info.Length > 1)
                        {
                            _encryptedPassword = info[1];
                        }
                    }
                }
            }

            SaveCookies(
                Core.App.Loader.JsonManager
                    .GetItemWithPrimaryKey(ConfigCookies)
                    ?.ToObject<List<Cookie>>());

            Events = new WebLoginPageEvents {WebViewEvents = new HSWebViewEvents()};
            Events.WebViewEvents.Navigated += OnNavigated;
            Events.WebViewEvents.Navigating += OnNavigating;
            Events.WebViewEvents.ReceivingJsData += OnReceivingJsData;
            TimeoutManager = new TimeoutManager(60);

            Task.Run(async () => { await CheckIsLogin(); });
        }

        private const string ServerName = "WebVpn";
        const string ConfigCookies = "webvpn.logincookies";
        public static WebVpn Instance => UseVpn ? Lazy.Value : null;
        private static readonly Lazy<WebVpn> Lazy = new Lazy<WebVpn>(() => new WebVpn());
        private bool _pageClosed;

        #region Login Fields

        public string LoginUrl => "https://webvpn.jlu.edu.cn/login";
        private bool _isLogin;
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

        private string _encryptedPassword;

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
                    ReInitWebClient();
                    AddCookie(WebClient);
                    _loginCookiesChanged = false;
                }

                using var response = await WebClient.GetAsync("https://webvpn.jlu.edu.cn");
                var res = await response.ReadAsStringAsync();
                return IsLogin = res.Contains("注销");
            }
            catch (WebsException ex)
            {
                Core.Logger.WriteException(ex);
                return IsLogin = false;
            }
        }

        private async Task<bool> TryLoginLegacyAsync()
        {
            ReInitWebClient();
            var loginData = new KeyValueDict
            {
                {"auth_type", "local"},
                {"username", Username},
                {"password", _encryptedPassword},
                {"remember_cookie", "on"},
            };
            var req = new WebRequestMeta("do-login", WebRequestMeta.All);
            using var post = await WebClient.PostAsync(req, loginData);
            var msg = await post.ReadAsStringAsync();
            if (msg.IsBlank()) return false;
            var resp = msg.ParseJSON<JObject>();
            if (resp?["success"]?.ToString().Trim().ToLower() == "true")
            {
                var cookies = WebClient.Cookie.GetCookies(new Uri("https://webvpn.jlu.edu.cn"));
                cookies.ToEnumerable().ForEach(c =>
                {
                    switch (c.Name)
                    {
                        case TicketName:
                            Ticket.Value = c.Value;
                            break;
                        case TokenName:
                            if (RememberToken is { } rememberToken)
                                rememberToken.Value = c.Value;
                            break;
                    }
                });
                AddCookie(WebClient);
                IsLogin = false;
                return true;
            }

            switch (resp?["error"]?.ToString().Trim().ToLower())
            {
                case "need_confirm":
                    using (var confirm = await WebClient.PostAsync(
                               new WebRequestMeta("do-confirm-login", WebRequestMeta.All),
                               null))
                    {
                        var text = await confirm.ReadAsStringAsync();
                        if (text.IsBlank()) return false;
                        var jo = text.ParseJSON<JObject>();
                        return jo?["success"]?.ToString().Trim().ToLower() == "true";
                    }
            }

            return false;
        }

        private void ReInitWebClient()
        {
            if (WebClient is null)
            {
                WebClient = new HttpClientImpl {BaseAddress = "https://webvpn.jlu.edu.cn/"};
                return;
            }

            if (!WebClient.Cookie.Clear())
            {
                WebClient?.Dispose();
                WebClient = new HttpClientImpl {BaseAddress = "https://webvpn.jlu.edu.cn/"};
            }
        }

        public async Task<bool> CheckLogin()
        {
            if (TimeoutManager.NotInit || TimeoutManager.IsTimeout())
            {
                if (await CheckIsLogin()) return true;
                if (Ticket is { } && Username.IsNotBlank() && _encryptedPassword.IsNotBlank())
                {
                    if (await TryLoginLegacyAsync()) return IsLogin = true;
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
                await (WebClient?.GetAsync("https://webvpn.jlu.edu.cn/logout") ?? Task.CompletedTask);
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

        #endregion

        public IWebClient WebClient { get; private set; }
        public WebLoginPageEvents Events { get; }

        private readonly Dictionary<string, string> _proxyUrl = new Dictionary<string, string>();
        private readonly List<IWebClient> _proxyClients = new List<IWebClient>();

        public string GetProxyUrl(string ori)
        {
            if (_proxyUrl.ContainsKey(ori))
            {
                return _proxyUrl[ori];
            }

            throw new KeyNotFoundException($"url {ori} has not registered");
        }
        
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

        /// <summary>
        /// 在Cookie序列中选出WebVpn登录所需的
        /// </summary>
        private bool SaveCookies(IEnumerable<Cookie> cookies)
        {
            var changed = false;
            cookies?.ForEach(c =>
            {
                switch (c.Name)
                {
                    case TicketName:
                        if (Ticket?.Value == c.Value) break;
                        Ticket = c;
                        changed = true;
                        break;
                    case TokenName:
                        if (RememberToken?.Value == c.Value) return;
                        RememberToken = c;
                        changed = true;
                        break;
                }
            });
            return changed;
        }
    }
}