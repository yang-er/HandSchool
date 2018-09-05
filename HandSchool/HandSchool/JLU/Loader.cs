using HandSchool.JLU.InfoQuery;
using HandSchool.JLU.ViewModels;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Threading.Tasks;

namespace HandSchool.JLU
{
    class Loader : ISchoolWrapper
    {
        public string SchoolName => "吉林大学";
        public string SchoolId => "jlu";

        public static SchoolCard Ykt;
        public static InfoEntranceGroup InfoList;

        public void PostLoad()
        {
            Task.Run(() => JsonObject.AlreadyKnownThings.Initialize());
            new YktViewModel();
            Ykt = new SchoolCard();
            NavigationViewModel.Instance.AddMenuEntry("一卡通", "YktPage", "\xE719", "JLU");
        }

        public void PreLoad()
        {
            Core.App.Service = new UIMS();
            Core.App.DailyClassCount = 11;
            Core.App.GradePoint = new GradeEntrance();
            Core.App.Schedule = new Schedule();
            Core.App.Message = new MessageEntrance();
            Core.App.Feed = new OA();
            InfoList = new InfoEntranceGroup { GroupTitle = "公共信息查询" };
            InfoList.Add(new InfoEntranceWrapper(typeof(EmptyRoom)));
            InfoList.Add(new InfoEntranceWrapper(typeof(TeachEvaluate)));
            InfoList.Add(new InfoEntranceWrapper(typeof(CollegeIntroduce)));
            InfoList.Add(new InfoEntranceWrapper(typeof(ProgramMaster)));
            InfoList.Add(new InfoEntranceWrapper(typeof(ClassSchedule)));
            InfoList.Add(new InfoEntranceWrapper(typeof(SelectCourse)));
            Core.App.InfoEntrances.Add(InfoList);
        }

        public override string ToString()
        {
            return SchoolName;
        }
    }
}
