using System;
using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.Services;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

[assembly: RegisterService(typeof(GradeEntrance))]
namespace HandSchool.JLU.Services
{
    /// <summary>
    /// 吉林大学的内网成绩查询服务。
    /// </summary>
    /// <inheritdoc cref="IGradeEntrance" />
    [Entrance("JLU", "成绩查询", "提供内网的成绩查询和查看成绩分布功能。")]
    [UseStorage("JLU", ConfigGpa, ConfigGrade, ConfigAllGrade)]
    internal sealed class GradeEntrance : IGradeEntrance
    {
        const string ConfigGrade = "jlu.grade.json";
        const string ConfigGpa = "jlu.gpa.json";
        private const string ConfigAllGrade = "jlu.gpa.all_grade.json";

        const string GpaPostValue = "{\"type\":\"query\",\"res\":\"stat-avg-gpoint\",\"params\":{\"studId\":`studId`}}";
        const string NewerScorePostValue = "{\"tag\":\"archiveScore@queryCourseScore\",\"branch\":\"latest\"}";

        private const string AllScorePostValue =
            "{\"tag\":\"scoreBook@queryScoreStore\",\"branch\":\"self\",\"params\":{}}";
        const string GradeDistributeUrl = "score/course-score-stat-stud.do";
        const string ServiceResourceUrl = "service/res.do";

        [ToFix("将获取GPA成绩改为并发逻辑")]
        public async Task Execute()
        {
            try
            {
                // Read Archive Score details
                var lastReport = await Core.App.Service.Post(ServiceResourceUrl, NewerScorePostValue);
                var ro = lastReport.ParseJSON<RootObject<ArchiveScoreValue>>();

                // Read score distribution details
                foreach (var asv in ro.value)
                {
                    var lastDetail = await Core.App.Service.Post(GradeDistributeUrl, $"{{\"asId\":\"{asv.asId}\"}}");
                    asv.distribute = lastDetail.ParseJSON<GradeDetails>();
                }
                // Save score details and add
                Core.Configure.Write(ConfigGrade, ro.Serialize());
                var newerGradeItems = ParseNewerScore(ro);

                Core.Platform.EnsureOnMainThread(() =>
                {
                    GradePointViewModel.Instance.NewerGradeItems.Clear();
                    GradePointViewModel.Instance.NewerGradeItems.AddRange(newerGradeItems);
                });
            }
            catch (WebsException ex)
            {
                if (ex.Status != WebStatus.Timeout) throw;
                await GradePointViewModel.Instance.ShowTimeoutMessage();
            }
        }

        public async Task EntranceAll()
        {
            try
            {
                var gpaInfo = await Core.App.Service.Post(ServiceResourceUrl, GpaPostValue);
                Core.Configure.Write(ConfigGpa, gpaInfo);
                var gpaItem = ParseGpa(gpaInfo);
                Core.Platform.EnsureOnMainThread(() =>
                {
                    GradePointViewModel.Instance.AllGradeItems.Clear();
                    if (gpaItem != null)
                        GradePointViewModel.Instance.AllGradeItems.Add(gpaItem);
                });

                var allScore = await Core.App.Service.Post(ServiceResourceUrl, AllScorePostValue);
                Core.Configure.Write(ConfigAllGrade, allScore);
                var allScoreItems = ParseAllScore(allScore)?.ToList();
                Core.Platform.EnsureOnMainThread(() =>
                {
                    GradePointViewModel.Instance.AllGradeItems.AddReverse(allScoreItems);
                });
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
            var gpaCache = Core.Configure.Read(ConfigGpa);
            var gpaItem = ParseGpa(gpaCache);
            var newerScoreCache = Core.Configure.Read(ConfigGrade);
            var newerScoreItems = ParseNewerScore(newerScoreCache);
            var allScoreCache = Core.Configure.Read(ConfigAllGrade);
            var allScoreItems = ParseAllScore(allScoreCache)?.ToList();
            
            Core.Platform.EnsureOnMainThread(() =>
            {
                GradePointViewModel.Instance.NewerGradeItems.Clear(); 
                GradePointViewModel.Instance.AllGradeItems.Clear();
                GradePointViewModel.Instance.NewerGradeItems.AddRange(newerScoreItems);
                if (gpaItem != null) GradePointViewModel.Instance.AllGradeItems.Add(gpaItem);
                GradePointViewModel.Instance.AllGradeItems.AddReverse(allScoreItems);
            });
        }

        static IEnumerable<InsideGradeItem> ParseNewerScore(string lastRead)
        {
            if (string.IsNullOrWhiteSpace(lastRead)) return null;
            return ParseNewerScore(lastRead.ParseJSON<RootObject<ArchiveScoreValue>>());
        }

        static IEnumerable<InsideGradeItem> ParseNewerScore(RootObject<ArchiveScoreValue> roNewer)
        {
            return from asv in roNewer.value select new InsideGradeItem(asv);
        }

        class QueryGradeItem : IBasicGradeItem
        {
            private readonly QueryScoreValue asv;

            public string Title => asv.course.courName;
            public string FirstScore => asv.firstScore;
            public string HighestScore => asv.bestScore;
            public string FirstPoint => asv.firstGpoint;
            public string HightestPoint => asv.bestGpoint;
            public string Type => AlreadyKnownThings.Type5Name(asv.type5);
            public string Credit => asv.credit;
            public bool IsPassed => asv.isPass == "Y";
            public string Term => asv.firstTerm.termName;
            public Color TypeColor => AlreadyKnownThings.Type5Color(asv.type5);

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendLine(IsPassed ? $"已通过" : "未通过")
                    .Append("最初成绩：").Append(FirstScore)
                    .Append($"{(int.TryParse(FirstScore, out var ___) ? "分" : "")} | ").Append("绩点：")
                    .AppendLine(FirstPoint)
                    .Append("最好成绩：").Append(HighestScore)
                    .Append($"{(int.TryParse(FirstScore, out var ____) ? "分" : "")} | ").Append("绩点：")
                    .AppendLine(HightestPoint)
                    .Append("学时：").Append(asv.classHour).Append(" | 学分：").Append(Credit);
                return sb.ToString();
            }

            private string _detail;
            public string Detail => _detail ??= ToString();
            public QueryGradeItem(QueryScoreValue asv) => this.asv = asv;
        }
        static IEnumerable<IBasicGradeItem> ParseAllScore(string allScoreJson)
        {
            if (string.IsNullOrWhiteSpace(allScoreJson)) return null;
            var jo = JsonConvert.DeserializeObject<JObject>(allScoreJson);
            var values = jo?["value"]?.ToObject<List<QueryScoreValue>>();
            return values?.Select(v => new QueryGradeItem(v));
        }

        static GPAItem ParseGpa(string lastReport)
        {
            if(string.IsNullOrWhiteSpace(lastReport)) return null;
            var ro = lastReport.ParseJSON<RootObject<GPAValue>>();
            var str = ro.value[0].HasNull ? 
                "没有GPA信息（可能是新生）" :
                $"按首次成绩\n学分平均绩点 {ro.value[0].gpaFirst ?? 0.0:N6}\n学分平均成绩 {ro.value[0].avgScoreFirst ?? 0.0:N6}\n\n按最好成绩\n学分平均绩点 {ro.value[0].gpaBest ?? 0.0:N6}\n学分平均成绩 {ro.value[0].avgScoreBest ?? 0.0:N6}";
            return new GPAItem(str);
        }
    }
}
