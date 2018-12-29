using HandSchool.JLU.InfoQuery;
using HandSchool.JLU.ViewModels;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HandSchool.JLU.Services;
using HandSchool.Internal;
using HandSchool.JLU;

[assembly:RegisterService(typeof(Loader))]
namespace HandSchool.JLU
{
    [UseStorage("jlu", configFile)]
    public class Loader : ISchoolWrapper
    {
        public string SchoolName => "吉林大学";
        public string SchoolId => "jlu";
        const string configFile = "jlu.config.json";

        public Lazy<ISchoolSystem> Service { get; set; }
        public Lazy<IGradeEntrance> GradePoint { get; set; }
        public Lazy<IScheduleEntrance> Schedule { get; set; }
        public Lazy<IMessageEntrance> Message { get; set; }
        public Lazy<IFeedEntrance> Feed { get; set; }
        public EventHandler<LoginStateEventArgs> NoticeChange { get; set; }

        public static List<string> RegisteredFiles { get; set; } = new List<string>();

        internal static SchoolCard Ykt;
        public static InfoEntranceGroup InfoList;

        public void PostLoad()
        {
            Ykt = new SchoolCard();
            NavigationViewModel.Instance.AddMenuEntry("一卡通", "YktPage", "\xE719", "JLU");
        }

        public void PreLoad()
        {
            Core.App.DailyClassCount = 11;
            
            foreach (RegisterServiceAttribute register in GetType().Assembly.GetCustomAttributes(typeof(RegisterServiceAttribute), false))
            {
                if (register.RegisterType.GetCustomAttribute(typeof(UseStorageAttribute)) is UseStorageAttribute stg)
                {
                    if (stg.School == "jlu")
                    {
                        RegisteredFiles.AddRange(stg.Files);
                    }
                }
            }

            var lp = Core.ReadConfig(configFile);
            SettingsJSON config = lp != "" ? lp.ParseJSON<SettingsJSON>() : new SettingsJSON();

            Service = new Lazy<ISchoolSystem>(() => new UIMS(config, NoticeChange));
            GradePoint = new Lazy<IGradeEntrance>(() => new InsideGradeEntrance());
            Schedule = new Lazy<IScheduleEntrance>(() => new Schedule());
            Message = new Lazy<IMessageEntrance>(() => new MessageEntrance());
            Feed = new Lazy<IFeedEntrance>(() => new OA());
            
            InfoList = new InfoEntranceGroup { GroupTitle = "公共信息查询" };
            InfoList.Add(new InfoEntranceWrapper(typeof(EmptyRoom)));
            InfoList.Add(new InfoEntranceWrapper(typeof(TeachEvaluate)));
            InfoList.Add(new InfoEntranceWrapper(typeof(CollegeIntroduce)));
            InfoList.Add(new InfoEntranceWrapper(typeof(ProgramMaster)));
            InfoList.Add(new InfoEntranceWrapper(typeof(ClassSchedule)));
            InfoList.Add(new InfoEntranceWrapper(typeof(SelectCourse)));
            InfoList.Add(new InfoEntranceWrapper(typeof(LibrarySearch)));
            // InfoList.Add(new InfoEntranceWrapper(typeof(LibraryZwyy)));
            InfoList.Add(new InfoEntranceWrapper(typeof(AdviceSchedule)));
            Core.App.InfoEntrances.Add(InfoList);
        }

        public override string ToString()
        {
            return SchoolName;
        }

        internal class SettingsJSON
        {
            public string ProxyServer { get; set; } = "10.60.65.8"; // uims.jlu.edu.cn
            public bool UseHttps { get; set; } = true;
            public bool OutsideSchool { get; set; } = false;
        }
        
        public static void SaveSettings(ISchoolSystem uims)
        {
            var service = uims as UIMS;
            var save = new SettingsJSON
            {
                ProxyServer = service.ProxyServer,
                UseHttps = service.UseHttps,
                OutsideSchool = service.OutsideSchool,
            }.Serialize();

            Core.WriteConfig(configFile, save);
        }
    }
}
