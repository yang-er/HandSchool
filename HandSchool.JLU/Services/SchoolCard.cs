using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.Services;
using HandSchool.JLU.ViewModels;
using HandSchool.Models;
using Newtonsoft.Json;
using System;
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
        const string nextUrl = "aHR0cDovLzIwMi45OC4xOC4yNDk6ODA3MC9TeW5DYXJkL01hbmFnZS9CYXNpY0luZm8=";
        const string parsePattern = @"(?<=<div class=\""tableDiv\""><table class=\""" +
            @"mobileT\"" cellpadding=\""0\"" cellspacing=\""0\"">)[\s\S]*(?=</table)";

        #region Login Fields

        bool is_login = false;
        bool auto_login = false;
        bool save_password = false;

        public Task<bool> BeforeLoginForm() => Task.FromResult(true);

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
                WebClient = Core.New<IWebClient>();
                WebClient.BaseAddress = baseUrl;
                await WebClient.GetAsync("");
                var login_str = await WebClient.GetStringAsync("Account/Login");
                var captcha_url = Regex.Match(login_str, @"id=""imgCheckCode"" src=""/(\S+)""");

                var reqMeta = new WebRequestMeta(captcha_url.Groups[1].Value, "image/jif");
                var captcha_resp = await WebClient.GetAsync(reqMeta);
                CaptchaSource = await captcha_resp.ReadAsByteArrayAsync();
                return CaptchaSource != null;
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
                { "SignType", "SynSno" },
                { "UserAccount", Username },
                { "Password", Password.ToBase64() },
                { "NextUrl", nextUrl },
                { "CheckCode", CaptchaCode },
                { "openid", "" },
                { "Schoolcode", "JLU" }
            };
            
            try
            {
                var reqMeta = new WebRequestMeta("Account/Login", WebRequestMeta.Json);
                reqMeta.SetHeader("Referer", baseUrl + "/Account/Login?next=" + nextUrl);
                var resp = await WebClient.PostAsync(reqMeta, post_value);
                LastReport = await resp.ReadAsStringAsync();

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
            catch (WebsException ex)
            {
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(ex));
                return IsLogin = false;
            }
            catch (JsonException)
            {
                this.WriteLog("Unexpected value received <<<EOF\n" + LastReport + "\nEOF;");
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

        /// <exception cref="WebsException" />
        public async Task BasicInfoAsync()
        {
            if (!await this.RequestLogin()) return;
            var resultBasicInfo = await WebClient.GetStringAsync("SynCard/Manage/BasicInfo", "text/html");
            YktViewModel.Instance.ParseBasicInfo(resultBasicInfo);
        }

        /// <exception cref="WebsException" />
        public async Task GetPickCardInfo()
        {
            if (!await this.RequestLogin()) return;
            
            var cardList = await WebClient.GetStringAsync("InfoPub/CardNotice/NFixCardList", "*/*");
            var parsedHtml = Regex.Match(RegularizeHtml(cardList), parsePattern).Value;
            var cardEnumer = PickCardInfo.EnumerateFromHtml("<Root><div><table>" + parsedHtml + "</table></div></Root>");
            YktViewModel.Instance.PickCardInfo.Clear();
            foreach (var item in cardEnumer)
                YktViewModel.Instance.PickCardInfo.Add(item);
        }

        /// <exception cref="OverflowException" />
        /// <exception cref="FormatException" />
        /// <exception cref="WebsException" />
        /// <exception cref="JsonException" />
        public async Task<bool> ChargeMoney(string money)
        {
            if (!await this.RequestLogin())
            {
                LastReport = "用户取消了转账。";
                return false;
            }

            var true_money = double.Parse(money);
            if (true_money > 200 || true_money <= 0)
                throw new OverflowException();

            var post_value = new KeyValueDict
            {
                { "FromCard", "bcard" },
                { "ToCard", "card" },
                { "Amount", true_money.ToString("f2")},
                { "Password", Password.ToBase64() }
            };

            var reqMeta = new WebRequestMeta("SynCard/Manage/TransferPost", WebRequestMeta.Json);
            reqMeta.SetHeader("Referer", baseUrl + "/SynCard/Manage/Transfer");
            var response = await WebClient.PostAsync(reqMeta, post_value);
            var transferReport = await response.ReadAsStringAsync();
            var result = transferReport.ParseJSON<YktResult>();

            LastReport = result.msg;
            return result.success;
        }
        
        /// <exception cref="WebsException" />
        public async Task QueryCost()
        {
            if (!await this.RequestLogin()) return;
            const string noHistory = "当前查询条件内没有流水记录";

            var resultWeek = await WebClient.GetStringAsync("SynCard/Manage/OneWeekTrjn");
            var resultDay = await WebClient.GetStringAsync("SynCard/Manage/CurrentDayTrjn");

            if (resultDay.Contains(noHistory) && resultWeek.Contains(noHistory))
            {
                resultWeek = await WebClient.GetStringAsync("SynCard/Manage/TrjnHistory");
                if (resultWeek.Contains(noHistory)) return;
            }

            var parseWeek = Regex.Match(RegularizeHtml(resultWeek), parsePattern).Value;
            var parseToday = Regex.Match(RegularizeHtml(resultDay), parsePattern).Value;
            var enumHtml = RecordInfo.EnumerateFromHtml("<Root><div><table>" + parseToday + parseWeek + "</table></div></Root>");

            YktViewModel.Instance.RecordInfo.Clear();

            foreach (var item in enumHtml)
                YktViewModel.Instance.RecordInfo.Add(item);
        }

        /// <exception cref="WebsException" />
        public async Task<bool> SetLost()
        {
            if (!await this.RequestLogin())
            {
                LastReport = "用户取消了挂失。";
                return false;
            }

            // First, we should get our card number.
            var value_got = await WebClient.GetStringAsync("SynCard/Manage/CardLost");
            var card_no = Regex.Match(value_got, @"name=""selectCardnos""><option value=""(\S+)"">").Groups[1].Value;

            // Then, go ahead.
            var post_value = new KeyValueDict
            {
                { "CardNo", card_no },
                { "Password", Password.ToBase64() },
                { "selectCardnos", card_no },
            };

            var reqMeta = new WebRequestMeta("SynCard/Manage/CardLost", WebRequestMeta.Json);
            reqMeta.SetHeader("Referer", baseUrl + "/SynCard/Manage/CardLost");
            var response = await WebClient.PostAsync(reqMeta, post_value);
            var cardLost = await response.ReadAsStringAsync();
            var Result = cardLost.ParseJSON<YktResult>();
            LastReport = Result.msg;
            return Result.success || LastReport == "Value cannot be null.\r\nParameter name: key";
        }

        public IWebClient WebClient { get; private set; }

        /// <summary>
        /// 上一个返回的错误
        /// </summary>
        public string LastReport { get; private set; }
    }
}
