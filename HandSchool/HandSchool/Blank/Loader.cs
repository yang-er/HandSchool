using System;
using HandSchool.Models;
using HandSchool.Services;

namespace HandSchool.Blank
{
    class Loader : ISchoolWrapper
    {
        public string SchoolName => "任意大学";
        public string SchoolId => "blank";

        public Lazy<ISchoolSystem> Service { get; set; }
        public Lazy<IGradeEntrance> GradePoint { get; set; }
        public Lazy<IScheduleEntrance> Schedule { get; set; }
        public Lazy<IMessageEntrance> Message { get; set; }
        public Lazy<IFeedEntrance> Feed { get; set; }
        public EventHandler<LoginStateEventArgs> NoticeChange { get; set; }

        public void PostLoad() { }

        public void PreLoad()
        {
            var sch = new BlankSchool();
            Service = new Lazy<ISchoolSystem>(() => sch);
            Schedule = new Lazy<IScheduleEntrance>(() => new Schedule());
            GradePoint = new Lazy<IGradeEntrance>(() => null);
            Message = new Lazy<IMessageEntrance>(() => null);
            Feed = new Lazy<IFeedEntrance>(() =>
            {
                if (sch.FeedUrl != "") return new FeedEntrance(sch.FeedUrl);
                else return null;
            });
        }

        public override string ToString()
        {
            return SchoolName;
        }
    }
}
