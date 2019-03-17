using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;

[assembly: RegisterService(typeof(OA))]
namespace HandSchool.JLU.Services
{
    /// <summary>
    /// 吉林大学的学校通知查询服务。
    /// </summary>
    /// <inheritdoc cref="IFeedEntrance" />
    [Entrance("JLU", "网上教务", "提供了吉林大学电子校务平台上的所有信息。")]
    [UseStorage("JLU", configOa, configOaTime)]
    internal sealed class OA : IFeedEntrance
    {
        const string configOa = "jlu.oa.xml";
        const string configOaTime = "jlu.oa.xml.time";

        public IWebClient WebClient => Lazy.Value;
        readonly Lazy<IWebClient> Lazy = new Lazy<IWebClient>(CreateWebClient);

        static IWebClient CreateWebClient()
        {
            var wc = Core.New<IWebClient>();
            wc.BaseAddress = "http://202.98.18.57:18080/";
            return wc;
        }
        
        public static async Task PreloadData()
        {
            await Task.Yield();
            var lu = Core.Configure.Read(configOaTime);
            bool timedOut = !DateTime.TryParse(lu, out var lastUpdate);
            if (!timedOut) timedOut = lastUpdate.AddHours(1) < DateTime.Now;

            if (timedOut)
            {
                await FeedViewModel.Instance.LoadItems(false);
                FeedViewModel.Instance.LeftPageCount = 2;
            }
            else
            {
                Parse(Core.Configure.Read(configOa));
                FeedViewModel.Instance.LeftPageCount = 2;
            }
        }

        private async Task InnerExecute(int page, bool fp)
        {
            try
            {
                var html = await WebClient.GetStringAsync("https://oa.jlu.edu.cn/defaultroot/PortalInformation!jldxList.action?1=1&channelId=179577&startPage=" + page);
                var requestMeta = new WebRequestMeta("webservice/m/api/getNewsList", WebRequestMeta.Json);

                var content = new KeyValueDict
                {
                    { "type", "179577" },
                    { "page", page.ToString() },
                    { "token", "NjFDREY3RDRGMUQzMUUxNEQyMEY3MjAwN0MzRDQ1QjIx" },
                };

                var resp = await WebClient.PostAsync(requestMeta, content);
                var lastReport = await resp.ReadAsStringAsync();
                if (lastReport == "") return;
                var dateString = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                if (fp) Core.Configure.Write(configOa, lastReport);
                if (fp) Core.Configure.Write(configOaTime, dateString);
                Parse(lastReport, fp);
                this.WriteLog("Feed updated at " + dateString);
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.NameResolutionFailure)
                    this.WriteLog("App not connected to network. Stop feeding.");
                else throw;
            }
        }

        public Task Execute() => InnerExecute(1, true);

        [ToFix("清理逻辑")]
        public async Task<int> Execute(int n)
        {
            if (n < 1)
            {
                return 0;
            }
            else if (n == 1)
            {
                await InnerExecute(1, true);
                return 2;
            }
            else if (n == 2)
            {
                await InnerExecute(2, false);
                return 0;
            }
            else
            {
                return 0;
            }
        }

        static void Parse(string feedXml, bool fp = true)
        {
            if (feedXml == "") return;

            try
            {
                var items = feedXml.ParseJSON<OaListRootObject>();
                if (fp) FeedViewModel.Instance.Clear();
                foreach (var item in items.resultValue)
                    FeedViewModel.Instance.Add(new OaFeedItem(item));
            }
            catch (Exception ex)
            {
                Core.Logger.WriteException(ex);
            }
        }
    }
}