﻿using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.Services;
using HandSchool.JLU.ViewModels;
using HandSchool.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HandSchool.Internal;

[assembly: RegisterService(typeof(SchoolCard))]
namespace HandSchool.JLU.Services
{
    /// <summary>
    /// 校园一卡通的执行服务。
    /// </summary>
    /// <inheritdoc cref="NotifyPropertyChanged" />
    /// <inheritdoc cref="ILoginField" />
    [Entrance("JLU", "校园一卡通", "提供校园卡的查询、充值、挂失等功能。")]

    //Android版本2.4.5.5以后，SchoolCard相关逻辑不再检查是否登录，此操作应在ViewModel中进行以保证窗口顺序正确
    public sealed class SchoolCard : NotifyPropertyChanged, ILoginField
    {
        private const string ServerName = "SchoolCard";
        string baseUrl = "http://dsf.jlu.edu.cn/";
        private string base8050Url = "http://dsf.jlu.edu.cn:8050/";

        #region Login Fields

        private bool _isLogin;
        private bool _autoLogin;
        private bool _savePassword;
        
        public bool IsWeb => false;

        public Task<TaskResp> BeforeLoginForm()
        {
            this.BindingVpnLoginState();
            return Task.FromResult(TaskResp.True);
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string CaptchaCode { get; set; }
        public byte[] CaptchaSource { get; set; }
        public TimeoutManager TimeoutManager { get; set; }
        public string Tips => "校园卡查询密码默认为身份证最后六位数字。";
        public string FormName => "校园卡服务中心";
        public bool NeedLogin => !_isLogin;
        public bool IsLogin { get => _isLogin; private set => SetProperty(ref _isLogin, value); }
        public bool AutoLogin { get => _autoLogin; set => SetProperty(ref _autoLogin, value); }
        public bool SavePassword { get => _savePassword; set => SetProperty(ref _savePassword, value); }
        public event EventHandler<LoginStateEventArgs> LoginStateChanged;

        public async Task<TaskResp> PrepareLogin()
        {
            try
            {
                await Logout();
                var loginStr = await WebClient.GetStringAsync("");
                var captchaUrl = Regex.Match(loginStr, @"id=""imgCheckCode"" src=""/(\S+)""");
                var codeUrl = (WebVpn.UseVpn ? "https://webvpn.jlu.edu.cn/"  : "http://dsf.jlu.edu.cn/") + captchaUrl.Groups[1].Value;
                var reqMeta = new WebRequestMeta(codeUrl, "image/gif");
                var captchaResp = await WebClient.GetAsync(reqMeta);
                CaptchaSource = await captchaResp.ReadAsByteArrayAsync();
                return new TaskResp(CaptchaSource != null);
            }
            catch (WebsException e)
            {
                return new TaskResp(false);
            }
        }

        public async Task Logout()
        {
            try
            {
                await WebClient.GetAsync($"{Loader.GetRealUrl(base8050Url)}Account/SignOff");
                IsLogin = false;
            }
            catch (WebsException ex)
            {
                Core.Logger.WriteException(ex);
            }
        }

        public async Task<TaskResp> Login()
        {
            if (Username == "" || Password == "")
            {
                return TaskResp.False;
            }

            Core.App.Loader.AccountManager.InsertOrUpdateTable(new UserAccount
            {
                ServerName = ServerName,
                UserName = Username,
                Password = SavePassword ? Password : string.Empty,
            });
                
            if (CaptchaCode.Equals(string.Empty))
            {
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "验证码不能为空！"));
                return TaskResp.False;
            }
            if (Username.Trim().Length != 11)
            {
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "用户名不对劲！"));
                return TaskResp.False;
            }
            var postValue = new KeyValueDict
            {
                { "signtype", "SynSno" },
                { "username", Username },
                { "password", Password },
                { "checkcode", CaptchaCode },
                { "isUsedKeyPad", "false" },
            };
            string error = null;
            try
            {
                var reqMeta = new WebRequestMeta("Account/MiniCheckIn", WebRequestMeta.All);
                reqMeta.SetHeader("Referer", baseUrl);
                var resp = await WebClient.PostAsync(reqMeta, postValue);
                error = await resp.ReadAsStringAsync();

                if (error.Contains("查询密码错误"))
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "密码错误！"));
                    return TaskResp.False;
                }
                if (error.Contains("验证码不正确"))
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "验证码错误！"));
                    return TaskResp.False;
                }
                var result = new YktResult { success = error == "success|False" };
                if (!result.success)
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "未知问题"));
                    return TaskResp.False;
                }

            }
            catch (WebsException ex)
            {
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(ex));
                return TaskResp.False;
            }
            catch (JsonException)
            {
                this.WriteLog("Unexpected value received <<<EOF\n" + error + "\nEOF;");
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "服务器响应有问题。"));
                IsLogin = false;
                return TaskResp.False;
            }

            IsLogin = true;
            LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Succeeded));
            return TaskResp.True;
        }

        public SchoolCard()
        {
            IsLogin = false;
            var acc = Core.App.Loader.AccountManager.GetItemWithPrimaryKey(ServerName);
            if (acc != null)
            {
                Username = acc.UserName;
                Password = acc.Password;
            }
            SavePassword = Password != "";
            TimeoutManager = new TimeoutManager(900);
            WebClient = Core.New<IWebClient>();
            WebClient.StringBaseAddress = baseUrl;
        }

        #endregion
        
        /// <exception cref="WebsException" />
        public async Task BasicInfoAsync()
        {
            var basicInfo = await WebClient.GetStringAsync("CardManage/CardInfo/BasicInfo");
            YktViewModel.Instance.ParseBasicInfo(basicInfo);
        }
        public async Task<bool> CheckLogin()
        {
            if (!IsLogin || TimeoutManager.IsTimeout())
            {
                if (IsLogin)
                {
                    await Logout();
                    IsLogin = false;
                }
            }
            else return true;
            if (await this.RequestLogin() == RequestLoginState.Success)
            {
                TimeoutManager.Refresh();
                return true;
            }
            else return false;
        }

        public async Task<TaskResp> PreChargeMoney()
        {
            try
            {
                var vpn = Loader.Vpn != null && Loader.Vpn.IsLogin;
                var valueGot = await WebClient.GetStringAsync("CardManage/CardInfo/Transfer");
                var captchaUrl = Regex.Match(valueGot, @"name=""img_transCheckCode"" src=""/(\S+)""");
                var reqUrl = (vpn ? "https://webvpn.jlu.edu.cn/" : "http://dsf.jlu.edu.cn/") +
                             captchaUrl.Groups[1].Value;
                var reqMeta = new WebRequestMeta(reqUrl, "image/gif");

                var reqMeta2 = new WebRequestMeta("Account/GetNumKeyPadImg", "image/jpeg");
                var captchaResp = await WebClient.GetAsync(reqMeta);
                var source = await captchaResp.ReadAsByteArrayAsync();
                captchaResp = await WebClient.GetAsync(reqMeta2);
                var keyboard = await captchaResp.ReadAsByteArrayAsync();
                return new TaskResp(true, (source, keyboard));
            }
            catch (Exception e)
            {
                Core.Logger.WriteException(e);
                return new TaskResp(false, e.Message);
            }

        }

        /// <exception cref="OverflowException" />
        /// <exception cref="FormatException" />
        /// <exception cref="WebsException" />
        /// <exception cref="JsonException" />
        public async Task<TaskResp> ChargeMoney(double money, string code, string keyboard)
        {
            if (money > 200 || money < 0.01)
                throw new OverflowException();
            var postValue = new KeyValueDict
            {
                { "password",Tools.EncodingPwd(Password,keyboard) },
                { "checkCode", code},
                { "amt",money.ToString("0.00")},
                { "fcard","bcard" },
                { "tocard","card" },
                { "bankno","" },
                { "bankpwd", "" }
            };

            var reqMeta = new WebRequestMeta("CardManage/CardInfo/TransferAccount", WebRequestMeta.All);
            reqMeta.SetHeader("Referer", WebClient.BaseAddress + "Backend/Management/Index");
            var response = await WebClient.PostAsync(reqMeta, postValue);
            var transferReport = await response.ReadAsStringAsync();

            if (transferReport.Contains("成功"))
                return TaskResp.True;
            try
            {
                transferReport = (JsonConvert.DeserializeObject<YktResult>(transferReport)?.msg) ?? transferReport;
            }
            catch
            {
                // ignored
            }

            return new TaskResp(false, transferReport);
        }
        /// <exception cref="WebsException" />
        public async Task QueryCost()
        {
            const string noHistory = "当前查询条件内没有流水记录";
            var infoLists = new List<List<RecordInfo>>();
            var todayUrl = "CardManage/CardInfo/TrjnList?type=0";
            var resToday = await WebClient.GetStringAsync(todayUrl);

            var now = DateTime.Now.AddDays(-1);
            var sevenDaysAgo = now.AddDays(-7);
            var url = $"CardManage/CardInfo/TrjnList?beginTime={Tools.DateFormatter(sevenDaysAgo)}&endTime={Tools.DateFormatter(now)}&type=1";
            var resultWeek = await WebClient.GetStringAsync(url);

            if (resToday.Contains(noHistory) && resultWeek.Contains(noHistory))
            {
                var _90DaysAgo = now.AddDays(-90);
                url = url.Replace(Tools.DateFormatter(sevenDaysAgo), Tools.DateFormatter(_90DaysAgo));
                resultWeek = await WebClient.GetStringAsync(url);
                if (resultWeek.Contains(noHistory)) return;
            }

            var cnt = 0;
            var today = Tools.AnalyzeHtmlToRecordInfos(resToday);
            if (today != null)
            {
                infoLists.Add(today);
                cnt += today.Count;
            }

            for (int i = 2; cnt < 50; i++)
            {
                var todayUrl2 = todayUrl + "&pageindex=" + i;
                var otherPage = await WebClient.GetStringAsync(todayUrl2);
                var otherList = Tools.AnalyzeHtmlToRecordInfos(otherPage);
                if (otherList != null)
                {
                    infoLists.Add(otherList);
                    cnt += otherList.Count;
                }
                else break;
            }

            var week = Tools.AnalyzeHtmlToRecordInfos(resultWeek);
            if (week != null)
            {
                infoLists.Add(week);
                cnt += week.Count;
            }

            for (int i = 2; cnt < 100; i++)
            {
                var weekUrl2 = url + "&pageindex=" + i;
                var otherPage = await WebClient.GetStringAsync(weekUrl2);
                var otherList = Tools.AnalyzeHtmlToRecordInfos(otherPage);
                if (otherList != null)
                {
                    infoLists.Add(otherList);
                    cnt += otherList.Count;
                }
                else break;
            }

            Core.Platform.EnsureOnMainThread(() =>
            {
                YktViewModel.Instance.RecordInfo.Clear();
                foreach (var item in infoLists.SelectMany(list => list))
                {
                    YktViewModel.Instance.RecordInfo.Add(item);
                }
            });
        }
        public async Task<TaskResp> PreSetLost()
        {
            // First, we should get our card number.
            var vpn = WebVpn.UseVpn;
            var valueGot = await WebClient.GetStringAsync("CardManage/CardInfo/LossCard");
            var captchaUrl = Regex.Match(valueGot, @"id=""imgCheckCode"" src=""/(\S+)""");
            var reqUrl = (vpn ? "https://webvpn.jlu.edu.cn/" : "http://dsf.jlu.edu.cn/") + captchaUrl.Groups[1].Value;
            var reqMeta = new WebRequestMeta(reqUrl, "image/gif");
            var reqMeta2 = new WebRequestMeta("Account/GetNumKeyPadImg", "image/jpeg");
            var captchaResp = await WebClient.GetAsync(reqMeta);
            var source = await captchaResp.ReadAsByteArrayAsync();
            captchaResp = await WebClient.GetAsync(reqMeta2);
            var keyboard = await captchaResp.ReadAsByteArrayAsync();
            return new TaskResp(true,(source, keyboard));
        }
        /// <exception cref="WebsException" />
        public async Task<TaskResp> SetLost(string code, string keyboard)
        {
            // Then, go ahead.
            var postValue = new KeyValueDict
            {
                { "checkCode", code },
                { "password", Tools.EncodingPwd(Password, keyboard.Replace(" ","")) },
            };

            var reqMeta = new WebRequestMeta("CardManage/CardInfo/SetCardLost", WebRequestMeta.All);
            reqMeta.SetHeader("Referer", $"{WebClient.BaseAddress}/Backend/Management/Index");
            var response = await WebClient.PostAsync(reqMeta, postValue);
            var cardLost = await response.ReadAsStringAsync();

            if (cardLost.Contains("挂失成功"))
                return TaskResp.True;

            return new TaskResp(false, cardLost);
        }

        public IWebClient WebClient { get; private set; }
    }
}
