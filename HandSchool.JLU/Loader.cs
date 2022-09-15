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

        private static Lazy<SchoolCard> _lazySchoolCard;
        public static SchoolCard Ykt => _lazySchoolCard.Value;
        
        internal static Vpn Vpn => Vpn.Instance;

        
        public static InfoEntranceGroup InfoList;
        
        public static string GetRealUrl(string ori)
        {
            return Vpn.UseVpn ? Vpn.Instance.GetProxyUrl(ori) : ori;
        }

        public void PostLoad()
        {
            Core.Reflection.RegisterImplement<ClassInfoSimplifier, JLUClassSimplifier>();
            Core.Reflection.RegisterImplement<IWebClient, Vpn.VpnHttpClient>();
            Core.Reflection.RegisterConstructor<InitializePage>();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Core.Reflection.RegisterConstructor<XykIos>();
                    Core.Reflection.RegisterConstructor<XykIosMoreInfo>();
                    break;
                default:
                    Core.Reflection.RegisterConstructor<XykDroid>();
                    break;
            }

            SettingViewModel.Instance.Items.Add(new SettingWrapper(typeof(Vpn).GetProperty("UseVpn")));

            NavigationViewModel.Instance.AddMenuEntry("校园卡", Device.RuntimePlatform == Device.iOS ? "XykIos" : "XykDroid",
                "JLU", MenuIcon.CreditCard);
            
            _lazySchoolCard = new Lazy<SchoolCard>(() => new SchoolCard());
            SchoolApplication.Actioning += (s, e) => CheckVpn();
            SchoolApplication.OnLoaded(this, EventArgs.Empty);
        }

        private static async Task<TaskResp> CheckVpn()
        {
            if (!Vpn.UseVpn || await Vpn.CheckLogin()) return true;
            return new TaskResp(false, "需登录吉大Vpn");
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
            SchoolApplication.OnLoading(this, EventArgs.Empty);
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
            
            Vpn.UseVpn = config.UseVpn;
            Service = new Lazy<ISchoolSystem>(() => new UIMS(config, NoticeChange));
            GradePoint = new Lazy<IGradeEntrance>(() => new GradeEntrance());
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
                UseVpn = Vpn.UseVpn,
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
