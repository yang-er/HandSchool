using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Blank
{
    /// <summary>
    /// 默认的空白大学，提供了一些基本操作。
    /// </summary>
    [Entrance("任意大学", type: EntranceType.SchoolEntrance)]
    class BlankSchool : NotifyPropertyChanged, ISchoolSystem
    {
        const string config_file = "blank.config.json";

        private string feedUrl = "";
        private string weatherLoc = "101060101";
        public string FormName => "";

        public BlankSchool()
        {
            var lp = Core.ReadConfig(config_file);
            SettingsJSON config;
            if (lp != "") config = lp.ParseJSON<SettingsJSON>();
            else config = new SettingsJSON();
            DailyClassCount = config.DailyClassCount;
            FeedUrl = config.FeedUri;
            WeatherLocation = config.WeatherLocation;
        }

        public string ServerUri => "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string CaptchaCode { get; set; } = "";
        public byte[] CaptchaSource { get; set; } = null;
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

        [Settings("天气位置", "首页天气显示的位置，参考SOJSON的地址。")]
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
            Core.Log("Blank->Get(url)");
            await Task.Run(() => { });
            return "";
        }

        public async Task<bool> Login()
        {
            Core.Log("Blank->Login()");
            await Task.Run(() => { });
            LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Succeeded));
            return true;
        }

        public async Task<string> Post(string url, string send)
        {
            Core.Log("Blank->Post(url, send)");
            await Task.Run(() => { });
            return "";
        }

        public async Task<bool> RequestLogin()
        {
            Core.Log("Blank->RequestLogin()");
            await Task.Run(() => { });
            return false;
        }

        public void SaveSettings()
        {
            var save = new SettingsJSON
            {
                DailyClassCount = Core.App.DailyClassCount,
                FeedUri = feedUrl,
                WeatherLocation = weatherLoc
            }.Serialize();

            Core.WriteConfig(config_file, save);
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

        public async Task<bool> PrepareLogin()
        {
            await Task.Run(() => { });
            return true;
        }

        [Settings("清除数据", "将应用数据清空，恢复到默认状态。")]
        public async void ResetSettings(IViewResponse resp)
        {
            if (!await resp.ShowAskMessage("清除数据", "确定要清除数据吗？", "取消", "确认")) return;
            Core.WriteConfig(config_file, "");
            Core.WriteConfig("hs.school.bin", "");
            await resp.ShowMessage("重置应用", "重置应用成功！重启应用后生效。");
        }
    }
}
