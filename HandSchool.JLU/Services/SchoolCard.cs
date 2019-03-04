using HandSchool.Design;
using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.Services;
using HandSchool.Models;
using HandSchool.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        bool is_login = false;

        private IConfigureProvider Configure { get; }
        private ILogger<SchoolCard> Logger { get; }
        public IWebClient WebClient { get; }
        
        public string Username { get; set; }
        public string Password { get; set; }
        public string CaptchaCode { get; set; }
        public byte[] CaptchaSource { get; set; }

        public string Tips => "校园卡查询密码默认为身份证最后六位数字。";
        public string FormName => "校园卡服务中心";
        public bool NeedLogin => !is_login;
        public bool AutoLogin { get; set; }
        public bool SavePassword { get; set; }
        public event EventHandler<LoginStateEventArgs> LoginStateChanged;
        public string LastReport { get; private set; }

        public bool IsLogin
        {
            get => is_login;
            private set => SetProperty(ref is_login, value);
        }

        public SchoolCard(IConfigureProvider configure, IWebClient webClient, ILogger<SchoolCard> logger)
        {
            Configure = configure;
            WebClient = webClient;
            WebClient.BaseAddress = baseUrl;
            Logger = logger;

            IsLogin = false;
            Username = Configure.Read(configUsername);
            if (!string.IsNullOrEmpty(Username))
                Password = Configure.Read(configPassword);
            SavePassword = !string.IsNullOrEmpty(Password);
        }
        
        /// <summary>
        /// 登录之前进行准备工作，例如加载验证码等内容。
        /// </summary>
        /// <returns>是否准备成功。</returns>
        public async Task<bool> PrepareLogin()
        {
            try
            {
                WebClient.ResetClient();
                await WebClient.GetAsync("");
                var loginStr = await WebClient.GetStringAsync("Account/Login");
                var captchaUrl = Regex.Match(loginStr, @"id=""imgCheckCode"" src=""/(\S+)""");

                var reqMeta = new WebRequestMeta(captchaUrl.Groups[1].Value, "image/jif");
                var captchaResp = await WebClient.GetAsync(reqMeta);
                CaptchaSource = await captchaResp.ReadAsByteArrayAsync();
                return CaptchaSource != null;
            }
            catch (WebsException)
            {
                return false;
            }
        }

        /// <summary>
        /// 登录操作，通过网络进行登录操作。
        /// </summary>
        /// <returns>是否登录成功。</returns>
        public async Task<bool> Login()
        {
            if (Username == "" || Password == "") return false;
            await Configure.SaveAsync(configUsername, Username);
            await Configure.SaveAsync(configPassword, SavePassword ? Password : "");

            string lastReport = "";
            var postValue = new KeyValueDict
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
                var resp = await WebClient.PostAsync(reqMeta, postValue);
                lastReport = await resp.ReadAsStringAsync();

                if (lastReport == "被拒绝") throw new ServiceException("软件版本过老，无法登录。");
                var result = lastReport.ParseJSON<YktResult>();
                if (!result.success) throw new ServiceException(result.msg);
            }
            catch (WebsException ex)
            {
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(ex));
                return IsLogin = false;
            }
            catch (JsonException)
            {
                Logger.Warn("Unexpected value received <<<EOF\n" + lastReport + "\nEOF;");
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "服务器响应有问题。"));
                return IsLogin = false;
            }
            catch (ServiceException ex)
            {
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, ex.Message));
                return false;
            }
            
            IsLogin = true;
            LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Succeeded));
            return true;
        }
        
        /// <summary>
        /// 将HTML字符串规格化。
        /// </summary>
        /// <param name="input">输入串</param>
        /// <returns>输出串</returns>
        static string RegularizeHtml(string input)
        {
            return input.Replace("    ", "").Replace("\r", "").Replace("\n", "");
        }

        /// <summary>
        /// 更新基本信息。
        /// </summary>
        /// <param name="basicInfo">基本信息</param>
        /// <exception cref="WebsException" />
        /// <returns>基本信息的集合</returns>
        public async Task BasicInfoAsync(CardBasicInfo basicInfo)
        {
            if (!await this.RequestLogin()) return;
            var resultBasicInfo = await WebClient.GetStringAsync("SynCard/Manage/BasicInfo", "text/html");
            ParseBasicInfo(basicInfo, resultBasicInfo);
        }

        /// <summary>
        /// 解析基础信息
        /// </summary>
        /// <param name="basicInfo">基础信息</param>
        /// <param name="html">需要解析的网页</param>
        private static void ParseBasicInfo(CardBasicInfo basicInfo, string html)
        {
            html = RegularizeHtml(html);

            foreach (var info in basicInfo)
            {
                if (info.Command != null) continue;
                info.Description = Regex.Match(html, @"<td class=""first"">" + info.Title + @"</td><td class=""second"">(\S+)</td>").Groups[1].Value;
            }
        }

        /// <summary>
        /// 获取拾卡信息。
        /// </summary>
        /// <exception cref="WebsException" />
        /// <returns>拾卡信息的集合</returns>
        public async Task<IEnumerable<PickCardInfo>> GetPickCardInfo()
        {
            if (!await this.RequestLogin()) return new PickCardInfo[0];
            
            var cardList = await WebClient.GetStringAsync("InfoPub/CardNotice/NFixCardList");
            var parsedHtml = Regex.Match(RegularizeHtml(cardList), parsePattern).Value;
            return PickCardInfo.EnumerateFromHtml("<Root><div><table>" + parsedHtml + "</table></div></Root>");
        }

        /// <summary>
        /// 充值钱币。
        /// </summary>
        /// <param name="money">充值的金额</param>
        /// <exception cref="OverflowException" />
        /// <exception cref="FormatException" />
        /// <exception cref="WebsException" />
        /// <exception cref="JsonException" />
        /// <returns>是否充值成功</returns>
        public async Task<bool> ChargeMoney(string money)
        {
            if (!await this.RequestLogin())
            {
                LastReport = "用户取消了转账。";
                return false;
            }

            var trueMoney = double.Parse(money);
            if (trueMoney > 200 || trueMoney <= 0)
                throw new OverflowException();

            var postValue = new KeyValueDict
            {
                { "FromCard", "bcard" },
                { "ToCard", "card" },
                { "Amount", trueMoney.ToString("f2")},
                { "Password", Password.ToBase64() }
            };

            var reqMeta = new WebRequestMeta("SynCard/Manage/TransferPost", WebRequestMeta.Json);
            reqMeta.SetHeader("Referer", baseUrl + "/SynCard/Manage/Transfer");
            var response = await WebClient.PostAsync(reqMeta, postValue);
            var transferReport = await response.ReadAsStringAsync();
            var result = transferReport.ParseJSON<YktResult>();

            LastReport = result.msg;
            return result.success;
        }
        
        /// <summary>
        /// 查询流水记录。
        /// </summary>
        /// <exception cref="WebsException" />
        /// <returns>流水记录的集合</returns>
        public async Task<IEnumerable<RecordInfo>> QueryCost()
        {
            if (!await this.RequestLogin()) return new RecordInfo[0];
            const string noHistory = "当前查询条件内没有流水记录";

            var resultWeek = await WebClient.GetStringAsync("SynCard/Manage/OneWeekTrjn");
            var resultDay = await WebClient.GetStringAsync("SynCard/Manage/CurrentDayTrjn");

            if (resultDay.Contains(noHistory) && resultWeek.Contains(noHistory))
            {
                resultWeek = await WebClient.GetStringAsync("SynCard/Manage/TrjnHistory");
                if (resultWeek.Contains(noHistory)) return new RecordInfo[0];
            }

            var parseWeek = Regex.Match(RegularizeHtml(resultWeek), parsePattern).Value;
            var parseToday = Regex.Match(RegularizeHtml(resultDay), parsePattern).Value;
            return RecordInfo.EnumerateFromHtml("<Root><div><table>" + parseToday + parseWeek + "</table></div></Root>");
        }

        /// <summary>
        /// 标记卡为挂失状态。
        /// </summary>
        /// <exception cref="WebsException" />
        /// <returns>挂失是否成功</returns>
        public async Task<bool> SetLost()
        {
            if (!await this.RequestLogin())
            {
                LastReport = "用户取消了挂失。";
                return false;
            }

            // First, we should get our card number.
            var valueGot = await WebClient.GetStringAsync("SynCard/Manage/CardLost");
            var cardNo = Regex.Match(valueGot, @"name=""selectCardnos""><option value=""(\S+)"">").Groups[1].Value;

            // Then, go ahead.
            var postValue = new KeyValueDict
            {
                { "CardNo", cardNo },
                { "Password", Password.ToBase64() },
                { "selectCardnos", cardNo },
            };

            var reqMeta = new WebRequestMeta("SynCard/Manage/CardLost", WebRequestMeta.Json);
            reqMeta.SetHeader("Referer", baseUrl + "/SynCard/Manage/CardLost");
            var response = await WebClient.PostAsync(reqMeta, postValue);
            var cardLost = await response.ReadAsStringAsync();
            var result = cardLost.ParseJSON<YktResult>();
            LastReport = result.msg;
            return result.success || LastReport == "Value cannot be null.\r\nParameter name: key";
        }
    }
}
