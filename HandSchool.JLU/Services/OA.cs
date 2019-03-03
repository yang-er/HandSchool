using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;

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

        private IWebClient WebClient { get; }

        public OA(IWebClient webClient)
        {
            WebClient = webClient;
            WebClient.BaseAddress = "https://joj.chinacloudsites.cn/";


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

        private async Task InnerExecute(string file, bool fp)
        {
            try
            {
                var lastReport = await WebClient.GetStringAsync(file, "text/xml");
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

        public Task Execute() => InnerExecute("feed.xml", true);

        [ToFix("清理逻辑")]
        public async Task<int> Execute(int n)
        {
            if (n < 1)
            {
                return 0;
            }
            else if (n == 1)
            {
                await InnerExecute("feed.xml", true);
                return 2;
            }
            else if (n == 2)
            {
                await InnerExecute($"feed{n}.xml", false);
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
                var items = feedXml.ParseRSS();
                if (fp) FeedViewModel.Instance.Clear();
                FeedViewModel.Instance.AddRange(items);
            }
            catch (XmlException ex)
            {
                Core.Logger.WriteException(ex);
            }
        }

        /// <summary>
        /// 获取第n页新闻。
        /// </summary>
        /// <param name="n">页号</param>
        /// <returns>下次查询页号与此次获取到的内容</returns>
        public async Task<Tuple<int, IEnumerable<FeedItem>>> FetchAsync(int n)
        {
            int leftPage = 0;
            IEnumerable<FeedItem> feeds = new FeedItem[0];

            if (n == 0)
            {
                return new Tuple<int, IEnumerable<FeedItem>>(0, new FeedItem[0]);
            }

            return new Tuple<int, IEnumerable<FeedItem>>(leftPage, feeds);
        }
    }
}