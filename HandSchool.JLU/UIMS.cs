﻿using HandSchool.Internals;
using HandSchool.JLU;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Threading.Tasks;
using HandSchool.Internal;
using HandSchool.JLU.Services;

[assembly: RegisterService(typeof(UIMS))]
namespace HandSchool.JLU
{
    [Entrance("JLU", "吉林大学", "提供了与UIMS交互的接口。", EntranceType.SchoolEntrance)]
    [UseStorage("JLU")]
    sealed partial class UIMS : NotifyPropertyChanged, ISchoolSystem
    {
        private const string ServerName = "UIMS";
        const string UserCacheColName = "uims.user";
        const string TeachTermColName = "uims.teachingterm";

        private ISideSchoolStrategy UsingStrategy { get; set; }

        #region Login Information

        public string Username { get; set; }
        public string Password { get; set; }
        public bool NeedLogin { get; private set; }
        public TimeoutManager TimeoutManager { get; set; } = new TimeoutManager(600);
        
        public string Tips => "用户名为教学号，新生默认密码为身份证后六位（x小写）。";

        public string FormName => "UIMS教务管理系统";

        private string _proxyServer;
        //[Settings("服务器", "通过此域名访问UIMS，但具体路径地址不变。\n如果在JLU.NET等公用WiFi下访问，建议改为 10.60.65.8。")]
        public string ProxyServer
        {
            get => _proxyServer;
            set
            {
                var newVal = value.Trim();
                if (newVal.Length == 0) newVal = "uims.jlu.edu.cn";
                SetProperty(ref _proxyServer, newVal);
            }
        }
        public async Task<bool> CheckLogin()
        {
            if (!IsLogin || TimeoutManager.IsTimeout())
            {
                IsLogin = false;
            }
            else return true;
            if (await this.RequestLogin() == RequestLoginState.Success)
            {
                TimeoutManager.Refresh();
                return true;
            }
            else return false;
        }
        private bool _useHttps;
        //[Settings("使用SSL连接", "通过HTTPS连接UIMS，不验证证书。连接成功率更高。")]
        public bool UseHttps
        {
            get => _useHttps;
            set => SetProperty(ref _useHttps, value);
        }

        private bool _quickMode;
        //[Settings("快速连接模式", "通过10.60.65.8连接UIMS，可能不稳定，随时有接口被封锁的风险，但是快。如果需要抢课等场景，建议提前开启并测试接口是否正常。")]
        public bool QuickMode
        {
            get => _quickMode;
            set => SetProperty(ref _quickMode, value);
        }

        private bool _outsideSchool;
        //[Settings("我在校外", "若无法连接到学校校园网，勾选后可以登录公网教务系统进行成绩查询，其他大部分功能将被暂停使用。切换后需要重启本应用程序。")]
        public bool OutsideSchool
        {
            get => _outsideSchool;
            set => SetProperty(ref _outsideSchool, value);
        }

        public event EventHandler<LoginStateEventArgs> LoginStateChanged;

        private bool _isLogin;
        public bool IsLogin
        {
            get => _isLogin;
            private set
            {
                SetProperty(ref _isLogin, value);
                OnPropertyChanged(nameof(WelcomeMessage));
                OnPropertyChanged(nameof(CurrentMessage));
                OnPropertyChanged(nameof(CurrentWeek));
            }
        }

        private bool _autoLogin;
        public bool AutoLogin
        {
            get => _autoLogin;
            set => SetProperty(ref _autoLogin, false);
        }

        private bool _savePassword = true;
        public bool SavePassword
        {
            get => _savePassword;
            set
            {
                SetProperty(ref _savePassword, value);
                if (!value) SetProperty(ref _autoLogin, false, nameof(AutoLogin));
            }
        }

        public int TotalWeek { get; private set; }
        public string WelcomeMessage => UsingStrategy.WelcomeMessage;
        public string CurrentMessage => UsingStrategy.CurrentMessage;

        #endregion
        
