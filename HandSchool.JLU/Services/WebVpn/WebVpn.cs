using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HandSchool.Internal;
using HandSchool.ViewModels;
using Newtonsoft.Json.Linq;
using Xamarin.Forms.Internals;
using Newtonsoft.Json;

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
            _util = new WebVpnUtil();
            _cookieDictionary = new NamedCookieDictionary();
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

            CookiesFilter(
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

        private readonly NamedCookieDictionary _cookieDictionary;
        
        private string _encryptedPassword;

        public IEnumerable<Cookie> GetLoginCookies()
        {
            return _cookieDictionary.Values.ToArray();
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
                if (_isLogin == value) return;
                SetProperty(ref _isLogin, value);
                if (value)
                {
                    _proxyClients.ForEach(AddCookie);
                }
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
                ReInitWebClient();
                AddCookie(WebClient);
                var response = await WebClient.GetStringAsync("user/info");
                return IsLogin = response.ParseJSON<JToken>()?["username"]?.ToString().IsNotBlank() == true;
            }
            catch (WebsException ex)
            {
                Core.Logger.WriteException(ex);
                return IsLogin = false;
            }
            catch (JsonException)
            {
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

            var success = false;
            try
            {
                using var post = await WebClient.PostAsync(req, loginData);
                var msg = await post.ReadAsStringAsync();
                if (msg.IsBlank()) return false;
                var resp = msg.ParseJSON<JObject>();

                if (resp?["success"]?.ToString().Trim().ToLower() == "true")
                {
                    var cookies = WebClient.Cookie.GetCookies(new Uri("https://webvpn.jlu.edu.cn"));
                    NamedCookieDictionary.Filter(cookies.Cast<Cookie>(), TicketName, TokenName)
                        .ForEach(c => _cookieDictionary.OnlyUpdateValue(c.Name, c.Value));
                    success = true;
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
                            if (jo?["success"]?.ToString().Trim().ToLower() == "true")
                            {
                                success = true;
                            }
                        }

                        break;
                }
            }
            catch //WebsException, JsonException
            {
                success = false;
            }

            if (!success) return false;

            IsLogin = false;
            SaveCookies();
            return true;
        }

        private void ReInitWebClient()
        {
            if (WebClient is null)
            {
                WebClient = new HttpClientImpl {StringBaseAddress = "https://webvpn.jlu.edu.cn/"};
                return;
            }

            if (!WebClient.Cookie.Clear())
            {
                WebClient?.Dispose();
                WebClient = new HttpClientImpl {StringBaseAddress = "https://webvpn.jlu.edu.cn/"};
            }
        }

        public async Task<bool> CheckLogin()
        {
            //距离上次联网查验时间小于设定时间时，直接返回
            if (!TimeoutManager.NotInit && !TimeoutManager.IsTimeout())
                return IsLogin = true;

            //进行联网查验
            if (await CheckIsLogin())
            {
                TimeoutManager.Refresh();
                return IsLogin = true;
            }

            //如果未登录，则尝试用保存的密码进行静默登录
            if (_cookieDictionary.Count != 0 && Username.IsNotBlank() && _encryptedPassword.IsNotBlank())
            {
                if (await TryLoginLegacyAsync())
                {
                    TimeoutManager.Refresh();
                    return IsLogin = true;
                }
            }

            //如果静默登录失败，则请求进行人工登录
            IsLogin = false;
            if (await LoginViewModel.RequestAsync(this) == RequestLoginState.Success)
            {
                IsLogin = true;
                TimeoutManager.Refresh();
            }

            return IsLogin;
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

        private readonly HashSet<IWebClient> _proxyClients = new HashSet<IWebClient>();

        /// <summary>
        /// 在Cookie序列中选出WebVpn登录所需的
        /// </summary>
        private bool CookiesFilter(IEnumerable<Cookie> cookies)
        {
            if (cookies is null) return false;
            var last = _cookieDictionary.Version;
            NamedCookieDictionary.Filter(cookies, TicketName, TokenName)
                .ForEach(c => _cookieDictionary.TryOnlyUpdateValue(c));
            return last != _cookieDictionary.Version;
        }

        private void SaveCookies()
        {
            Core.App.Loader.JsonManager.InsertOrUpdateTable(new ServerJson
            {
                JsonName = ConfigCookies,
                Json = _cookieDictionary.ToJson()
            });
        }

        public async Task<Cookie[]> GetCookiesAsync(bool isHttps, string domain, string path)
        {
            try
            {
                var cookieStr = (await WebClient.GetStringAsync(
                    $"wengine-vpn/cookie?method=get&host={domain}&scheme={(isHttps ? "https" : "http")}&path={path}"));
                return
                    cookieStr.Split(';')
                        .Select(a => a.Split('='))
                        .Where(a => a.Length == 2)
                        .Select(a =>
                        {
                            a[0] = a[0].Trim();
                            a[1] = a[1].Trim();
                            return a;
                        })
                        .Select(a => new Cookie {Domain = domain, Path = path, Name = a[0], Value = a[1]}).ToArray();
            }
            catch
            {
                return Array.Empty<Cookie>();
            }
        }

        public async Task<bool> SetCookieAsync(bool isHttps, string domain, string path, string name, string value)
        {
            var realPath = path.Trim();
            
            if (!realPath.StartsWith("/"))
                realPath = "/" + realPath;

            if (!realPath.EndsWith("/"))
                realPath += '/';

            try
            {
                var target =
                    $"wengine-vpn/cookie?method=set&host={domain}&scheme={(isHttps ? "https" : "http")}&path={realPath}&ck_data={name}={value}";
                return await WebClient.GetStringAsync(target) == "success";
            }
            catch
            {
                return false;
            }
        }
    }
}