using HandSchool.Design;
using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Threading.Tasks;

[assembly: RegisterService(typeof(SchoolBase))]
namespace HandSchool.JLU.Services
{
    [UseStorage("JLU", configPassword, configUsername)]
    internal abstract class SchoolBase : NotifyPropertyChanged, ISchoolSystem
    {
        const string configUsername = "jlu.uims.username.txt";
        const string configPassword = "jlu.uims.password.txt";
        
        private string proxy_server;
        private bool use_https;
        private bool outside_school;
        private bool is_login = false;
        private bool auto_login = true;
        private bool save_password = true;
        protected IConfiguration Configure { get; }

        public string Username { get; set; }
        public string Password { get; set; }
        public bool NeedLogin { get; protected set; }
        public string FormName => "UIMS教务管理系统";

        public IWebClient WebClient { get; set; }
        public string ServerUri => $"http{(use_https ? "s" : "")}://{proxy_server}/ntms/";
        public string WeatherLocation => "101060101";
        public int CurrentWeek { get; set; }
        public string CaptchaCode { get; set; } = "";
        public byte[] CaptchaSource { get; set; } = null;

        protected SchoolBase(IConfiguration config, IWebClient wc)
        {
            Configure = config;
            WebClient = wc;
        }

        [Settings("提示", "保存使设置永久生效，部分设置重启后生效。")]
        public string Tips => "用户名为教学号，新生默认密码为身份证后六位（x小写）。";
        
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

        [Settings("使用SSL连接", "通过HTTPS连接UIMS，不验证证书。连接成功率更高。")]
        public bool UseHttps
        {
            get => use_https;
            set => SetProperty(ref use_https, value);
        }
        
        [Settings("我在校外", "若无法连接到学校校园网，勾选后可以登录公网教务系统进行成绩查询，其他大部分功能将被暂停使用。切换后需要重启本应用程序。")]
        public bool OutsideSchool
        {
            get => outside_school;
            set => SetProperty(ref outside_school, value);
        }

        public event EventHandler<LoginStateEventArgs> LoginStateChanged;

        protected void SendLoginStateChanged(LoginStateEventArgs args)
        {
            LoginStateChanged?.Invoke(this, args);
        }

        public bool IsLogin
        {
            get => is_login;
            protected set
            {
                SetProperty(ref is_login, value);
                OnPropertyChanged(nameof(WelcomeMessage));
                OnPropertyChanged(nameof(CurrentMessage));
                OnPropertyChanged(nameof(CurrentWeek));
            }
        }

        public bool AutoLogin
        {
            get => auto_login;
            set
            {
                SetProperty(ref auto_login, value);
                if (value) SetProperty(ref save_password, true, nameof(SavePassword));
            }
        }

        public bool SavePassword
        {
            get => save_password;
            set
            {
                SetProperty(ref save_password, value);
                if (!value) SetProperty(ref auto_login, false, nameof(AutoLogin));
            }
        }

        public abstract string WelcomeMessage { get; }
        public abstract string CurrentMessage { get; }
        public abstract string TimeoutUrl { get; }
        public abstract string FormatArguments(string args);
        public abstract Task<bool> LoginSide();

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

                if (response.Location == TimeoutUrl)
                {
                    throw new WebsException("登录超时。", WebStatus.Timeout);
                }

                return await response.ReadAsStringAsync();
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.Timeout)
                    is_login = false;
                throw;
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

                if (ret.Location == TimeoutUrl)
                {
                    throw new WebsException("登录超时。", WebStatus.Timeout);
                }

                return await ret.ReadAsStringAsync();
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.Timeout)
                    is_login = false;
                throw;
            }
        }

        public Task<bool> PrepareLogin()
        {
            return Task.FromResult(true);
        }
        
        public void ResetSettings() { }

        public async Task<bool> Login()
        {
            if (Username == "" || Password == "")
            {
                NeedLogin = true;
                return false;
            }
            else
            {
                await Configure.SaveAsync(configUsername, Username);
                await Configure.SaveAsync(configPassword, SavePassword ? Password : "");
            }

            return await LoginSide();
        }
    }
}
