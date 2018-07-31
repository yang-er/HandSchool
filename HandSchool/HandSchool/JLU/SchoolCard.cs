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
using HandSchool.JLU.ViewModels;
using HandSchool.JLU.Models;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using HandSchool.ViewModels;

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
            
            try
            {
                WebClient.Headers["Referer"] = "http://ykt.jlu.edu.cn:8070/Account/Login?next=aHR0cDovLzIwMi45OC4xOC4yNDk6ODA3MC9TeW5DYXJkL01hbmFnZS9CYXNpY0luZm8=";
                LastReport = await WebClient.PostAsync("Account/Login", post_value);

                if (LastReport == "被拒绝")
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "软件版本过老，无法登录。"));
                    return false;
                }

                var result = Helper.JSON<YktResult>(LastReport);
                if (!result.success)
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, result.msg));
                    return false;
                }

                YktViewModel.Instance.BasicInfo.ParseFromHtml(await WebClient.GetAsync("SynCard/Manage/BasicInfo", "text/html"));
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Succeeded));
                // var a = await ChargeMoney(0.1);
                
                return IsLogin = true;
            }
            catch (WebException ex)
            {
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(ex));
                return IsLogin = false;
            }
            catch (JsonException)
            {
                System.Diagnostics.Debug.WriteLine(LastReport);
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "服务器响应有问题。"));
                return IsLogin = false;
            }
        }

        #endregion

        public async Task<bool> RequestLogin()
        {
            if (AutoLogin && !IsLogin) await Login();
            if (!IsLogin) await LoginViewModel.RequestAsync(this);
            return IsLogin;
        }

        public async Task<IEnumerable<PickCardInfo>> GetPickCardInfo()
        {
            //TODO:检查登陆
            try
            {
                string Html = await WebClient.GetAsync("InfoPub/CardNotice/NFixCardList", "*/*");
                Html = Html.Replace("    ", "")
                           .Replace("\r", "")
                           .Replace("\n", "");
                var PrasedHtml = Regex.Match(Html, @"(?<=<div class=\""tableDiv\""><table class=\""mobileT\"" cellpadding=\""0\"" cellspacing=\""0\"">)[\s\S]*(?=</table)");
                return PickCardInfo.EnumerateFromHtml("<Root>" + "<div>" + "<table>" + PrasedHtml.Value + "</table>" + "</div>" + "</Root>");
            }
            catch(WebException ex)
            {
                //TODO:  返回的对吗emmmm
                return new List<PickCardInfo>();
            }
            
        }
        public async Task<bool> ChargeMoney(double Money)
        {
            var post_value = new NameValueCollection
            {
                { "Amount", Money.ToString("f1")},
                { "FromCard", "bcard" },
                { "Password", Helper.ToBase64(Password) },
                { "ToCard", "card" },
            };
            try
            {
               
                var Response = await WebClient.PostAsync("SynCard/Manage/TransferPost", post_value);
                var Result = Helper.JSON<YktResult>(Response);
                if (!Result.success)
                {
                    return false;
                }
            }
            catch(JsonException ex)
            {
                return false;
            }

            return true;

        }

        public AwaredWebClient WebClient { get; } = new AwaredWebClient("http://ykt.jlu.edu.cn:8070", Encoding.UTF8);

        public string LastReport { get; private set; }
    }
}
