using HandSchool.Design;
using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.Services;
using HandSchool.Models;
using HandSchool.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: RegisterService(typeof(UimsGrade))]
namespace HandSchool.JLU.Services
{
    /// <summary>
    /// 吉林大学的内网成绩查询服务。
    /// </summary>
    /// <inheritdoc cref="IGradeEntrance" />
    [Entrance("JLU", "成绩查询", "提供内网的成绩查询和查看成绩分布功能。")]
    [UseStorage("JLU", configGpa, configGrade)]
    internal sealed class UimsGrade : IGradeEntrance
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

        private IConfigureProvider Configure { get; }
        private ISchoolSystem Connection { get; }
        
        public UimsGrade(IConfigureProvider configure, ISchoolSystem connection)
        {
            Configure = configure;
            Connection = connection;
        }
        
        static IEnumerable<InsideGradeItem> ParseAsv(string lastRead)
        {
            return ParseAsv(lastRead.ParseJSON<RootObject<ArchiveScoreValue>>());
        }

        static IEnumerable<InsideGradeItem> ParseAsv(RootObject<ArchiveScoreValue> roAsv)
        {
            return from asv in roAsv.value select new InsideGradeItem(asv);
        }

        static GPAItem ParseGpa(string lastReport)
        {
            var ro = lastReport.ParseJSON<RootObject<GPAValue>>();
            var str =  "按首次成绩\n" +
                      $"学分平均绩点 {ro.value[0].gpaFirst:N6}\n" +
                      $"学分平均成绩 {ro.value[0].avgScoreFirst:N6}\n" +
                       "\n" +
                       "按最好成绩\n" +
                      $"学分平均绩点 {ro.value[0].gpaBest:N6}\n" +
                      $"学分平均成绩 {ro.value[0].avgScoreBest:N6}";
            return new GPAItem(str);
        }

        [ToFix("将获取GPA成绩改为并发逻辑")]
        public async Task<IEnumerable<IGradeItem>> OnlineAsync()
        {
            try
            {
                // Read Archive Score details
                var lastReport = await Connection.Post(serviceResourceUrl, scorePostValue);
                var ro = lastReport.ParseJSON<RootObject<ArchiveScoreValue>>();

                // Read score distribution details
                foreach (var asv in ro.value)
                {
                    var lastDetail = await Connection.Post(gradeDistributeUrl, $"{{\"asId\":\"{asv.asId}\"}}");
                    asv.distribute = lastDetail.ParseJSON<GradeDetails>();
                }

                // Read GPA details and add
                lastReport = await Connection.Post(serviceResourceUrl, gpaPostValue);
                await Configure.SaveAsync(configGpa, lastReport);
                await Configure.SaveAsync(configGrade, ro.Serialize());

                var returnSource = new List<IGradeItem>();
                if (ro.value.Length > 0) returnSource.Add(ParseGpa(lastReport));
                returnSource.AddRange(ParseAsv(ro));
                return returnSource;
            }
            catch (WebsException ex)
            {
                if (ex.Status != WebStatus.Timeout) throw;
                throw new ServiceException("连接超时，请重试。", ex);
            }
            catch (JsonException ex)
            {
                throw new ServiceException("数据解析出现问题。", ex);
            }
        }

        public async Task<IEnumerable<IGradeItem>> OfflineAsync()
        {
            var returnSource = new List<IGradeItem>();

            try
            {
                var lastReportGpa = await Configure.ReadAsync(configGpa);
                if (lastReportGpa != "") returnSource.Add(ParseGpa(lastReportGpa));

                var lastReport = await Configure.ReadAsync(configGrade);
                if (lastReport != "") returnSource.AddRange(ParseAsv(lastReport));
            }
            catch (JsonException ex)
            {
                throw new ServiceException("数据解析出现问题。", ex);
            }

            return returnSource;
        }
    }
}
