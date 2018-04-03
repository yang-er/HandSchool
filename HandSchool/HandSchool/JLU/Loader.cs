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
            InfoEntrances = new List<InfoEntranceWrapper>
            {
                new InfoEntranceWrapper { Name = "学院介绍查询", Description = "查询学院介绍", Load = () => new CollegeIntroduce() }
            };
            return () => { };
        }
    }
}
