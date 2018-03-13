using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using HandSchool.ViewModels;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    class GradeItem : IGradeItem
    {
        private ArchiveScoreValue asv;

        public GradeItem(ArchiveScoreValue value)
        {
            asv = value;
            Attach = new NameValueCollection
            {
                { "选课课号", asv.xkkh }
            };
        }

        public string Name => asv.course.courName;
        public string Score => asv.score;
        public string Point => asv.gpoint;
        public string Credit => asv.credit;
        public bool ReSelect => asv.isReselect != "N";
        public bool Pass => asv.isPass == "Y";
        public string Term => asv.teachingTerm.termName;
        public DateTime Date => asv.dateScore;
        public NameValueCollection Attach { get; private set; }

        public string Type
        {
            get
            {
                return "";
            }
        }

        public string Show => string.Format("{2}发布；{0}通过，绩点 {1}。", Pass ? "已" : "未", Point, Date.ToShortDateString());
    }

    class GradeEntrance : ISystemEntrance
    {
        public int RowLimit = 15;

        public string Name => "成绩查询";
        public string ScriptFileUri => "service/res.do";
        public bool IsPost => true;
        public string PostValue => "{\"tag\":\"archiveScore@queryCourseScore\",\"branch\":\"latest\",\"params\":{},\"rowLimit\":" + RowLimit + "}";
        public string StorageFile => "jlu.grade.json";
        public string LastReport { get; private set; }
        
        public async Task Execute()
        {
            LastReport = await App.Current.Service.PostJson(ScriptFileUri, PostValue);
            WriteConfFile(StorageFile, LastReport);
            Parse();
        }
        
        public GradeEntrance()
        {
            new GradePointViewModel();
            LastReport = ReadConfFile(StorageFile);
            if (LastReport != "") Parse();
        }

        public void Parse()
        {
            var ro = JSON<RootObject>(LastReport);
            foreach (var asv in ro.value)
            {
                GradePointViewModel.Instance.Items.Add(new GradeItem(asv));
            }
        }

        public class RootObject
        {
            public string id { get; set; }
            public int status { get; set; }
            public ArchiveScoreValue[] value { get; set; }
            public string resName { get; set; }
            public string msg { get; set; }
        }
    }
}
