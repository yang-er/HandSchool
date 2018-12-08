using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

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

        public string Type => AlreadyKnownThings.Type5Name(asv.type5);

        public string Show => string.Format("{2}发布；{0}通过，绩点 {1}。", Pass ? "已" : "未", Point, Date.ToShortDateString());
    }

    [Entrance("成绩查询")]
    class GradeEntrance : IGradeEntrance
    {
        internal const string config_grade = "jlu.grade.json";
        internal const string config_gpa = "jlu.gpa.json";

        public int RowLimit { get; set; } = 25;
        
        public string ScriptFileUri => "service/res.do";
        public bool IsPost => true;
        public string PostValue => "{\"tag\":\"archiveScore@queryCourseScore\",\"branch\":\"latest\",\"params\":{},\"rowLimit\":" + RowLimit + "}";
        public string GPAPostValue => "{\"type\":\"query\",\"res\":\"stat-avg-gpoint\",\"params\":{\"studId\":`studId`}}";
        public string StorageFile => config_grade;
        public string LastReport { get; private set; }
        public string LastReportGPA { get; private set; }

        public async Task Execute()
        {
            try
            {
                LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    await GradePointViewModel.Instance.ShowMessage("错误", "连接超时，请重试。");
                    return;
                }

                throw ex;
            }

            Core.WriteConfig(config_grade, LastReport);
            await GatherGPA();
            Parse();
        }
        
        public GradeEntrance()
        {
            Task.Run(async () =>
            {
                await Task.Yield();
                LastReportGPA = Core.ReadConfig(config_gpa);
                if (LastReportGPA != "") ParseGPA();
                LastReport = Core.ReadConfig(config_grade);
                if (LastReport != "") Parse();
            });
        }

        public void Parse()
        {
            var ro = LastReport.ParseJSON<RootObject<ArchiveScoreValue>>();
            
            foreach (var asv in ro.value)
            {
                GradePointViewModel.Instance.Items.Add(new GradeItem(asv));
            }
        }

        public void ParseGPA()
        {
            var ro = LastReportGPA.ParseJSON<RootObject<GPAValue>>();
            var str = string.Format("按首次成绩\n学分平均绩点 {0:N6}\n学分平均成绩 {1:N6}\n\n按最好成绩\n学分平均绩点 {2:N6}\n学分平均成绩 {3:N6}",
                ro.value[0].gpaFirst, ro.value[0].avgScoreFirst, ro.value[0].gpaBest, ro.value[0].avgScoreBest);
            GradePointViewModel.Instance.Items.Clear();
            GradePointViewModel.Instance.Items.Add(new GPAItem(str));
        }

        public async Task GatherGPA()
        {
            try
            {
                LastReportGPA = await Core.App.Service.Post(ScriptFileUri, GPAPostValue);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    await GradePointViewModel.Instance.ShowMessage("错误", "连接超时，请重试。");
                    return;
                }

                throw ex;
            }

            Core.WriteConfig(config_gpa, LastReportGPA);
            ParseGPA();
        }
    }
}
