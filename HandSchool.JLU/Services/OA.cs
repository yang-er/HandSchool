using HandSchool.Internal;
using HandSchool.JLU.Services;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

[assembly: RegisterService(typeof(OA))]
namespace HandSchool.JLU.Services
{
    [Entrance("JLU", "网上教务", "提供了吉林大学电子校务平台上的所有信息。")]
    [UseStorage("JLU", configOa, configOaTime)]
    internal sealed class OA : IFeedEntrance
    {
        const string configOa = "jlu.oa.xml";
        const string configOaTime = "jlu.oa.xml.time";
        const string feedUrl = "https://joj.chinacloudsites.cn/feed.xml";
        
        public static async Task PreloadData()
        {
            await Task.Yield();
            var lu = Core.Configure.Read(configOaTime);
            bool timedOut = !DateTime.TryParse(lu, out var lastUpdate);
            if (!timedOut) timedOut = lastUpdate.AddHours(1) < DateTime.Now;

            if (timedOut) await Core.App.Feed.Execute();
            else Parse(Core.Configure.Read(configOa));
        }

        public async Task Execute()
        {
            try
            {
                string lastReport;

                using (var client = new AwaredWebClient("", System.Text.Encoding.UTF8))
                {
                    lastReport = await client.GetAsync(feedUrl, "text/xml");
                }

                if (lastReport == "") return;
                Core.Configure.Write(configOa, lastReport);
                Core.Configure.Write(configOaTime, DateTime.Now.ToString(CultureInfo.InvariantCulture));
                Parse(lastReport);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                    Core.Log("App not connected");
                else throw;
            }
        }

        static void Parse(string feedXml)
        {
            if (feedXml == "") return;

            try
            {
                var items = feedXml.ParseRSS();
                FeedViewModel.Instance.Clear();
                FeedViewModel.Instance.AddRange(items);
            }
            catch (XmlException ex)
            {
                Core.Log(ex);
            }
        }
    }
}
