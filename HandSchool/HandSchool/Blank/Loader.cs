using HandSchool.Services;

namespace HandSchool.Blank
{
    class Loader : ISchoolWrapper
    {
        public string SchoolName => "任意大学";
        public string SchoolId => "blank";

        public void PostLoad() { }

        public void PreLoad()
        {
            var sch = new BlankSchool();
            Core.App.Service = sch;
            Core.App.Schedule = new Schedule();
            if (sch.FeedUrl != "") Core.App.Feed = new FeedEntrance(sch.FeedUrl);
        }

        public override string ToString()
        {
            return SchoolName;
        }
    }
}
