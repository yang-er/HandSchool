using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace HandSchool.JLU
{
    partial class UIMS : NotifyPropertyChanged, ISchoolSystem
    {
        internal const string config_file = "jlu.config.json";
        internal const string config_username = "jlu.uims.username.txt";
        internal const string config_password = "jlu.uims.password.txt";
        internal const string config_usercache = "jlu.user.json";
        internal const string config_teachterm = "jlu.teachingterm.json";
        private ISideSchoolStrategy UsingStrategy { get; set; }

        #region Login Information

        public string Username { get; set; }
        public string Password { get; set; }
        public bool NeedLogin { get; private set; }
        
        [Settings("提示", "保存使设置永久生效，部分设置重启后生效。", -233)]
        public string Tips { get; private set; } = "用户名为教学号，新生默认密码为身份证后六位（x小写）。";
        
        public string FormName { get; private set; } = "UIMS教务管理系统";

        private string proxy_server;
        [Settings("服务器", "通过此域名访问UIMS，但具体路径地址不变。\n如果在JLU.NET等公用WiFi下访问，建议改为 10.60.65.8。")]
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
        [Settings("使用SSL连接", "通过HTTPS连接UIMS，不验证证书。连接成功率更高。")]
        public bool UseHttps
        {
            get => use_https;
            set => SetProperty(ref use_https, value);
        }

        private bool outside_school;
        [Settings("我在校外", "若无法连接到学校校园网，勾选后可以登录公网教务系统进行成绩查询，其他大部分功能将被暂停使用。切换后需要重启本应用程序。")]
        public bool OutsideSchool
        {
            get => outside_school;
            set => SetProperty(ref outside_school, value);
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

        private bool auto_login = true;
        public bool AutoLogin
        {
            get => auto_login;
            set
            {
                SetProperty(ref auto_login, value);
                if (value) SetProperty(ref save_password, true, nameof(SavePassword));
            }
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
        
        public AwaredWebClient WebClient { get; set; }
        public NameValueCollection AttachInfomation { get; set; }
        public string ServerUri => $"http{(use_https ? "s" : "")}://{proxy_server}/ntms/";
        public string WeatherLocation => "101060101";
        public int CurrentWeek { get; set; }
        public string CaptchaCode { get; set; } = "";
        public byte[] CaptchaSource { get; set; } = null;

        /// <summary>
        /// 建立访问UIMS的对象。
        /// </summary>
        /// <param name="injectedHandler">事件处理传递方法。</param>
        public UIMS(EventHandler<LoginStateEventArgs> injectedHandler = null)
        {
            if (injectedHandler != null)
            {
                LoginStateChanged += injectedHandler;
            }

            var lp = Core.ReadConfig(config_file);
            SettingsJSON config;
            if (lp != "") config = lp.ParseJSON<SettingsJSON>();
            else config = new SettingsJSON();
            ProxyServer = config.ProxyServer;
            UseHttps = config.UseHttps;
            OutsideSchool = config.OutsideSchool;
            
            IsLogin = false;
            NeedLogin = false;
            Username = Core.ReadConfig(config_username);
            AttachInfomation = new NameValueCollection();
            if (Username != "") Password = Core.ReadConfig(config_password);
            if (Password == "") SavePassword = false;

            if (OutsideSchool) UsingStrategy = new OutsideSchoolStrategy(this);
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
                Core.WriteConfig(config_username, Username);
                Core.WriteConfig(config_password, SavePassword ? Password : "");
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
                throw new WebException("登录超时。", WebExceptionStatus.Timeout);
            }

            try
            {
                var ret = await WebClient.PostAsync(url, FormatArguments(send));

                if (WebClient.Location == UsingStrategy.TimeoutUrl)
                {
                    throw new WebException("登录超时。", WebExceptionStatus.Timeout);
                }

                return ret;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                    is_login = false;
                throw ex;
            }
        }

        public async Task<string> Get(string url)
        {
            if (await this.RequestLogin() == false)
            {
                throw new WebException("登录超时。", WebExceptionStatus.Timeout);
            }

            try
            {
                var ret = await WebClient.GetAsync(url);

                if (WebClient.Location == UsingStrategy.TimeoutUrl)
                {
                    throw new WebException("登录超时。", WebExceptionStatus.Timeout);
                }

                return ret;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                    is_login = false;
                throw ex;
            }
        }

        public void SaveSettings()
        {
            var save = new SettingsJSON
            {
                ProxyServer = proxy_server,
                UseHttps = use_https,
                OutsideSchool = outside_school
            }.Serialize();

            Core.WriteConfig(config_file, save);
        }

        [Settings("清除数据", "将应用数据清空，恢复到默认状态。")]
        public async void ResetSettings(IViewResponse resp)
        {
            if (!await resp.ShowAskMessage("清除数据", "确定要清除数据吗？", "取消", "确认")) return;
            Core.WriteConfig(config_username, "");
            Core.WriteConfig(config_password, "");
            Core.WriteConfig(config_usercache, "");
            Core.WriteConfig(config_username, "");
            Core.WriteConfig(config_file, "");
            Core.WriteConfig(OA.config_oa, "");
            Core.WriteConfig(OA.config_oa_time, "");
            Core.WriteConfig(Schedule.config_kcb, "");
            Core.WriteConfig(Schedule.config_kcb_orig, "");
            Core.WriteConfig(SchoolCard.config_username, "");
            Core.WriteConfig(SchoolCard.config_password, "");
            Core.WriteConfig(SchoolCard.config_school, "");
            Core.WriteConfig(MessageEntrance.config_msgbox, "");
            Core.WriteConfig(GradeEntrance.config_gpa, "");
            Core.WriteConfig(GradeEntrance.config_grade, "");
            Core.WriteConfig(InfoQuery.LibraryRent.conf_username, "");
            Core.WriteConfig(InfoQuery.LibraryRent.conf_password, "");
            Core.WriteConfig("hs.school.bin", "");
            await resp.ShowMessage("清除数据", "重置应用成功！重启应用后生效。");
        }

        public async Task<bool> PrepareLogin()
        {
            await Task.Run(() => { });
            return true;
        }

        class SettingsJSON
        {
            public string ProxyServer { get; set; } = "10.60.65.8"; // uims.jlu.edu.cn
            public bool UseHttps { get; set; } = true;
            public bool OutsideSchool { get; set; } = false;
        }
    }
}
