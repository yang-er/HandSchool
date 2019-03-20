using Autofac;
using HandSchool.Design;
using HandSchool.Internals;
using HandSchool.JLU;
using HandSchool.JLU.InfoQuery;
using HandSchool.JLU.Models;
using HandSchool.JLU.Services;
using HandSchool.JLU.ViewModels;
using HandSchool.JLU.Views;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;

[assembly: RegisterService(typeof(JluLoader))]
[assembly: ExportSchool("JLU", "吉林大学", typeof(JluLoader), typeof(HelloPage))]
namespace HandSchool.JLU
{
    [UseStorage("JLU", configFile)]
    public class JluLoader : SchoolBuilder
    {
        const string configFile = "jlu.config.json";

        internal SettingsJson InternalSettings { get; set; }

        public JluLoader(ILogger<JluLoader> logger, IConfiguration config)
            : base("吉林大学", "JLU", logger, config, 11)
        {
        }
        
        protected override void Startup(ContainerBuilder that)
        {
            base.Startup(that);

            InternalSettings = Configure.ReadAs<SettingsJson>(configFile);

            // 注册校园服务类
            that.RegisterType<YktService>()
                .InstancePerLifetimeScope();
            that.RegisterType<YktViewModel>()
                .InstancePerLifetimeScope();

            that.RegisterType<OA>()
                .As<IFeedEntrance>()
                .InstancePerLifetimeScope();
            that.RegisterType<FeedViewModel>()
                .InstancePerLifetimeScope();

            that.RegisterType<LibrarySearch>();
            that.RegisterType<LibraryRent>();
            // this.RegisterType<LibraryZwyy>();

            // 注册校内外对应的服务类
            if (InternalSettings.OutsideSchool)
            {
                that.RegisterType<CjcxSchool>()
                    .As<ISchoolSystem>()
                    .InstancePerLifetimeScope();

                that.RegisterType<CjcxGrade>()
                    .As<IGradeEntrance>()
                    .InstancePerLifetimeScope();
                that.RegisterType<GradePointViewModel>()
                    .InstancePerLifetimeScope();

                that.RegisterType<LibrarySearch>();
            }
            else
            {
                that.RegisterType<UimsSchool>()
                    .As<ISchoolSystem>()
                    .InstancePerLifetimeScope();

                that.RegisterType<UimsGrade>()
                    .As<IGradeEntrance>()
                    .InstancePerLifetimeScope();
                that.RegisterType<GradePointViewModel>()
                    .InstancePerLifetimeScope();

                that.RegisterType<UimsMessage>()
                    .As<IMessageEntrance>()
                    .InstancePerLifetimeScope();
                that.RegisterType<MessageViewModel>()
                    .InstancePerLifetimeScope();

                that.RegisterType<UimsSchedule>()
                    .As<IScheduleEntrance>()
                    .InstancePerLifetimeScope();
                that.RegisterType<ScheduleViewModel>()
                    .InstancePerLifetimeScope();

                that.RegisterType<EmptyRoom>();
                that.RegisterType<TeachEvaluate>();
                that.RegisterType<CollegeIntroduce>();
                that.RegisterType<ProgramMaster>();
                that.RegisterType<ClassSchedule>();
                that.RegisterType<SelectCourse>();
                that.RegisterType<AdviceSchedule>();
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