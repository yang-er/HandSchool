using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using static HandSchool.Internal.Helper;

namespace HandSchool.Blank
{
    class Loader : ISchoolWrapper
    {
        public string SchoolName => "任意大学";
        public string SchoolId => "blank";

        public void PostLoad() { }

        public void PreLoad()
        {
            var sch = new BlankSchool();
            Core.App.Service = sch;
            Core.App.Schedule = new Schedule();
            if (sch.FeedUrl != "") Core.App.Feed = new FeedEntrance(sch.FeedUrl);
        }

        public override string ToString()
        {
            return SchoolName;
        }
    }

    class BlankSchool : NotifyPropertyChanged, ISchoolSystem
    {
        private string feedUrl = "";
        private string weatherLoc = "长春";

        public BlankSchool()
        {
            var lp = ReadConfFile("blank.config.json");
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
            WriteConfFile("blank.config.json", save);
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

    class Schedule : IScheduleEntrance
    {
        public List<CurriculumItem> Items { get; private set; }
        public string LastReport { get; private set; } = "";

        public int ClassNext => 0;
        public string Name => "课程表";
        public string ScriptFileUri => "";
        public bool IsPost => true;
        public string PostValue => "";
        public string StorageFile => "blank.kcb.json";

        public async Task Execute()
        {
            await Task.Run(() => System.Diagnostics.Debug.WriteLine("HandSchool.Blank.ScheduleEntrance->Excute()"));
            Parse();
            Save();
        }

        public void Parse()
        {
            System.Diagnostics.Debug.WriteLine("HandSchool.Blank.ScheduleEntrance->Parse()");
        }

        public void RenderWeek(int week, out List<CurriculumItem> list, bool showAll = false)
        {
            if (showAll)
                throw new NotImplementedException();

            list = Items.FindAll((item) => showAll || item.IfShow(week));
        }

        public void Save()
        {
            Items.Sort((x, y) => (x.WeekDay * 100 + x.DayBegin).CompareTo(y.WeekDay * 100 + y.DayBegin));
            WriteConfFile("blank.kcb.json", Serialize(Items));
        }

        public Schedule()
        {
            LastReport = ReadConfFile("blank.kcb.json");
            if (LastReport != "")
                Items = JSON<List<CurriculumItem>>(LastReport);
            else
                Items = new List<CurriculumItem>();
            Items.Sort((x, y) => (x.WeekDay * 100 + x.DayBegin).CompareTo(y.WeekDay * 100 + y.DayBegin));
        }
    }

    class FeedEntrance : IFeedEntrance
    {
        public string Name => "RSS阅读器";
        public string ScriptFileUri { get; }
        public bool IsPost => false;
        public string PostValue => string.Empty;
        public string StorageFile => "blank.feed.xml";
        public string LastReport { get; private set; } = string.Empty;
        public DateTime LastUpdate { get; private set; }

        public FeedEntrance(string url)
        {
            ScriptFileUri = url;
            var lu = ReadConfFile(StorageFile + ".time");
            if (lu == "" || (LastUpdate = DateTime.Parse(lu)).AddHours(1).CompareTo(DateTime.Now) == -1)
            {
                Task.Run(Execute);
            }
            else
            {
                LastReport = ReadConfFile(StorageFile);
                Parse();
            }
        }

        public async Task Execute()
        {
            using (var client = new AwaredWebClient("", System.Text.Encoding.UTF8))
                LastReport = await client.GetAsync(ScriptFileUri, "application/rss+xml");
            LastReport = LastReport.Trim();
            WriteConfFile(StorageFile, LastReport);
            WriteConfFile(StorageFile + ".time", DateTime.Now.ToString());
            Parse();
        }

        public void Parse()
        {
            if (LastReport == "") return;
            var xdoc = XDocument.Parse(LastReport);
            var id = 0;
            var items = (from item in xdoc.Root.Element("channel").Descendants("item")
                            select new FeedItem
                            {
                                Title = (string)item.Element("title"),
                                Description = (string)item.Element("description"),
                                PubDate = (string)item.Element("pubDate"),
                                Category = (string)item.Elements("category").Last(),
                                Link = (string)item.Element("link"),
                                Id = id++
                            });
            FeedViewModel.Instance.Items.Clear();
            foreach (var item in items) FeedViewModel.Instance.Items.Add(item);
        }
    }
}
