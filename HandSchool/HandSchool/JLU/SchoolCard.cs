using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.ViewModels;
using HandSchool.Models;
using HandSchool.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandSchool.JLU
{
    class SchoolCard : NotifyPropertyChanged, ILoginField
    {
        internal const string config_username = "jlu.schoolcard.username.txt";
        internal const string config_password = "jlu.schoolcard.password.txt";
        internal const string config_school = "jlu.schoolcard.detail.json";

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
        public string FormName => "校园卡服务中心";
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
            if (Username == "" || Password == "")
            {
                return false;
            }
            else
            {
                Core.WriteConfig(config_username, Username);
                Core.WriteConfig(config_password, SavePassword ? Password : "");
            }

            var post_value = new NameValueCollection
            {
                { "SignType", "SynSno" },
                { "UserAccount", Username },
                { "Password", Password.ToBase64() },
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

                var result = LastReport.ParseJSON<YktResult>();
                if (!result.success)
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, result.msg));
                    return false;
                }

                await BasicInfoAsync();
            }
            catch (WebException ex)
            {
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(ex));
                return IsLogin = false;
            }
            catch (JsonException)
            {
                Core.Log(LastReport);
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "服务器响应有问题。"));
                return IsLogin = false;
            }
            
            IsLogin = true;
            LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Succeeded));
            return true;
        }

        public async Task<bool> RequestLogin()
        {
            if (AutoLogin && !IsLogin) await Login();
            if (!IsLogin) await LoginViewModel.RequestAsync(this);
            return IsLogin;
        }

        #endregion
        
        public SchoolCard()
        {
            IsLogin = false;
            Username = Core.ReadConfig(config_username);
            if (Username != "") Password = Core.ReadConfig(config_password);
            SavePassword = Password != "";
        }

        public async Task BasicInfoAsync()
        {
            LastReport = await WebClient.GetAsync("SynCard/Manage/BasicInfo", "text/html");
            YktViewModel.Instance.BasicInfo.ParseFromHtml(LastReport);
        }

        /// <exception cref="WebException" />
        /// <exception cref="ContentAcceptException" />
        public async Task GetPickCardInfo()
        {
            if (!await RequestLogin()) return;
            
            string Html = await WebClient.GetAsync("InfoPub/CardNotice/NFixCardList", "*/*");
            Html = Html.Replace("    ", "")
                        .Replace("\r", "")
                        .Replace("\n", "");
            var PrasedHtml = Regex.Match(Html, @"(?<=<div class=\""tableDiv\""><table class=\""mobileT\"" cellpadding=\""0\"" cellspacing=\""0\"">)[\s\S]*(?=</table)");
            var enumer = PickCardInfo.EnumerateFromHtml("<Root>" + "<div>" + "<table>" + PrasedHtml.Value + "</table>" + "</div>" + "</Root>");
            YktViewModel.Instance.PickCardInfo.Clear();
            foreach (var item in enumer)
                YktViewModel.Instance.PickCardInfo.Add(item);
        }

        /// <exception cref="OverflowException" />
        /// <exception cref="FormatException" />
        /// <exception cref="WebException" />
        /// <exception cref="JsonException" />
        /// <exception cref="ContentAcceptException" />
        public async Task<bool> ChargeMoney(string money)
        {
            if (!await RequestLogin()) return false;
            var true_money = double.Parse(money);
            if (true_money > 200 || true_money <= 0)
                throw new OverflowException();

            var post_value = new NameValueCollection
            {
                { "FromCard", "bcard" },
                { "ToCard", "card" },
                { "Amount", true_money.ToString("f2")},
                { "Password", Password.ToBase64() }
            };

            WebClient.Headers["Referer"] = "http://ykt.jlu.edu.cn:8070/SynCard/Manage/Transfer";
            LastReport = await WebClient.PostAsync("SynCard/Manage/TransferPost", post_value);
            var Result = LastReport.ParseJSON<YktResult>();

            if (!Result.success)
            {
                LastReport = Result.msg;
                return false;
            }
            else
            {
                return true;
            }
        }
        
        /// <exception cref="WebException" />
        /// <exception cref="ContentAcceptException" />
        public async Task QueryCost()
        {
            if (!await RequestLogin()) return;
            string ResultHtml = await WebClient.GetAsync("SynCard/Manage/TrjnHistory");
            ResultHtml = ResultHtml.Replace("    ", "")
                                   .Replace("\r", "")
                                   .Replace("\n", "");

            string ToPrase = Regex.Match(ResultHtml, @"(?<=<div class=\""tableDiv\""><table class=\""mobileT\"" cellpadding=\""0\"" cellspacing=\""0\"">)[\s\S]*(?=</table)").Value;
            var enumer = RecordInfo.EnumerateFromHtml("<Root>" + "<div>" + "<table>" + ToPrase + "</table>" + "</div>" + "</Root>");
            YktViewModel.Instance.RecordInfo.Clear();
            foreach(var i in enumer)
                YktViewModel.Instance.RecordInfo.Add(i);
        }

        public AwaredWebClient WebClient { get; } = new AwaredWebClient("http://ykt.jlu.edu.cn:8070", Encoding.UTF8);

        public string LastReport { get; private set; }
    }
}
