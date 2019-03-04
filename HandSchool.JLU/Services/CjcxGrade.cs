using HandSchool.Design;
using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.Models;
using HandSchool.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandSchool.JLU.Services
{
    /// <summary>
    /// 吉林大学的外网成绩查询服务。
    /// </summary>
    /// <inheritdoc cref="IGradeEntrance" />
    [Entrance("JLU", "成绩查询", "提供外网的成绩查询功能。")]
    internal sealed class CjcxGrade : IGradeEntrance
    {
        const string configGrade = "jlu.grade3.json";
        const string scriptUrl = "service_res.php";
        const string postValue = "{\"tag\":\"lessonSelectResult@oldStudScore\",\"params\":{\"xh\":\"00000000\"}}";
        
        private IConfigureProvider Configure { get; }
        private ISchoolSystem Connection { get; }

        public CjcxGrade(IConfigureProvider configure, ISchoolSystem connection)
        {
            Configure = configure;
            Connection = connection;
        }
        
        static IEnumerable<CJCXGradeItem> ParseCjcx(CJCXCJ roAsv)
        {
            return from asv in roAsv.items select new CJCXGradeItem(asv);
        }

        static IEnumerable<CJCXGradeItem> ParseCjcx(string src)
        {
            return ParseCjcx(src.ParseJSON<CJCXCJ>());
        }
        
        public async Task<IEnumerable<IGradeItem>> OnlineAsync()
        {
            try
            {
                var lastReport = await Connection.Post(scriptUrl, postValue);
                var retVal = ParseCjcx(lastReport);
                await Configure.SaveAsync(configGrade, lastReport);
                return retVal;
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
                var lastReport = await Configure.ReadAsync(configGrade);
                if (lastReport != "") returnSource.AddRange(ParseCjcx(lastReport));
            }
            catch (JsonException ex)
            {
                throw new ServiceException("数据解析出现问题。", ex);
            }

            return returnSource;
        }
    }
}
