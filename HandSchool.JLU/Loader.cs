﻿using HandSchool.Internals;
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
    [UseStorage("JLU", configFile)]
    public class Loader : ISchoolWrapper
    {
        public string SchoolName => "吉林大学";
        public string SchoolId => "jlu";
        const string configFile = "jlu.config.json";
        public Type HelloPage => typeof(HelloPage);
        public const string FileBaseUrl = "https://gitee.com/tlylz99/HandSchool/raw/new-2/HandSchool.JLU";
        public Lazy<ISchoolSystem> Service { get; set; }
        public Lazy<IGradeEntrance> GradePoint { get; set; }
        public Lazy<IScheduleEntrance> Schedule { get; set; }
        public Lazy<IMessageEntrance> Message { get; set; }
        public Lazy<IFeedEntrance> Feed { get; set; }
        public EventHandler<LoginStateEventArgs> NoticeChange { get; set; }
        public static WebDialogAdditionalArgs CancelLostWebAdditionalArgs { set => YktViewModel.CancelLostWebAdditionalArgs = value; }

        public List<string> RegisteredFiles { get; private set; }
        public static SchoolCard Ykt;
        internal static WebVpn Vpn;
        public static LibRoomReservation LibRoom;
        public static InfoEntranceGroup InfoList;
        
        public static string GetRealUrl(string ori)
        {
            return WebVpn.Instance.GetProxyUrl(ori);
        }
        public void PostLoad()
        {
            Core.Reflection.RegisterType<ClassInfoSimplifier, JLUClassSimplifier>();
            Core.Reflection.RegisterType<IWebClient, WebVpn.VpnHttpClient>();
            
            Ykt = new SchoolCard();
            LibRoom = new LibRoomReservation();
            SettingViewModel.Instance.Items.Add(new SettingWrapper(typeof(WebVpn).GetProperty("UseVpn")));
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Core.Reflection.RegisterCtor<XykIos>();
                    Core.Reflection.RegisterCtor<XykIosMoreInfo>();
                    break;
                default: Core.Reflection.RegisterCtor<XykDroid>();break;
            }
            Core.Reflection.RegisterCtor<InitializePage>();
            NavigationViewModel.Instance.AddMenuEntry("校园卡", Core.Platform.RuntimeName == "iOS" ? "XykIos" : "XykDroid", "JLU", MenuIcon.CreditCard);
            NavigationViewModel.Instance.AddMenuEntry("鼎新馆预约", nameof(LibRoomReservationPage), "JLU", MenuIcon.LibRoomResv);

            Vpn = WebVpn.Instance;

            FeedViewModel.BeforeOperatingCheck = CheckVpn;
            IndexViewModel.BeforeOperatingCheck = CheckVpn;
            YktViewModel.BeforeOperatingCheck = CheckVpn;
            MessageViewModel.BeforeOperatingCheck = CheckVpn;
            GradePointViewModel.BeforeOperatingCheck = CheckVpn;
            ScheduleViewModel.BeforeOperatingCheck = CheckVpn;
        }
        private static async Task<TaskResp> CheckVpn()
        {
            if (WebVpn.UseVpn)
            {
                if (!await Vpn.CheckLogin())
                {
                    return new TaskResp(false, "需登录Vpn");
                }
            }
            return TaskResp.True;
        }
        public void PreLoad()
        {
            Core.App.CityWeatherCode = "101060101";
            Core.App.DailyClassCount = 11;
            RegisteredFiles = new List<string>();
            Core.Reflection.RegisterFiles(this.GetAssembly(), "JLU", RegisteredFiles);
            
            var lp = Core.Configure.Read(configFile);
            SettingsJSON config = lp != "" ? lp.ParseJSON<SettingsJSON>() : new SettingsJSON();
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
            Core.Configure.Write(configFile, json.Serialize());
        }
    }
}
