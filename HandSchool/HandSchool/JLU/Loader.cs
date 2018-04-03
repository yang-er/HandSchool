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
            var group1 = new InfoEntranceGroup { GroupTitle = "公共信息查询" };
            group1.Add(new InfoEntranceWrapper("学院介绍查询", "查询学院介绍", () => new CollegeIntroduce()));
            group1.Add(new InfoEntranceWrapper("学院介绍查询", "查询学院介绍", () => new CollegeIntroduce()));
            InfoEntrances.Add(group1);
            return () => { };
        }
    }
}
