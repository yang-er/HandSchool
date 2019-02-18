using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.Services;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: RegisterService(typeof(GradeEntrance))]
namespace HandSchool.JLU.Services
{
    /// <summary>
    /// 吉林大学的内网成绩查询服务。
    /// </summary>
    /// <inheritdoc cref="IGradeEntrance" />
    [Entrance("JLU", "成绩查询", "提供内网的成绩查询和查看成绩分布功能。")]
    [UseStorage("JLU", configGpa, configGrade)]
    internal sealed class GradeEntrance : IGradeEntrance
    {
        const string configGrade = "jlu.grade.json";
        const string configGpa = "jlu.gpa.json";

        const string gpaPostValue = "{\"type\":\"query\",\"res\":\"stat-avg-gpoint\",\"params\":{\"studId\":`studId`}}";
        const string scorePostValue = "{\"tag\":\"archiveScore@queryCourseScore\",\"branch\":\"latest\",\"params\":{},\"rowLimit\":25}";
        const string gradeDistributeUrl = "score/course-score-stat.do";
        const string serviceResourceUrl = "service/res.do";

        /// <summary>
        /// 成绩读取限制条数。在 scorePostValue 中修改。
        /// </summary>
        public int RowLimit { get; set; } = 25;
        
        [ToFix("将获取GPA成绩改为并发逻辑")]
        public async Task Execute()
        {
            try
            {
                // Read Archive Score details
                var lastReport = await Core.App.Service.Post(serviceResourceUrl, scorePostValue);
                var ro = lastReport.ParseJSON<RootObject<ArchiveScoreValue>>();

                // Read score distribution details
                foreach (var asv in ro.value)
                {
                    var LastDetail = await Core.App.Service.Post(gradeDistributeUrl, $"{{\"asId\":\"{asv.asId}\"}}");
                    asv.distribute = LastDetail.ParseJSON<GradeDetails>();
                }

                // Read GPA details and add
                lastReport = await Core.App.Service.Post(serviceResourceUrl, gpaPostValue);
                Core.Configure.Write(configGpa, lastReport);
                GradePointViewModel.Instance.Clear();
                if (ro.value.Length > 0) GradePointViewModel.Instance.Add(ParseGPA(lastReport));

                // Save score details and add
                Core.Configure.Write(configGrade, ro.Serialize());
                GradePointViewModel.Instance.AddRange(ParseASV(ro));
            }
            catch (WebsException ex)
            {
                if (ex.Status != WebStatus.Timeout) throw;
                await GradePointViewModel.Instance.ShowTimeoutMessage();
            }
        }
        
        public static async Task PreloadData()
        {
            await Task.Yield();
            GradePointViewModel.Instance.Clear();

            var LastReportGPA = Core.Configure.Read(configGpa);
            if (LastReportGPA != "") GradePointViewModel.Instance.Add(ParseGPA(LastReportGPA));

            var LastReport = Core.Configure.Read(configGrade);
            if (LastReport != "") GradePointViewModel.Instance.AddRange(ParseASV(LastReport));
        }

        static IEnumerable<InsideGradeItem> ParseASV(string lastRead)
        {
            return ParseASV(lastRead.ParseJSON<RootObject<ArchiveScoreValue>>());
        }

        static IEnumerable<InsideGradeItem> ParseASV(RootObject<ArchiveScoreValue> roAsv)
        {
            return from asv in roAsv.value select new InsideGradeItem(asv);
        }

        static GPAItem ParseGPA(string lastReport)
        {
            var ro = lastReport.ParseJSON<RootObject<GPAValue>>();
            var str = string.Format("按首次成绩\n学分平均绩点 {0:N6}\n学分平均成绩 {1:N6}\n\n按最好成绩\n学分平均绩点 {2:N6}\n学分平均成绩 {3:N6}",
                ro.value[0].gpaFirst, ro.value[0].avgScoreFirst, ro.value[0].gpaBest, ro.value[0].avgScoreBest);
            return new GPAItem(str);
        }
    }
}
