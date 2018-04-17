using HandSchool.Blank;
using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using static HandSchool.Internal.Helper;

namespace HandSchool
{
    public partial class Core
    {
        public ISchoolWrapper Blank { get; } = new Loader();
    }

    namespace Blank
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
                Core.App.DailyClassCount = 10;
                Core.App.Schedule = new Schedule();
                if (sch.FeedUrl != "") Core.App.Feed = new FeedEntrance(sch.FeedUrl);
            }
        }

        class BlankSchool : NotifyPropertyChanged, ISchoolSystem
        {
            private string feedUrl = "";

            public string ServerUri => "";
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
            public bool IsLogin => true;
            public bool NeedLogin => false;
            public int CurrentWeek { get; set; } = 1;
            public string Tips => "";
            public bool AutoLogin { get; set; } = true;
            public bool SavePassword { get; set; } = true;
            public string WelcomeMessage => "欢迎。";
            public string CurrentMessage => "";

            [Settings("每日课程数量", "每日有多少节课，配合课程表使用。")]
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
            public string Name => "消息通知";
            public string ScriptFileUri { get; }
            public bool IsPost => false;
            public string PostValue => "";
            public string StorageFile => "blank.feed.xml";
            public string LastReport { get; private set; }

            public async Task Execute()
            {
                await Task.Run(() => System.Diagnostics.Debug.WriteLine("HandSchool.Blank.FeedIntrance->Excute()"));
            }

            public void Parse()
            {
                System.Diagnostics.Debug.WriteLine("HandSchool.Blank.FeedIntrance->Parse()");
            }

            public FeedEntrance(string uri)
            {
                ScriptFileUri = uri;
            }
        }
    }
}
