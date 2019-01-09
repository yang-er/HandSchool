using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.Services;
using HandSchool.JLU.ViewModels;
using HandSchool.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[assembly: RegisterService(typeof(SchoolCard))]
namespace HandSchool.JLU.Services
{
    /// <summary>
    /// 校园一卡通的执行服务。
    /// </summary>
    /// <inheritdoc cref="NotifyPropertyChanged" />
    /// <inheritdoc cref="ILoginField" />
    [Entrance("JLU", "校园一卡通", "提供校园卡的查询、充值、挂失等功能。")]
    [UseStorage("JLU", configUsername, configPassword)]
    internal sealed class SchoolCard : NotifyPropertyChanged, ILoginField
    {
        const string configUsername = "jlu.schoolcard.username.txt";
        const string configPassword = "jlu.schoolcard.password.txt";

        const string baseUrl = "http://ykt.jlu.edu.cn:8070";

        const string parsePattern = @"(?<=<div class=\""tableDiv\""><table class=\""" +
            @"mobileT\"" cellpadding=\""0\"" cellspacing=\""0\"">)[\s\S]*(?=</table)";

        #region Login Fields

        bool is_login = false;
        bool auto_login = false;
        bool save_password = false;

        public string Username { get; set; }
        public string Password { get; set; }
        public string CaptchaCode { get; set; }
        public byte[] CaptchaSource { get; set; }

        public string Tips => "校园卡查询密码默认为身份证最后六位数字。";
        public string FormName => "校园卡服务中心";
        public bool NeedLogin => !is_login;
        public bool IsLogin { get => is_login; private set => SetProperty(ref is_login, value); }
        public bool AutoLogin { get => auto_login; set => SetProperty(ref auto_login, value); }
        public bool SavePassword { get => save_password; set => SetProperty(ref save_password, value); }
        public event EventHandler<LoginStateEventArgs> LoginStateChanged;

        public async Task<bool> PrepareLogin()
        {
            try
            {
                WebClient = new AwaredWebClient(baseUrl, Encoding.UTF8);
                await WebClient.GetAsync("", "*/*");
                var login_str = await WebClient.GetAsync("Account/Login", "*/*");
                var captcha_url = Regex.Match(login_str, @"id=""imgCheckCode"" src=""/(\S+)""");
                CaptchaSource = await WebClient.GetAsync(captcha_url.Groups[1].Value, "image/jif", "jif");
                return CaptchaSource != null;
            }
            catch (WebException)
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
        
        public SchoolCard()
        {
            IsLogin = false;
            Username = Core.Configure.Read(configUsername);
            if (Username != "") Password = Core.Configure.Read(configPassword);
            SavePassword = Password != "";
        }

        #endregion

        static string RegularizeHtml(string input)
        {
            return input.Replace("    ", "").Replace("\r", "").Replace("\n", "");
        }

        /// <exception cref="WebException" />
        /// <exception cref="ContentAcceptException" />
        public async Task BasicInfoAsync()
        {
            if (!await this.RequestLogin()) return;
            var resultBasicInfo = await WebClient.GetAsync("SynCard/Manage/BasicInfo", "text/html");
            YktViewModel.Instance.ParseBasicInfo(resultBasicInfo);
        }

        /// <exception cref="WebException" />
        /// <exception cref="ContentAcceptException" />
        public async Task GetPickCardInfo()
        {
            if (!await this.RequestLogin()) return;
            
            var cardList = await WebClient.GetAsync("InfoPub/CardNotice/NFixCardList", "*/*");
            var parsedHtml = Regex.Match(RegularizeHtml(cardList), parsePattern).Value;
            var cardEnumer = PickCardInfo.EnumerateFromHtml("<Root><div><table>" + parsedHtml + "</table></div></Root>");
            YktViewModel.Instance.PickCardInfo.Clear();
            foreach (var item in cardEnumer)
                YktViewModel.Instance.PickCardInfo.Add(item);
        }

        /// <exception cref="OverflowException" />
        /// <exception cref="FormatException" />
        /// <exception cref="WebException" />
        /// <exception cref="JsonException" />
        /// <exception cref="ContentAcceptException" />
        public async Task<bool> ChargeMoney(string money)
        {
            if (!await this.RequestLogin())
                throw new ContentAcceptException("", "用户取消了转账。", "");
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

            WebClient.Headers["Referer"] = baseUrl + "/SynCard/Manage/Transfer";
            var transferReport = await WebClient.PostAsync("SynCard/Manage/TransferPost", post_value);
            var result = transferReport.ParseJSON<YktResult>();

            LastReport = result.msg;
            return result.success;
        }
        
        /// <exception cref="WebException" />
        /// <exception cref="ContentAcceptException" />
        public async Task QueryCost()
        {
            if (!await this.RequestLogin()) return;

            var resultWeek = await WebClient.GetAsync("SynCard/Manage/OneWeekTrjn");
            var parseWeek = Regex.Match(RegularizeHtml(resultWeek), parsePattern).Value;
            var resultDay = await WebClient.GetAsync("SynCard/Manage/CurrentDayTrjn");
            var parseToday = Regex.Match(RegularizeHtml(resultDay), parsePattern).Value;

            var enumHtml = RecordInfo.EnumerateFromHtml("<Root><div><table>" + parseToday + parseWeek + "</table></div></Root>");

            YktViewModel.Instance.RecordInfo.Clear();

            foreach (var item in enumHtml)
                YktViewModel.Instance.RecordInfo.Add(item);
        }

        /// <exception cref="WebException" />
        /// <exception cref="ContentAcceptException" />
        public async Task<bool> SetLost()
        {
            if (!await this.RequestLogin())
                throw new ContentAcceptException("", "用户取消了挂失。", "");

            // First, we should get our card number.
            var value_got = await WebClient.GetAsync("SynCard/Manage/CardLost", "*/*");
            var card_no = Regex.Match(value_got, @"name=""selectCardnos""><option value=""(\S+)"">").Groups[1].Value;

            // Then, go ahead.
            var post_value = new NameValueCollection
            {
                { "CardNo", card_no },
                { "Password", Password.ToBase64() },
                { "selectCardnos", card_no },
            };

            WebClient.Headers["Referer"] = baseUrl + "/SynCard/Manage/CardLost";

            var cardLost = await WebClient.PostAsync("SynCard/Manage/CardLost", post_value);
            var Result = cardLost.ParseJSON<YktResult>();
            LastReport = Result.msg;
            return Result.success || LastReport == "Value cannot be null.\r\nParameter name: key";
        }

        public AwaredWebClient WebClient { get; private set; }

        /// <summary>
        /// 上一个返回的错误
        /// </summary>
        public string LastReport { get; private set; }
    }
}