        public IWebClient WebClient { get; set; }
        public string ServerUri => WebClient.StringBaseAddress;
        public string WeatherLocation => "101060101";
        
        public SchoolState SchoolState { get; set; }
        public bool IsWeb => false;
        public int CurrentWeek { get; set; }
        public string CaptchaCode { get; set; } = "";
        public byte[] CaptchaSource { get; set; }

        /// <summary>
        /// 建立访问UIMS的对象。
        /// </summary>
        /// <param name="config">设置属性。</param>
        /// <param name="injectedHandler">事件处理传递方法。</param>
        public UIMS(Loader.SettingsJSON config, EventHandler<LoginStateEventArgs> injectedHandler = null)
        {
            if (injectedHandler != null)
            {
                LoginStateChanged += injectedHandler;
            }

            ProxyServer = "uims.jlu.edu.cn";
            UseHttps = true;
            OutsideSchool = false; // config.OutsideSchool;
            QuickMode = false; //config.QuickMode;

            IsLogin = false;
            NeedLogin = true;
            var acc = Core.App.Loader.AccountManager.GetItemWithPrimaryKey(ServerName);
            if (acc != null)
            {
                Username = acc.UserName;
                Password = acc.Password;
            }
            if (Password == "") SavePassword = false;
            UsingStrategy = new DefaultSchoolStrategy(this);
            UsingStrategy.OnLoad();
        }

        public async Task<TaskResp> Login()
        {
            if (Username == "" || Password == "")
            {
                NeedLogin = true;
                return TaskResp.False;
            }
            else
            {
                Core.App.Loader.AccountManager.InsertOrUpdateTable(new UserAccount
                {
                    ServerName = ServerName,
                    UserName = Username,
                    Password = SavePassword ? Password : string.Empty
                });
            }

            return await UsingStrategy.LoginSide();
        }

        public async Task Logout()
        {
            await UsingStrategy.LogoutSide();
        }
        
        public string FormatArguments(string args)
        {
            return UsingStrategy.FormatArguments(args);
        }
        
        public async Task<string> Post(string url, string send)
        {
            if (!await this.CheckLogin())
            {
                throw new WebsException("登录失败。", WebStatus.Timeout);
            }

            try
            {
                var reqMeta = new WebRequestMeta(url, WebRequestMeta.Json);
                var response = await WebClient.PostAsync(reqMeta, FormatArguments(send), WebRequestMeta.Json);

                if (response.Location.Contains(UsingStrategy.TimeoutUrl))
                {
                    throw new WebsException("登录超时。", WebStatus.Timeout);
                }

                return await response.ReadAsStringAsync();
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.Timeout)
                    _isLogin = false;
                throw;
            }
        }

        public async Task<string> Get(string url)
        {
            if (!await this.CheckLogin())
            {
                throw new WebsException("登录超时。", WebStatus.Timeout);
            }

            try
            {
                var ret = await WebClient.GetAsync(url, WebRequestMeta.Json);

                if (ret.Location.Contains(UsingStrategy.TimeoutUrl))
                {
                    throw new WebsException("登录超时。", WebStatus.Timeout);
                }

                return await ret.ReadAsStringAsync();
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.Timeout)
                    _isLogin = false;
                throw;
            }
        }

        public void SaveSettings() => Core.App.Loader.SaveSettings(this);
        
        public void ResetSettings() { }

        public Task<TaskResp> PrepareLogin()
        {
            return UsingStrategy.PrepareLogin();
        }

        public Task<TaskResp> BeforeLoginForm()
        {
            this.BindingVpnLoginState();
            return UsingStrategy.BeforeLoginForm();
        }

        interface ISideSchoolStrategy
        {
            string TimeoutUrl { get; }
            Task<TaskResp> LoginSide();
            Task LogoutSide();
            void OnLoad();
            string FormatArguments(string input);
            string WelcomeMessage { get; }
            string CurrentMessage { get; }
            Task<TaskResp> PrepareLogin();
            Task<TaskResp> BeforeLoginForm();
        }
    }
}