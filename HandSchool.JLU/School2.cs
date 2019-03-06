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
using HandSchool.JLU.InfoQuery;

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

            this.RegisterType<LibrarySearch>();
            this.RegisterType<LibraryRent>();
            // this.RegisterType<LibraryZwyy>();

            // 注册校内外对应的服务类
            if (InternalSettings.OutsideSchool)
            {
                this.RegisterType<CjcxSchool>()
                    .As<ISchoolSystem>()
                    .InstancePerLifetimeScope();

                this.RegisterType<CjcxGrade>()
                    .As<IGradeEntrance>()
                    .InstancePerLifetimeScope();
                this.RegisterType<GradePointViewModel>()
                    .InstancePerLifetimeScope();

                this.RegisterType<LibrarySearch>();
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

                this.RegisterType<EmptyRoom>();
                this.RegisterType<TeachEvaluate>();
                this.RegisterType<CollegeIntroduce>();
                this.RegisterType<ProgramMaster>();
                this.RegisterType<ClassSchedule>();
                this.RegisterType<SelectCourse>();
                this.RegisterType<AdviceSchedule>();
            }
        }

        protected override HeadedList<SettingWrapper> EnumerateSettings()
        {
            var list = base.EnumerateSettings();
            list.AddRange(SettingWrapper.From(Resolve<ISchoolSystem>()));
            return list;
        }

        public override HeadedObservableCollection<InfoEntranceWrapper> EnumerateInfoQuery()
        {
            var collection = base.EnumerateInfoQuery();
            collection.AddType(Resolve<LibrarySearch>);

            if (!InternalSettings.OutsideSchool)
            {
                collection.AddType(Resolve<EmptyRoom>);
                collection.AddType(Resolve<TeachEvaluate>);
                collection.AddType(Resolve<CollegeIntroduce>);
                collection.AddType(Resolve<ProgramMaster>);
                collection.AddType(Resolve<ClassSchedule>);
                collection.AddType(Resolve<SelectCourse>);
                collection.AddType(Resolve<AdviceSchedule>);
            }

            return collection;
        }
    }
}