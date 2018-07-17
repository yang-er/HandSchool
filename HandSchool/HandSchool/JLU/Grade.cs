using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
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

    class GradeEntrance : IGradeEntrance
    {
        public int RowLimit { get; set; } = 15;

        public string Name => "成绩查询";
        public string ScriptFileUri => "service/res.do";
        public bool IsPost => true;
        public string PostValue => "{\"tag\":\"archiveScore@queryCourseScore\",\"branch\":\"latest\",\"params\":{},\"rowLimit\":" + RowLimit + "}";
        public string GPAPostValue => "{\"type\":\"query\",\"res\":\"stat-avg-gpoint\",\"params\":{\"studId\":`studId`}}";
        public string StorageFile => "jlu.grade.json";
        public string LastReport { get; private set; }
        
        public async Task Execute()
        {
            LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
            Core.WriteConfig(StorageFile, LastReport);
            Parse();
        }
        
        public GradeEntrance()
        {
            new GradePointViewModel();
            LastReport = Core.ReadConfig(StorageFile);
            if (LastReport != "") Parse();
        }

        public void Parse()
        {
            var ro = JSON<RootObject<ArchiveScoreValue>>(LastReport);
            GradePointViewModel.Instance.Items.Clear();
            foreach (var asv in ro.value)
            {
                GradePointViewModel.Instance.Items.Add(new GradeItem(asv));
            }
        }

        public async Task<string> GatherGPA()
        {
            LastReport = await Core.App.Service.Post(ScriptFileUri, GPAPostValue);
            var ro = JSON<RootObject<GPAValue>>(LastReport);
            return string.Format("按首次成绩\n学分平均绩点 {0:N6}\n学分平均成绩 {1:N6}\n\n按最好成绩\n学分平均绩点 {2:N6}\n学分平均成绩 {3:N6}",
                ro.value[0].gpaFirst, ro.value[0].avgScoreFirst, ro.value[0].gpaBest, ro.value[0].avgScoreBest);
        }
    }
}
