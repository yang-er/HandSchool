using HandSchool.JLU.InfoQuery;
using HandSchool.JLU.ViewModels;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Threading.Tasks;

namespace HandSchool.JLU
{
    class Loader : ISchoolWrapper
    {
        public string SchoolName => "吉林大学";
        public string SchoolId => "jlu";

        public Lazy<ISchoolSystem> Service { get; set; }
        public Lazy<IGradeEntrance> GradePoint { get; set; }
        public Lazy<IScheduleEntrance> Schedule { get; set; }
        public Lazy<IMessageEntrance> Message { get; set; }
        public Lazy<IFeedEntrance> Feed { get; set; }
        public EventHandler<LoginStateEventArgs> NoticeChange { get; set; }

        public static SchoolCard Ykt;
        public static InfoEntranceGroup InfoList;

        public void PostLoad()
        {
            Ykt = new SchoolCard();
            NavigationViewModel.Instance.AddMenuEntry("一卡通", "YktPage", "\xE719", "JLU");
        }

        public void PreLoad()
        {
            Core.App.DailyClassCount = 11;
            Service = new Lazy<ISchoolSystem>(() =>
            {
                var uims = new UIMS(NoticeChange);
                NoticeChange?.Invoke(uims, new LoginStateEventArgs(LoginState.Succeeded));
                return uims;
            });

            GradePoint = new Lazy<IGradeEntrance>(() => new GradeEntrance());
            Schedule = new Lazy<IScheduleEntrance>(() => new Schedule());
            Message = new Lazy<IMessageEntrance>(() => new MessageEntrance());
            Feed = new Lazy<IFeedEntrance>(() => new OA());
            
            InfoList = new InfoEntranceGroup { GroupTitle = "公共信息查询" };
            InfoList.Add(new InfoEntranceWrapper(typeof(EmptyRoom)));
            InfoList.Add(new InfoEntranceWrapper(typeof(TeachEvaluate)));
            InfoList.Add(new InfoEntranceWrapper(typeof(CollegeIntroduce)));
            InfoList.Add(new InfoEntranceWrapper(typeof(ProgramMaster)));
            InfoList.Add(new InfoEntranceWrapper(typeof(ClassSchedule)));
            InfoList.Add(new InfoEntranceWrapper(typeof(SelectCourse)));
            InfoList.Add(new InfoEntranceWrapper(typeof(LibrarySearch)));
            Core.App.InfoEntrances.Add(InfoList);
        }

        public override string ToString()
        {
            return SchoolName;
        }
    }
}
