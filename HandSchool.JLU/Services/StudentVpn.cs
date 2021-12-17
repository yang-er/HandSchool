using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Models;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HandSchool.Internal;

[assembly: RegisterService(typeof(StudentVpn))]
namespace HandSchool.JLU.Services
{
    [UseStorage("JLU", ConfigUsername, ConfigPassword, ConfigRemember)]
    internal sealed class StudentVpn : NotifyPropertyChanged, ILoginField
    {
        const string ConfigUsername = "jlu.vpn.username.txt";
        const string ConfigPassword = "jlu.vpn.password.txt";
        const string ConfigRemember = "jlu.vpn.remember_token.txt";

        const string BaseUrl = "https://vpns.jlu.edu.cn";

        #region Login Fields

        bool _isLogin = false;
        bool _autoLogin = false;
        bool _savePassword = false;
        public TimeoutManager TimeoutManager { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CaptchaCode { get; set; }
        public byte[] CaptchaSource { get; set; }
        public string RememberToken => _rememberToken;

        private string _captchaId;
        private string _rememberToken;

        public string Tips => "账号为吉林大学学生邮箱的用户名(不包含@mails.jlu.edu.cn）和密码。";
        public string FormName => "学生VPN";
        public bool NeedLogin { get; set; } = true;

        public Task<TaskResp> BeforeLoginForm() => Task.FromResult(TaskResp.True);

        public bool IsLogin
        {
            get => _isLogin;
            private set => SetProperty(ref _isLogin, value);
        }

        public bool AutoLogin
        {
            get => _autoLogin;
            set
            {
                SetProperty(ref _autoLogin, value);
                if (value) SetProperty(ref _savePassword, true, nameof(SavePassword));
            }
        }

        public bool SavePassword
        {
            get => _savePassword;
            set
            {
                SetProperty(ref _savePassword, value);
                if (!value) SetProperty(ref _autoLogin, false, nameof(AutoLogin));
            }
        }

        public event EventHandler<LoginStateEventArgs> LoginStateChanged;
        public async Task<bool> CheckLogin()
        {
            if (!IsLogin || TimeoutManager.IsTimeout())
            {
                IsLogin = false;
            }
            else return true;
            if (await this.RequestLogin() == RequestLoginState.SUCCESSED)
            {
                TimeoutManager.Refresh();
                return true;
            }
            else return false;
        }

        public async Task<TaskResp> PrepareLogin()
        {
            try
            {
                WebClient = Core.New<IWebClient>();
                WebClient.BaseAddress = BaseUrl;
                WebClient.AddCookie(new System.Net.Cookie("remember_token", _rememberToken ?? "aaaa"));
                var resp = await WebClient.GetAsync("");

                if (resp.StatusCode == System.Net.HttpStatusCode.Found)
                {
                    var loginStr = await WebClient.GetStringAsync("/login?local_login=true");
                    var captchaUrl = Regex.Match(loginStr, @"name=""captcha_id"" value=""/(\S+)""");

                    if (captchaUrl.Success)
                    {
                        _captchaId = captchaUrl.Groups[1].Value;
                        var reqMeta = new WebRequestMeta("/captcha/" + _captchaId + ".png", "image/png");
                        var captchaResp = await WebClient.GetAsync(reqMeta);
                        CaptchaSource = await captchaResp.ReadAsByteArrayAsync();
                        return new TaskResp(CaptchaSource != null);
                    }
                    else
                    {
                        _captchaId = null;
                        return TaskResp.True;
                    }
                }
                else
                {
                    _isLogin = true;
                    return TaskResp.True;
                }
            }
            catch (WebsException)
            {
                return TaskResp.False;
            }
        }

        public async Task<TaskResp> Login()
        {
            if (Username == "" || Password == "")
            {
                return TaskResp.False;
            }
            else
            {
                Core.Configure.Write(ConfigUsername, Username);
                Core.Configure.Write(ConfigPassword, SavePassword ? Password : "");
            }

            var postValue = new KeyValueDict
            {
                { "auth_type", "local" },
                { "username", Username },
                { "password", Password },
                { "sms_code", "" },
                { "remember_cookie", "on" }
            };

            if (_captchaId != null)
            {
                postValue.Add("captcha_id", _captchaId);
                postValue.Add("captcha", CaptchaCode);
                postValue.Add("needCaptcha", "true");
            }

            try
            {
                var reqMeta = new WebRequestMeta("do-login?local_login=true", WebRequestMeta.All);
                reqMeta.SetHeader("Referer", BaseUrl + "/login");
                var resp = await WebClient.PostAsync(reqMeta, postValue);

                if (resp.Location == "/login?local_login=true")
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "用户名密码或验证码错误"));
                    return TaskResp.False;
                }
                else if (resp.Location != "/")
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "未知响应：" + resp.Location));
                    return TaskResp.False;
                }
                else
                {
                    foreach (var item in resp.GetHeaders())
                    {
                        if (item.Key.ToLower() != "set-cookie") continue;
                        foreach (var vals in item.Value)
                        {
                            if (!vals.StartsWith("remember_token=")) continue;
                            _rememberToken = vals.Substring(15, vals.IndexOf(';') - 15);
                            Core.Configure.Write(ConfigRemember, _rememberToken);
                        }
                    }

                    IsLogin = true;
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Succeeded));
                    return TaskResp.True;
                }
            }
            catch (WebsException ex)
            {
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(ex));
                IsLogin = false;
                return TaskResp.False;
            }
        }

        public StudentVpn()
        {
            IsLogin = false;
            Username = Core.Configure.Read(ConfigUsername);
            if (Username != "") Password = Core.Configure.Read(ConfigPassword);
            AutoLogin = SavePassword = !string.IsNullOrEmpty(Password);
            if (AutoLogin) _rememberToken = Core.Configure.Read(ConfigRemember);
            if (string.IsNullOrWhiteSpace(_rememberToken)) _rememberToken = "aaaa";
            else NeedLogin = false;
            TimeoutManager = new TimeoutManager(43200);
        }

        #endregion

        public async Task<bool> SetCookieAsync(string host, bool https, string path, string ckData, string referer)
        {
            var content = "/wengine-vpn/cookie?method=set&host=" + host + "&scheme=" + (https ? "https" : "http");
            content += "&path=" + path + "&ck_data=" + ckData + "%3B%20expires%3DTue%2C%2025%20Feb%202020%2015%3A52%3A09%20GMT";
            var request = new WebRequestMeta(content, "*/*");
            request.SetHeader("Referer", referer);
            var resp = await WebClient.PostAsync(request, "", null);
            return await resp.ReadAsStringAsync() == "success";
        }

        public IWebClient WebClient { get; private set; }

        /// <summary>
        /// 上一个返回的错误
        /// </summary>
        public string LastReport { get; private set; }
    }
}
