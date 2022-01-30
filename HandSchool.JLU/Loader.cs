using HandSchool.Internals;
using HandSchool.JLU;
using HandSchool.JLU.InfoQuery;
using HandSchool.JLU.Services;
using HandSchool.JLU.ViewModels;
using HandSchool.JLU.Views;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: RegisterService(typeof(Loader))]
[assembly: ExportSchool(typeof(Loader))]
namespace HandSchool.JLU
{
    public class Loader : ISchoolWrapper
    {
        public string SchoolName => "吉林大学";
        public string DataBaseName => DataBase;

        public const string DataBase = "jlu_user_data.db";
        public string SchoolId => "jlu";
        
        const string ConfigName = "uims.config";
        public Type HelloPage => typeof(HelloPage);
        public const string FileBaseUrl = "https://gitee.com/tlylz99/HandSchool/raw/new-2/HandSchool.JLU";
        public Lazy<ISchoolSystem> Service { get; set; }
        public Lazy<IGradeEntrance> GradePoint { get; set; }
        public Lazy<IScheduleEntrance> Schedule { get; set; }
        public Lazy<IMessageEntrance> Message { get; set; }
        public Lazy<IFeedEntrance> Feed { get; set; }
        public EventHandler<LoginStateEventArgs> NoticeChange { get; set; }
        public SQLiteTableManager<UserAccount> AccountManager { get; set; }
        public SQLiteTableManager<ServerJson> JsonManager { get; set; }
        public static WebDialogAdditionalArgs CancelLostWebAdditionalArgs { set => YktViewModel.CancelLostWebAdditionalArgs = value; }

        public List<string> RegisteredFiles { get; private set; }
        public static SchoolCard Ykt;
        internal static WebVpn Vpn;
        public static LibRoomReservation LibRoom;
        public static InfoEntranceGroup InfoList;
        
        public static string GetRealUrl(string ori)
        {
            if (WebVpn.UseVpn)
            {
                return WebVpn.Instance.GetProxyUrl(ori);
            }
            return ori;
        }

        public void PostLoad()
        {
            Core.Reflection.RegisterType<ClassInfoSimplifier, JLUClassSimplifier>();
            Core.Reflection.RegisterType<IWebClient, WebVpn.VpnHttpClient>();
            Core.Reflection.RegisterCtor<InitializePage>();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Core.Reflection.RegisterCtor<XykIos>();
                    Core.Reflection.RegisterCtor<XykIosMoreInfo>();
                    break;
                default:
                    Core.Reflection.RegisterCtor<XykDroid>();
                    break;
            }

            SettingViewModel.Instance.Items.Add(new SettingWrapper(typeof(WebVpn).GetProperty("UseVpn")));

            NavigationViewModel.Instance.AddMenuEntry("校园卡", Core.Platform.RuntimeName == "iOS" ? "XykIos" : "XykDroid",
                "JLU", MenuIcon.CreditCard);
            NavigationViewModel.Instance.AddMenuEntry("鼎新馆预约", nameof(LibRoomReservationPage), "JLU",
                MenuIcon.LibRoomResv);

            Vpn = WebVpn.Instance;
            Ykt = new SchoolCard();
            LibRoom = new LibRoomReservation();
            
            FeedViewModel.BeforeOperatingCheck = 
            IndexViewModel.BeforeOperatingCheck = 
            YktViewModel.BeforeOperatingCheck = 
            MessageViewModel.BeforeOperatingCheck = 
            GradePointViewModel.BeforeOperatingCheck = 
            ScheduleViewModel.BeforeOperatingCheck = CheckVpn;
        }

        private static async Task<TaskResp> CheckVpn()
        {
            if (WebVpn.UseVpn)
            {
                if (!await Vpn.CheckLogin())
                {
                    return new TaskResp(false, "需登录WebVpn");
                }
            }
            return TaskResp.True;
        }

        public Loader()
        {
            AccountManager = 
                new SQLiteTableManager<UserAccount>(true, Core.Platform.ConfigureDirectory, SchoolId, DataBaseName);
            JsonManager =
                new SQLiteTableManager<ServerJson>(true, Core.Platform.ConfigureDirectory, SchoolId, DataBaseName);
        }
        
        public void PreLoad()
        {
            Core.App.DailyClassCount = 11;
            RegisteredFiles = new List<string>
            {
                SchoolId + System.IO.Path.DirectorySeparatorChar + DataBaseName
            };
            Core.Reflection.RegisterFiles(this.GetAssembly(), "JLU", RegisteredFiles);

            var config = JsonManager
                             .GetItemWithPrimaryKey(ConfigName)
                             ?.ToObject<SettingsJSON>()
                         ?? new SettingsJSON();
            
            WebVpn.UseVpn = config.UseVpn;
            Service = new Lazy<ISchoolSystem>(() => new UIMS(config, NoticeChange));
            GradePoint = new Lazy<IGradeEntrance>(() => new GradeEntrance());
            Task.Run(GradeEntrance.PreloadData);
            Schedule = new Lazy<IScheduleEntrance>(() => new Schedule());
            Message = new Lazy<IMessageEntrance>(() => new MessageEntrance());
            Feed = new Lazy<IFeedEntrance>(() => new Oa());

            InfoList = new InfoEntranceGroup("公共信息查询")
            {
                //TapEntranceWrapper.From<EhallFill>(),
                TapEntranceWrapper.From<EmptyRoomPageShell>(),
                //InfoEntranceWrapper.From<TeachEvaluate>(),
                //InfoEntranceWrapper.From<CollegeIntroduce>(),
                //InfoEntranceWrapper.From<ProgramMaster>(),
                //InfoEntranceWrapper.From<ClassSchedule>(),
                //InfoEntranceWrapper.From<SelectCourse>(),
                TapEntranceWrapper.From<SelectCourseShell>(),
                InfoEntranceWrapper.From<AdviceSchedule>(),
                TapEntranceWrapper.From<TeacherEvaShell>(),
            };

            Core.App.InfoEntrances.Add(InfoList);
        }

        public override string ToString()
        {
            return SchoolName;
        }

        public class SettingsJSON
        {
            public SettingsJSON()
            {
                ProxyServer = "10.60.65.8"; // uims.jlu.edu.cn
                UseHttps = false;
                OutsideSchool = false;
                QuickMode = false;
                UseVpn = true;
            }

            public string ProxyServer { get; set; }
            public bool UseHttps { get; set; }
            public bool OutsideSchool { get; set; }
            public bool QuickMode { get; set; }
            public bool UseVpn { get; set; }
        }
        
        public void SaveSettings(ISchoolSystem uims)
        {
            var service = uims as UIMS;
            var save = new SettingsJSON
            {
                ProxyServer = service.ProxyServer,
                UseHttps = service.UseHttps,
                OutsideSchool = service.OutsideSchool,
                QuickMode = service.QuickMode,
                UseVpn = WebVpn.UseVpn,
            };

            SaveSettings(save);
        }

        public void SaveSettings(SettingsJSON json)
        {
            JsonManager.InsertOrUpdateTable(new ServerJson
            {
                JsonName = ConfigName,
                Json = json.Serialize()
            });
        }
    }
}
