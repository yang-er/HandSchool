using Autofac;
using HandSchool.Design;
using HandSchool.Internals;
using HandSchool.JLU.Models;
using HandSchool.JLU.Services;
using HandSchool.JLU.ViewModels;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Threading.Tasks;
using HandSchool.JLU;
using HandSchool.Models;

[assembly: RegisterService(typeof(JluLoader))]
namespace HandSchool.JLU
{
    [UseStorage("JLU", configFile)]
    public class JluLoader : SchoolBuilder
    {
        const string configFile = "jlu.config.json";

        internal SettingsJson InternalSettings { get; }

        public JluLoader(ILogger<JluLoader> logger, IConfiguration config)
            : base("吉林大学", "JLU", logger, config, 11)
        {
            InternalSettings = Configure.ReadAs<SettingsJson>(configFile);
        }
        
        public override void Startup()
        {
            base.Startup();

            // 注册校园服务类
            this.RegisterType<YktService>()
                .InstancePerLifetimeScope();
            this.RegisterType<YktViewModel>()
                .InstancePerLifetimeScope();

            this.RegisterType<OA>()
                .As<IFeedEntrance>()
                .InstancePerLifetimeScope();
            this.RegisterType<FeedViewModel>()
                .InstancePerLifetimeScope();

            // 注册校内外对应的服务类
            if (Settings.OutsideSchool)
            {
                this.RegisterType<CjcxSchool>()
                    .As<ISchoolSystem>()
                    .InstancePerLifetimeScope();

                this.RegisterType<CjcxGrade>()
                    .As<IGradeEntrance>()
                    .InstancePerLifetimeScope();
                this.RegisterType<GradePointViewModel>()
                    .InstancePerLifetimeScope();
            }
            else
            {
                this.RegisterType<UimsSchool>()
                    .As<ISchoolSystem>()
                    .InstancePerLifetimeScope();

                this.RegisterType<UimsGrade>()
                    .As<IGradeEntrance>()
                    .InstancePerLifetimeScope();
                this.RegisterType<GradePointViewModel>()
                    .InstancePerLifetimeScope();

                this.RegisterType<UimsMessage>()
                    .As<IMessageEntrance>()
                    .InstancePerLifetimeScope();
                this.RegisterType<MessageViewModel>()
                    .InstancePerLifetimeScope();

                this.RegisterType<UimsSchedule>()
                    .As<IScheduleEntrance>()
                    .InstancePerLifetimeScope();
                this.RegisterType<ScheduleViewModel>()
                    .InstancePerLifetimeScope();
            }
        }

        protected override HeadedList<SettingWrapper> EnumerateSettings()
        {
            var list = base.EnumerateSettings();
            list.AddRange(SettingWrapper.From(Resolve<ISchoolSystem>()));
            return list;
        }

        public override async Task LoadDataAsync()
        {
            await base.LoadDataAsync();
        }
    }
}