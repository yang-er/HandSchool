using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[assembly: RegisterService(typeof(StudentVpn))]
namespace HandSchool.JLU.Services
{
    [UseStorage("JLU", configUsername, configPassword, configRemember)]
    internal sealed class StudentVpn : NotifyPropertyChanged, ILoginField
    {
        const string configUsername = "jlu.vpn.username.txt";
        const string configPassword = "jlu.vpn.password.txt";
        const string configRemember = "jlu.vpn.remember_token.txt";

        const string baseUrl = "https://vpns.jlu.edu.cn";

        #region Login Fields

        bool is_login = false;
        bool auto_login = false;
        bool save_password = false;

        public string Username { get; set; }
        public string Password { get; set; }
        public string CaptchaCode { get; set; }
        public byte[] CaptchaSource { get; set; }
        public string RememberToken => remember_token;

        private string captcha_id;
        private string remember_token;

        public string Tips => "账号为吉林大学学生邮箱的用户名(不包含@mails.jlu.edu.cn）和密码。";
        public string FormName => "学生VPN";
        public bool NeedLogin => !is_login;

        public Task<bool> BeforeLoginForm() => Task.FromResult(true);

        public bool IsLogin
        {
            get => is_login;
            private set => SetProperty(ref is_login, value);
        }

        public bool AutoLogin
        {
            get => auto_login;
            set
            {
                SetProperty(ref auto_login, value);
                if (value) SetProperty(ref save_password, true, nameof(SavePassword));
            }
        }
        
        public bool SavePassword
        {
            get => save_password;
            set
            {
                SetProperty(ref save_password, value);
                if (!value) SetProperty(ref auto_login, false, nameof(AutoLogin));
            }
        }

        public event EventHandler<LoginStateEventArgs> LoginStateChanged;

        public async Task<bool> PrepareLogin()
        {
            try
            {
                WebClient = Core.New<IWebClient>();
                WebClient.BaseAddress = baseUrl;
                WebClient.AddCookie(new System.Net.Cookie("remember_token", remember_token ?? "aaaa"));
                var resp = await WebClient.GetAsync("");

                if (resp.StatusCode == System.Net.HttpStatusCode.Found)
                {
                    var login_str = await WebClient.GetStringAsync("/login?local_login=true");
                    var captcha_url = Regex.Match(login_str, @"name=""captcha_id"" value=""/(\S+)""");

                    if (captcha_url.Success)
                    {
                        captcha_id = captcha_url.Groups[1].Value;
                        var reqMeta = new WebRequestMeta("/captcha/" + captcha_id + ".png", "image/png");
                        var captcha_resp = await WebClient.GetAsync(reqMeta);
                        CaptchaSource = await captcha_resp.ReadAsByteArrayAsync();
                        return CaptchaSource != null;
                    }
                    else
                    {
                        captcha_id = null;
                        return true;
                    }
                }
                else
                {
                    is_login = true;
                    return true;
                }
            }
            catch (WebsException)
            {
                return false;
            }
        }

        public async Task<bool> Login()
        {
            if (Username == "" || Password == "")
            {
                return false;
            }
            else
            {
                Core.Configure.Write(configUsername, Username);
                Core.Configure.Write(configPassword, SavePassword ? Password : "");
            }

            var post_value = new KeyValueDict
            {
                { "auth_type", "local" },
                { "username", Username },
                { "password", Password },
                { "sms_code", "" },
                { "remember_cookie", "on" }
            };

            if (captcha_id != null)
            {
                post_value.Add("captcha_id", captcha_id);
                post_value.Add("captcha", CaptchaCode);
                post_value.Add("needCaptcha", "true");
            }

            try
            {
                var reqMeta = new WebRequestMeta("do-login?local_login=true", WebRequestMeta.All);
                reqMeta.SetHeader("Referer", baseUrl + "/login");
                var resp = await WebClient.PostAsync(reqMeta, post_value);
                
                if (resp.Location == "/login?local_login=true")
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "用户名密码或验证码错误"));
                    return false;
                }
                else if (resp.Location != "/")
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "未知响应：" + resp.Location));
                    return false;
                }
                else
                {
                    foreach (var item in resp.GetHeaders())
                    {
                        if (item.Key.ToLower() != "set-cookie") continue;
                        foreach (var vals in item.Value)
                        {
                            if (!vals.StartsWith("remember_token=")) continue;
                            remember_token = vals.Substring(15, vals.IndexOf(';') - 15);
                            Core.Configure.Write(configRemember, remember_token);
                        }
                    }

                    IsLogin = true;
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Succeeded));
                    return true;
                }
            }
            catch (WebsException ex)
            {
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(ex));
                return IsLogin = false;
            }
        }

        public StudentVpn()
        {
            IsLogin = false;
            Username = Core.Configure.Read(configUsername);
            if (Username != "") Password = Core.Configure.Read(configPassword);
            AutoLogin = SavePassword = Password != "";
            if (AutoLogin) remember_token = Core.Configure.Read(configRemember);
            if (string.IsNullOrWhiteSpace(remember_token)) remember_token = "aaaa";
        }

        #endregion

        public async Task<bool> SetCookieAsync(string host, bool https, string path, string ck_data, string referer)
        {
            var content = "/wengine-vpn/cookie?method=set&host=" + host + "&scheme=" + (https ? "https" : "http");
            content += "&path=" + path + "&ck_data=" + ck_data + "%3B%20expires%3DTue%2C%2025%20Feb%202020%2015%3A52%3A09%20GMT";
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
