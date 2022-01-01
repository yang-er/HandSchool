using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using HandSchool.Internal;
using HandSchool.ViewModels;
using Xamarin.Forms;

[assembly: RegisterService(typeof(WebVpn))]
namespace HandSchool.JLU.Services
{
    [UseStorage("JLU", ConfigUsername, ConfigPassword, ConfigRemember, ConfigTicket)]
    internal sealed class WebVpn : NotifyPropertyChanged, IWebLoginField
    {
        const string ConfigUsername = "jlu.vpn.username.txt";
        const string ConfigPassword = "jlu.vpn.password.txt";
        const string ConfigRemember = "jlu.vpn.remember_token.txt";
        const string ConfigTicket = "jlu.vpn.ticket.txt";
        
        #region Login Fields
        public string LoginUrl => "https://webvpn.jlu.edu.cn/logout";
        bool _isLogin;
        public TimeoutManager TimeoutManager { get; set; }
        public string Username { get; set; }
        public bool IsWeb => true;
        public string Password { get; set; }
        public string CaptchaCode { get; set; }
        public byte[] CaptchaSource { get; set; }
        public string RememberToken => _rememberToken;

        private string _rememberToken;
        public string Tips => "账号为吉林大学学生邮箱的用户名(不包含@mails.jlu.edu.cn）和密码。";
        public string FormName => "VPN";
        public bool NeedLogin => !IsLogin;
        public Task<TaskResp> BeforeLoginForm() => Task.FromResult(TaskResp.True);

        public void AddCookie(IWebClient webClient)
        {
            foreach(var cookie in GetLoginCookies())
            {
                webClient.Cookie.Add(cookie);
            }
        }
        public Cookie[] GetLoginCookies()
        {
            return new[]
            {
                new Cookie("remember_token", Loader.Vpn.RememberToken, "/", "webvpn.jlu.edu.cn"),
                new Cookie("wengine_vpn_ticketwebvpn_jlu_edu_cn", Loader.Vpn.Ticket, "/", "webvpn.jlu.edu.cn")
            };
        }
        public bool IsLogin
        {
            get => _isLogin;
            private set => SetProperty(ref _isLogin, value);
        }

        public bool AutoLogin
        {
            get => true;
            set{}
        }

        public bool SavePassword
        {
            get => true;
            set {}
        }
        private string _ticket;
        public string Ticket => _ticket;

        public event EventHandler<LoginStateEventArgs> LoginStateChanged;
        private async Task<bool> CheckIsLogin()
        {
            AddCookie(WebClient);
            var response = await WebClient.GetAsync("https://webvpn.jlu.edu.cn");
            var res = await response.ReadAsStringAsync();
            return res.Contains("注销");
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
                        return true;
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
                    return true;
                }
            }
            return await LoginViewModel.RequestAsync(this) == RequestLoginState.Success;
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
        public WebVpn()
        {
            IsLogin = false;
            Username = Core.Configure.Read(ConfigUsername);
            if (Username != "") Password = Core.Configure.Read(ConfigPassword);
            _rememberToken = Core.Configure.Read(ConfigRemember);
            _ticket = Core.Configure.Read(ConfigTicket);
            Events = new WebLoginPageEvents
            {
                WebViewEvents = new HSWebViewEvents()
            };
            Events.WebViewEvents.Navigated += OnNavigated;
            Events.WebViewEvents.Navigating += OnNavigating;
            TimeoutManager = new TimeoutManager(60);
            WebClient = Core.New<IWebClient>();
            if (!string.IsNullOrWhiteSpace(_ticket) && !string.IsNullOrWhiteSpace(_rememberToken))
            {
                WebClient.Cookie.Add(new Cookie("remember_token", _rememberToken, "/login", "webvpn.jlu.edu.cn"));
                WebClient.Cookie.Add(new Cookie("wengine_vpn_ticketwebvpn_jlu_edu_cn", _ticket, "/login", "webvpn.jlu.edu.cn"));
            }
            Task.Run(async () =>
            {
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
                Username = await Events.WebViewEvents.EvaluateJavaScriptAsync("document.getElementById('user_name').value");
                Password = await Events.WebViewEvents.EvaluateJavaScriptAsync("document.getElementsByName('password')[0].value");
                Core.Configure.Write(ConfigUsername, Username);
                Core.Configure.Write(ConfigPassword, Password);
            }
        }
        private async void OnNavigated(object s, WebNavigatedEventArgs e)
        {
            if (e.Url == "https://webvpn.jlu.edu.cn/login")
            {
                await Events.WebViewEvents.EvaluateJavaScriptAsync(
                    "let a = document.getElementsByName('remember_cookie')[0]; " +
                    "a.checked=true; " +
                    "document.getElementsByClassName('remember-field')[0].hidden=true;") ;
                if (!string.IsNullOrWhiteSpace(Username))
                {
                    await Events.WebViewEvents.EvaluateJavaScriptAsync($"document.getElementById('user_name').value='{Username}'");
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
                        if (_rememberToken != cookie[i].Value)
                        {
                            _rememberToken = cookie[i].Value;
                            updated = true;
                        }
                    }

                    if (cookie[i].Name == "wengine_vpn_ticketwebvpn_jlu_edu_cn")
                    {
                        if (_ticket != cookie[i].Value)
                        {
                            _ticket = cookie[i].Value;
                            updated = true;
                        }
                    }
                }

                if (updated)
                {
                    Core.Configure.Write(ConfigRemember, _rememberToken);
                    Core.Configure.Write(ConfigTicket, _ticket);
                }
                IsLogin = await CheckIsLogin();
                if (IsLogin)
                {
                    await (Events?.Page?.CloseAsync() ?? Task.CompletedTask);
                    Events?.Result?.TrySetResult(TaskResp.True);
                }
            }
        }
    }
}
