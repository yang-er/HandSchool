using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
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
            wc.Timeout = 5000;
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
                var pc = ParseCache(Core.Configure.Read(configOa));
                FeedViewModel.Instance.AddRange(from d in pc select new OaFeedItem(d));
                FeedViewModel.Instance.LeftPageCount = 2;
            }
        }

        private async Task InnerExecute(int page, bool fp)
        {
            try
            {
                var uims = Core.App.Service as UIMS;
                string domain = "https://oa.jlu.edu.cn";
                if (uims.UseVpn)
                {
                    domain = "https://vpns.jlu.edu.cn/https/77726476706e69737468656265737421fff60f962b2526557a1dc7af96";
                    WebClient.Cookie.Add(new Uri("https://vpns.jlu.edu.cn"), new System.Net.Cookie("remember_token", Loader.Vpn.RememberToken, "/"));
                }

                var lastReport = await WebClient.GetStringAsync(domain + "/defaultroot/PortalInformation!jldxList.action?1=1&channelId=179577&startPage=" + page);
                lastReport = lastReport.Substring(lastReport.IndexOf(@"<div id=""itemContainer"">") + 24);
                lastReport = lastReport.Substring(0, lastReport.IndexOf("</div>"));
                lastReport = lastReport.Replace("    ", "")
                                       .Replace("\r", "")
                                       .Replace("\n", "")
                                       .Replace("\t", "");
                lastReport = Regex.Replace(lastReport, @"<img\b[^>]*>", "");

                if (lastReport == "") return;
                var dateString = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                var dataList = ParseOa(lastReport).ToList();

                if (fp)
                {
                    Core.Configure.Write(configOa, dataList.Serialize());
                    Core.Configure.Write(configOaTime, dateString);
                    FeedViewModel.Instance.Clear();
                }

                FeedViewModel.Instance.AddRange(from d in dataList select new OaFeedItem(d));
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
            if (n < 1) return 0;
            await InnerExecute(n, n == 1);
            return n + 1;
        }
        
        static IEnumerable<DigResultValue> ParseOa(string feedXml)
        {
            try
            {
                var rootNode = "<root>" + 
                               feedXml.Replace("<font class='red'>[置顶]</font>", "<font class=\"red\">[置顶]</font>")
                                      .Replace("&", "&amp;")
                                      .Replace("&amp;nbsp;", "")
                                      .Replace("<a ", "<A ")
                                      .Replace("<span ", "<SPAN ")
                               + "</root>";
                var vp = XDocument.Parse(rootNode).Root;

                return
                    from item in vp.Descendants("DIV")
                    let a2 = item.Element("A")

                    select new DigResultValue
                    {
                        publishdate = (string) item.Element("SPAN"),
                        content = "",
                        depart = (string) item.Elements("A").Last(),
                        title = ((XText) a2.LastNode).Value,
                        link = "/defaultroot/" + a2.Attribute("href").Value,
                        flgtop = a2.FirstNode is XElement
                    };
            }
            catch (Exception ex)
            {
                Core.Logger.WriteException(ex);
                return new DigResultValue[0];
            }
        }

        static IEnumerable<DigResultValue> ParseCache(string cacheJson)
        {
            try
            {
                return cacheJson.ParseJSON<List<DigResultValue>>();
            }
            catch
            {
                return new DigResultValue[0];
            }
        }
    }
}