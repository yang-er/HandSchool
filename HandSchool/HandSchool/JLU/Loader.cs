using HandSchool.JLU;
using HandSchool.JLU.InfoQuery;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HandSchool
{
    public partial class App : Application
    {
        private Action LoadJLU()
        {
            Service = new UIMS();
            DailyClassCount = 11;
            GradePoint = new GradeEntrance();
            Schedule = new Schedule();
            Message = new MessageEntrance();
            Feed = new OA();
            var group1 = new InfoEntranceGroup { GroupTitle = "公共信息查询" };
            group1.Add(new InfoEntranceWrapper("学院介绍查询", "查询学院介绍", () => new CollegeIntroduce()));
            InfoEntrances.Add(group1);
            return () => { };
        }
    }

    public partial class Core
    {
        public ISchoolWrapper JLU { get; } = new Loader();
    }

    namespace JLU
    {
        class Loader : ISchoolWrapper
        {
            public string SchoolName => "吉林大学";
            public string SchoolId => "jlu";

            public void PostLoad() { }

            public void PreLoad()
            {
                Core.App.Service = new UIMS();
                Core.App.DailyClassCount = 11;
                Core.App.GradePoint = new GradeEntrance();
                Core.App.Schedule = new Schedule();
                Core.App.Message = new MessageEntrance();
                Core.App.Feed = new OA();
                var group1 = new InfoEntranceGroup { GroupTitle = "公共信息查询" };
                group1.Add(new InfoEntranceWrapper("学院介绍查询", "查询学院介绍", () => new CollegeIntroduce()));
                Core.App.InfoEntrances.Add(group1);
            }
        }
    }
}
