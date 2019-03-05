using HandSchool.Design;
using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;

namespace HandSchool.JLU.Services
{
    /// <summary>
    /// 吉林大学的学校通知查询服务。
    /// </summary>
    /// <inheritdoc cref="IFeedEntrance" />
    [Entrance("JLU", "网上教务", "提供了吉林大学电子校务平台上的所有信息。")]
    internal sealed class OA : IFeedEntrance
    {
        const string configOa = "jlu.oa.xml";
        const string configOaTime = "jlu.oa.xml.time";

        private IWebClient WebClient { get; }
        private IConfiguration Configure { get; }
        private ILogger Logger { get; }

        public OA(IWebClient webClient, IConfiguration configure, ILogger<OA> logger)
        {
            WebClient = webClient;
            WebClient.BaseAddress = "https://joj.chinacloudsites.cn/";
            Configure = configure;
            Logger = logger;
        }
        
        /// <summary>
        /// 内部执行数据的获取。
        /// </summary>
        /// <param name="file">文件名</param>
        /// <param name="fp">是否为第一页</param>
        /// <exception cref="ServiceException" />
        /// <returns>通知项的迭代器</returns>
        private async Task<IEnumerable<FeedItem>> InnerExecute(string file, bool fp)
        {
            try
            {
                var lastReport = await WebClient.GetStringAsync(file, "text/xml");
                if (lastReport == "") return new FeedItem[0];
                var dateString = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                if (fp)
                {
                    await Configure.SaveAsync(configOa, lastReport);
                    await Configure.SaveAsync(configOaTime, dateString);
                }

                Logger.Info("Feed updated at " + dateString);
                return lastReport.ParseRSS();
            }
            catch (WebsException ex)
            {
                throw new ServiceException(ex.Status.ToDescription(), ex);
            }
            catch (XmlException ex)
            {
                throw new ServiceException("解析数据出现错误。", ex);
            }
        }

        /// <summary>
        /// 获取第n页新闻。
        /// </summary>
        /// <param name="n">页号</param>
        /// <exception cref="ServiceException" />
        /// <returns>下次查询页号与此次获取到的内容</returns>
        public async Task<Tuple<int, IEnumerable<FeedItem>>> FetchAsync(int n)
        {
            int leftPage = 0;
            IEnumerable<FeedItem> feeds = new FeedItem[0];

            if (n == 1)
            {
                leftPage = 2;
                feeds = await InnerExecute("feed.xml", true);
            }
            else if (n == 2)
            {
                leftPage = 0;
                feeds = await InnerExecute("feed2.xml", false);
            }

            return new Tuple<int, IEnumerable<FeedItem>>(leftPage, feeds);
        }
        
        /// <summary>
        /// 从缓存中读取数据。
        /// </summary>
        /// <returns>缓存的内容，若没有则为null</returns>
        public async Task<IEnumerable<FeedItem>> FromCacheAsync()
        {
            var lu = await Configure.ReadAsync(configOaTime);
            bool timedOut = !DateTime.TryParse(lu, out var lastUpdate);
            if (!timedOut) timedOut = lastUpdate.AddHours(1) < DateTime.Now;
            if (timedOut) return null;

            try
            {
                return lu.ParseRSS();
            }
            catch
            {
                return new FeedItem[0];
            }
        }
    }
}