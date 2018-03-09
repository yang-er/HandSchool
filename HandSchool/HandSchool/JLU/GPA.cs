using HandSchool.Internal;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    class GPA : ISystemEntrance
    {
        private string lastReport;

        public string Name => "学分绩点统计";
        public string ScriptFileUri => "service/res.do";
        public bool IsPost => true;

        public string PostValue
        {
            get
            {
                var uims = (UIMS)App.Service;
                return "{\"type\":\"query\",\"res\":\"stat-avg-gpoint\",\"params\":{\"studId\":" + uims.LoginInfo.userId + "}}";
            }
        }

        public string ResultShown { get; private set; }

        public void Execute()
        {
            lastReport = App.Service.Post(ScriptFileUri, PostValue);
            var ro = JSON<RootObject>(lastReport);
            ResultShown = string.Format("按首次成绩，\n学分平均绩点 {0:N6}\n学分平均绩点 {1:N6}\n\n按最好成绩，\n学分平均绩点 {2:N6}\n学分平均绩点 {3:N6}",
                ro.value[0].gpaFirst, ro.value[0].avgScoreFirst, ro.value[0].gpaBest, ro.value[0].avgScoreBest);
        }

        public override string ToString()
        {
            return ResultShown;
        }

        public class RootObject
        {
            public int status { get; set; }
            public GPAValue[] value { get; set; }
            public string resName { get; set; }
            public string msg { get; set; }
        }

        public class GPAValue
        {
            public float avgScoreBest { get; set; }
            public float avgScoreFirst { get; set; }
            public float gpaFirst { get; set; }
            public float gpaBest { get; set; }
        }
    }
}
