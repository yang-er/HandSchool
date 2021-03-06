﻿using HandSchool.Internals;
using HandSchool.JLU;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.Views;
using System;
using System.Threading.Tasks;

[assembly: RegisterService(typeof(UIMS))]
namespace HandSchool.JLU
{
    [Entrance("JLU", "吉林大学", "提供了与UIMS交互的接口。", EntranceType.SchoolEntrance)]
    [UseStorage("JLU", configUsername, configPassword, configUserCache, configTeachTerm)]
    sealed partial class UIMS : NotifyPropertyChanged, ISchoolSystem
    {
        const string configUsername = "jlu.uims.username.txt";
        const string configPassword = "jlu.uims.password.txt";
        const string configUserCache = "jlu.user.json";
        const string configTeachTerm = "jlu.teachingterm.json";

        private ISideSchoolStrategy UsingStrategy { get; set; }

        #region Login Information

        public string Username { get; set; }
        public string Password { get; set; }
        public bool NeedLogin { get; private set; }
        
        [Settings("提示", "保存使设置永久生效，部分设置重启后生效。")]
        public string Tips => "用户名为教学号，新生默认密码为身份证后六位（x小写）。";
        
        public string FormName => "UIMS教务管理系统";

        private string proxy_server;
        //[Settings("服务器", "通过此域名访问UIMS，但具体路径地址不变。\n如果在JLU.NET等公用WiFi下访问，建议改为 10.60.65.8。")]
        public string ProxyServer
        {
            get => proxy_server;
            set
            {
                var new_val = value.Trim();
                if (new_val.Length == 0) new_val = "uims.jlu.edu.cn";
                SetProperty(ref proxy_server, new_val);
            }
        }

        private bool use_https;
        //[Settings("使用SSL连接", "通过HTTPS连接UIMS，不验证证书。连接成功率更高。")]
        public bool UseHttps
        {
            get => use_https;
            set => SetProperty(ref use_https, value);
        }

        private bool quick_mode;
        //[Settings("快速连接模式", "通过10.60.65.8连接UIMS，可能不稳定，随时有接口被封锁的风险，但是快。如果需要抢课等场景，建议提前开启并测试接口是否正常。")]
        public bool QuickMode
        {
            get => quick_mode;
            set => SetProperty(ref quick_mode, value);
        }

        private bool outside_school;
        //[Settings("我在校外", "若无法连接到学校校园网，勾选后可以登录公网教务系统进行成绩查询，其他大部分功能将被暂停使用。切换后需要重启本应用程序。")]
        public bool OutsideSchool
        {
            get => outside_school;
            set => SetProperty(ref outside_school, value);
        }

        private bool use_vpn;
        [Settings("使用学生VPN", "使用学生VPN连接教务系统，不稳定，建议在内网时不使用此选项。切换后需要重启本应用程序。")]
        public bool UseVpn
        {
            get => use_vpn;
            set => SetProperty(ref use_vpn, value);
        }

        public event EventHandler<LoginStateEventArgs> LoginStateChanged;

        private bool is_login = false;
        public bool IsLogin
        {
            get => is_login;
            private set
            {
                SetProperty(ref is_login, value);
                OnPropertyChanged(nameof(WelcomeMessage));
                OnPropertyChanged(nameof(CurrentMessage));
                OnPropertyChanged(nameof(CurrentWeek));
            }
        }

        private bool auto_login = false;
        public bool AutoLogin
        {
            get => auto_login;
            set => SetProperty(ref auto_login, false);
        }

        private bool save_password = true;
        public bool SavePassword
        {
            get => save_password;
            set
            {
                SetProperty(ref save_password, value);
                if (!value) SetProperty(ref auto_login, false, nameof(AutoLogin));
            }
        }

        public string WelcomeMessage => UsingStrategy.WelcomeMessage;
        public string CurrentMessage => UsingStrategy.CurrentMessage;

        #endregion
        
        public IWebClient WebClient { get; set; }
        public string ServerUri => $"http{(use_https ? "s" : "")}://{proxy_server}/ntms/";
        public string WeatherLocation => "101060101";
        public int CurrentWeek { get; set; }
        public string CaptchaCode { get; set; } = "";
        public byte[] CaptchaSource { get; set; } = null;

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
            UseVpn = config.UseVpn;

            IsLogin = false;
            NeedLogin = true;
            Username = Core.Configure.Read(configUsername);
            if (Username != "") Password = Core.Configure.Read(configPassword);
            if (Password == "") SavePassword = false;

            if (OutsideSchool) UsingStrategy = new OutsideSchoolStrategy(this);
            else if (UseVpn) UsingStrategy = new VpnSchoolStrategy(this);
            else UsingStrategy = new InsideSchoolStrategy(this);
            UsingStrategy.OnLoad();
        }

        public async Task<bool> Login()
        {
            if (Username == "" || Password == "")
            {
                NeedLogin = true;
                return false;
            }
            else
            {
                Core.Configure.Write(configUsername, Username);
                Core.Configure.Write(configPassword, SavePassword ? Password : "");
            }

            return await UsingStrategy.LoginSide();
        }
        
        public string FormatArguments(string args)
        {
            return UsingStrategy.FormatArguments(args);
        }
        
        public async Task<string> Post(string url, string send)
        {
            if (await this.RequestLogin() == false)
            {
                throw new WebsException("登录超时。", WebStatus.Timeout);
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
                    is_login = false;
                throw ex;
            }
        }

        public async Task<string> Get(string url)
        {
            if (await this.RequestLogin() == false)
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
                    is_login = false;
                throw ex;
            }
        }

        public void SaveSettings() => Core.App.Loader.SaveSettings(this);
        
        public void ResetSettings() { }

        public Task<bool> PrepareLogin()
        {
            return UsingStrategy.PrepareLogin();
        }

        public Task<bool> BeforeLoginForm()
        {
            if (UseVpn)
                return UsingStrategy.PrepareLogin();
            return Task.FromResult(true);
        }

        interface ISideSchoolStrategy
        {
            string TimeoutUrl { get; }
            Task<bool> LoginSide();
            void OnLoad();
            string FormatArguments(string input);
            string WelcomeMessage { get; }
            string CurrentMessage { get; }
            Task<bool> PrepareLogin();
        }
    }
}