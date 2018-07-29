using HandSchool.JLU.InfoQuery;
using HandSchool.JLU.ViewModels;
using HandSchool.Models;
using HandSchool.Services;
using System.Threading.Tasks;

namespace HandSchool.JLU
{
    class Loader : ISchoolWrapper
    {
        public string SchoolName => "吉林大学";
        public string SchoolId => "jlu";

        public static SchoolCard Ykt;

        public void PostLoad()
        {
            Task.Run(() => JsonObject.AlreadyKnownThings.Initialize());
            new YktViewModel();
            Ykt = new SchoolCard();
        }

        public void PreLoad()
        {
            Core.App.Service = new UIMS();
            Core.App.DailyClassCount = 11;
            Core.App.GradePoint = new GradeEntrance();
            Core.App.Schedule = new Schedule();
            Core.App.Message = new MessageEntrance();
            Core.App.Feed = new OA();
            var group1 = new InfoEntranceGroup { GroupTitle = "公共信息查询" };
            group1.Add(new InfoEntranceWrapper(typeof(EmptyRoom)));
            group1.Add(new InfoEntranceWrapper(typeof(TeachEvaluate)));
            group1.Add(new InfoEntranceWrapper(typeof(CollegeIntroduce)));
            group1.Add(new InfoEntranceWrapper(typeof(ProgramMaster)));
            group1.Add(new InfoEntranceWrapper(typeof(ClassSchedule)));
            Core.App.InfoEntrances.Add(group1);
        }

        public override string ToString()
        {
            return SchoolName;
        }
    }
}
