using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.JLU.JsonObject;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json;

namespace HandSchool.JLU
{
    class SchoolCard : NotifyPropertyChanged, ILoginField
    {
        const string password_config = "jlu.schoolcard.password.conf";

        #region Login Fields

        bool _islogin = false;
        bool _autologin = false;
        bool _savepassword = false;

        public string Username { get; set; }
        public string Password { get; set; }
        public string InnerError { get; private set; }
        public string CaptchaCode { get; set; }
        public byte[] CaptchaSource { get; set; }

        public string Tips => "校园卡查询密码默认为身份证最后六位数字。";
        public bool NeedLogin => !_islogin;
        public bool IsLogin { get => _islogin; private set => SetProperty(ref _islogin, value); }
        public bool AutoLogin { get => _autologin; set => SetProperty(ref _autologin, value); }
        public bool SavePassword { get => _savepassword; set => SetProperty(ref _savepassword, value); }
        public event EventHandler<LoginStateEventArgs> LoginStateChanged;

        public async Task<bool> PrepareLogin()
        {
            var prepare = await WebClient.GetAsync("", "*/*");
            var login_str = await WebClient.GetAsync("Account/Login", "*/*");
            var captcha_url = Regex.Match(login_str, @"id=""imgCheckCode"" src=""/(\S+)""");
            CaptchaSource = await WebClient.GetAsync(captcha_url.Groups[1].Value, "image/jif", "jif");
            return CaptchaSource != null;
        }

        public async Task<bool> Login()
        {
            var post_value = new NameValueCollection
            {
                { "SignType", "SynSno" },
                { "UserAccount", Username },
                { "Password", Helper.ToBase64(Password) },
                { "NextUrl", "aHR0cDovLzIwMi45OC4xOC4yNDk6ODA3MC9TeW5DYXJkL01hbmFnZS9CYXNpY0luZm8=" },
                { "CheckCode", CaptchaCode },
                { "openid", "" },
                { "Schoolcode", "JLU" }
            };

            WebClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134";
            WebClient.Headers["X-Requested-With"] = "XMLHttpRequest";
            WebClient.Headers["Referer"] = "http://ykt.jlu.edu.cn:8070/Account/Login?next=aHR0cDovLzIwMi45OC4xOC4yNDk6ODA3MC9TeW5DYXJkL01hbmFnZS9CYXNpY0luZm8=";
            WebClient.Headers["Origin"] = "http://ykt.jlu.edu.cn:8070";
            try
            {
                LastReport = await WebClient.PostAsync("Account/Login", post_value);

                if (LastReport == "被拒绝")
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "软件版本过老，无法登录。"));
                    return IsLogin = false;
                }

                var result = Helper.JSON<YktLogin>(LastReport);
                if (!result.success)
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, result.msg));
                    return IsLogin = false;
                }

                System.Diagnostics.Debug.WriteLine(await WebClient.GetAsync("SynCard/Manage/BasicInfo", "text/html"));
                return IsLogin = true;
            }
            catch (WebException ex)
            {
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(ex));
            }
            catch (JsonException)
            {
                System.Diagnostics.Debug.WriteLine(LastReport);
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "服务器响应有问题。"));
            }
        }

        #endregion

        public AwaredWebClient WebClient { get; } = new AwaredWebClient("http://ykt.jlu.edu.cn:8070", Encoding.UTF8);

        public string LastReport { get; private set; }
    }
}
