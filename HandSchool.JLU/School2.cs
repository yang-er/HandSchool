using HandSchool.Design;
using Autofac;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using HandSchool.JLU.Services;
using HandSchool.JLU.Models;
using HandSchool.Internals;
using HandSchool.JLU.ViewModels;
using System.Threading.Tasks;

namespace HandSchool.JLU
{
    [UseStorage("JLU", configFile)]
    public class JluLoader : SchoolWrapper
    {
        const string configFile = "jlu.config.json";

        internal SettingsJson Settings { get; }

        public JluLoader(ILogger<JluLoader> logger, IConfiguration config)
            : base("吉林大学", "JLU", logger, config, 11)
        {
            Settings = Configure.ReadAs<SettingsJson>(configFile);
        }

        public override void Startup()
        {
            base.Startup();

            // 注册校园服务类
            this.RegisterType<YktService>();
            this.RegisterType<YktViewModel>();

            this.RegisterType<OA>().As<IFeedEntrance>();
            this.RegisterType<FeedViewModel>();

            // 注册校内外对应的服务类
            if (Settings.OutsideSchool)
            {
                this.RegisterType<CjcxSchool>().As<ISchoolSystem>();

                this.RegisterType<CjcxGrade>().As<IGradeEntrance>();
                this.RegisterType<GradePointViewModel>();
            }
            else
            {
                this.RegisterType<UimsSchool>().As<ISchoolSystem>();

                this.RegisterType<UimsGrade>().As<IGradeEntrance>();
                this.RegisterType<GradePointViewModel>();

                this.RegisterType<UimsMessage>().As<IMessageEntrance>();
                this.RegisterType<MessageViewModel>();

                this.RegisterType<UimsSchedule>().As<IScheduleEntrance>();
                this.RegisterType<ScheduleViewModel>();
            }
        }

        public override async Task LoadDataAsync()
        {
            await base.LoadDataAsync();
            
        }
    }
}