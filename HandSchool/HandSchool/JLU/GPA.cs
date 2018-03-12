using HandSchool.Internal;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    class GPA : ISystemEntrance
    {
        public string Name => "学分绩点统计";
        public string ScriptFileUri => "service/res.do";
        public bool IsPost => true;
        public string StorageFile => "jlu.gpa.json";
        public string PostValue => "{\"type\":\"query\",\"res\":\"stat-avg-gpoint\",\"params\":{\"studId\":" + (App.Service as UIMS).LoginInfo.userId + "}}";
        public string ResultShown { get; private set; }
        public string LastReport { get; private set; }

        public void Execute()
        {
            LastReport = App.Service.PostJson(ScriptFileUri, PostValue);
            var ro = JSON<RootObject>(LastReport);
            ResultShown = string.Format("按首次成绩\n学分平均绩点 {0:N6}\n学分平均成绩 {1:N6}\n\n按最好成绩\n学分平均绩点 {2:N6}\n学分平均成绩 {3:N6}",
                ro.value[0].gpaFirst, ro.value[0].avgScoreFirst, ro.value[0].gpaBest, ro.value[0].avgScoreBest);
        }

        public void Parse() {}

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
