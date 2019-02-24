using HandSchool.Internals;
using HandSchool.JLU;
using HandSchool.JLU.InfoQuery;
using HandSchool.JLU.Services;
using HandSchool.JLU.Views;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: RegisterService(typeof(Loader))]
[assembly: ExportSchool(typeof(Loader))]
namespace HandSchool.JLU
{
    [UseStorage("JLU", configFile)]
    public class Loader : ISchoolWrapper
    {
        public string SchoolName => "吉林大学";
        public string SchoolId => "jlu";
        const string configFile = "jlu.config.json";
        public Type HelloPage => typeof(HelloPage);
        public const string FileBaseUrl = "https://raw.githubusercontent.com/yang-er/HandSchool/master/HandSchool/HandSchool/JLU";

        public Lazy<ISchoolSystem> Service { get; set; }
        public Lazy<IGradeEntrance> GradePoint { get; set; }
        public Lazy<IScheduleEntrance> Schedule { get; set; }
        public Lazy<IMessageEntrance> Message { get; set; }
        public Lazy<IFeedEntrance> Feed { get; set; }
        public EventHandler<LoginStateEventArgs> NoticeChange { get; set; }

        public List<string> RegisteredFiles { get; private set; }
        // public static bool OutsideSchool { get; set; }

        internal static SchoolCard Ykt;
        public static InfoEntranceGroup InfoList;

        public void PostLoad()
        {
            Ykt = new SchoolCard();
            Core.Reflection.RegisterCtor<YktViewPresenter>();
            Core.Reflection.RegisterCtor<YktPage>();
            NavigationViewModel.Instance.AddMenuEntry("一卡通", Core.Platform.RuntimeName == "Android" ? "YktPage" : "YktViewPresenter", "JLU", MenuIcon.CreditCard);
        }

        public void PreLoad()
        {
            Core.App.DailyClassCount = 11;
            RegisteredFiles = new List<string>();
            Core.Reflection.RegisterFiles(this.GetAssembly(), "JLU", RegisteredFiles);
            
            var lp = Core.Configure.Read(configFile);
            SettingsJSON config = lp != "" ? lp.ParseJSON<SettingsJSON>() : new SettingsJSON();

            Service = new Lazy<ISchoolSystem>(() => new UIMS(config, NoticeChange));
            GradePoint = new Lazy<IGradeEntrance>(() => new GradeEntrance());
            Task.Run(GradeEntrance.PreloadData);
            Schedule = new Lazy<IScheduleEntrance>(() => new Schedule());
            Message = new Lazy<IMessageEntrance>(() => new MessageEntrance());
            Feed = new Lazy<IFeedEntrance>(() => new OA());
            Task.Run(OA.PreloadData);
            
            InfoList = new InfoEntranceGroup("公共信息查询")
            {
                InfoEntranceWrapper.From<EmptyRoom>(),
                InfoEntranceWrapper.From<TeachEvaluate>(),
                InfoEntranceWrapper.From<CollegeIntroduce>(),
                InfoEntranceWrapper.From<ProgramMaster>(),
                InfoEntranceWrapper.From<ClassSchedule>(),
                InfoEntranceWrapper.From<SelectCourse>(),
                InfoEntranceWrapper.From<LibrarySearch>(),
                // InfoEntranceWrapper.From<LibraryZwyy>(),
                InfoEntranceWrapper.From<AdviceSchedule>(),
            };

            Core.App.InfoEntrances.Add(InfoList);
        }

        public override string ToString()
        {
            return SchoolName;
        }

        internal class SettingsJSON
        {
            public SettingsJSON()
            {
                ProxyServer = "10.60.65.8"; // uims.jlu.edu.cn
                UseHttps = false;
                OutsideSchool = false;
            }

            public string ProxyServer { get; set; }
            public bool UseHttps { get; set; }
            public bool OutsideSchool { get; set; }
        }
        
        public void SaveSettings(ISchoolSystem uims)
        {
            var service = uims as UIMS;
            var save = new SettingsJSON
            {
                ProxyServer = service.ProxyServer,
                UseHttps = service.UseHttps,
                OutsideSchool = service.OutsideSchool,
            }.Serialize();

            Core.Configure.Write(configFile, save);
        }
    }
}
