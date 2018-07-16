using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using static HandSchool.Internal.Helper;

namespace HandSchool.Blank
{
    class BlankSchool : NotifyPropertyChanged, ISchoolSystem
    {
        private string feedUrl = "";
        private string weatherLoc = "长春";

        public BlankSchool()
        {
            var lp = Core.ReadConfig("blank.config.json");
            SettingsJSON config;
            if (lp != "") config = JSON<SettingsJSON>(lp);
            else config = new SettingsJSON();
            DailyClassCount = config.DailyClassCount;
            FeedUrl = config.FeedUri;
            WeatherLocation = config.WeatherLocation;
        }

        public string ServerUri => "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public bool IsLogin => true;
        public bool NeedLogin => false;
        public int CurrentWeek { get; set; } = 1;

        [Settings("提示", "保存使设置永久生效，部分设置重启后生效。", -233)]
        public string Tips => "";

        public bool AutoLogin { get; set; } = true;
        public bool SavePassword { get; set; } = true;
        public string WelcomeMessage => "欢迎。";
        public string CurrentMessage => "";

        [Settings("每日课程数量", "每日有多少节课，配合课程表使用。", 1, 15)]
        public int DailyClassCount
        {
            get => Core.App.DailyClassCount;
            set => SetProperty(ref Core.App.DailyClassCount, value);
        }

        [Settings("消息通知地址", "消息通知的RSS地址。")]
        public string FeedUrl
        {
            get => feedUrl;
            set => SetProperty(ref feedUrl, value);
        }

        [Settings("天气位置", "首页天气显示的位置。")]
        public string WeatherLocation
        {
            get => weatherLoc;
            set => SetProperty(ref weatherLoc, value);
        }

        public AwaredWebClient WebClient { get; set; }
        public NameValueCollection AttachInfomation { get; set; } = new NameValueCollection();
        public event EventHandler<LoginStateEventArgs> LoginStateChanged;

        public async Task<string> Get(string url)
        {
            await Task.Run(() => System.Diagnostics.Debug.WriteLine("Blank->Get(url)"));
            return "";
        }

        public async Task<bool> Login()
        {
            await Task.Run(() => System.Diagnostics.Debug.WriteLine("Blank->Login()"));
            LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Succeeded));
            return true;
        }

        public async Task<string> Post(string url, string send)
        {
            await Task.Run(() => System.Diagnostics.Debug.WriteLine("Blank->Post(url, send)"));
            return "";
        }

        public async Task<bool> RequestLogin()
        {
            await Task.Run(() => System.Diagnostics.Debug.WriteLine("Blank->RequestLogin()"));
            return false;
        }

        public void SaveSettings()
        {
            var save = Serialize(new SettingsJSON { DailyClassCount = Core.App.DailyClassCount, FeedUri = feedUrl, WeatherLocation = weatherLoc });
            Core.WriteConfig("blank.config.json", save);
        }

        class SettingsJSON
        {
            public int DailyClassCount { get; set; } = 10;
            public string FeedUri { get; set; } = "";
            public string WeatherLocation { get; set; } = "长春";
        }

        public string FormatArguments(string args)
        {
            return args;
        }
    }
}
