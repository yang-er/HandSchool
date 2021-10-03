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
using HandSchool.JLU.ViewModels;

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
            wc.GetThreadAddVpnCookie().Start();
            return wc;
        }
        
        private async Task SearchByWord(string word, int page, bool reload)
        {
            string domain = "https://oa.jlu.edu.cn/defaultroot/PortalInformation!jldxList.action?searchId=" + word + "&startPage=" + page;
            if (Loader.Vpn != null && Loader.Vpn.IsLogin)
            {
                domain = "https://vpns.jlu.edu.cn/https/77726476706e69737468656265737421fff60f962b2526557a1dc7af96/defaultroot/PortalInformation!jldxList.action?searchId=" + word + "&startPage=" + page;
            }
            var lastReport = await WebClient.GetStringAsync(domain);
            var dateString = DateTime.Now.ToString(CultureInfo.InvariantCulture);

            var dataList = ParseOa(lastReport);

            if (reload)
            {
                Core.Configure.Write(configOa, dataList.Serialize());
                Core.Configure.Write(configOaTime, dateString);
                FeedViewModel.Instance.Clear();
            }

            Core.Platform.EnsureOnMainThread(() =>
            {
                FeedViewModel.Instance.AddRange(from d in dataList.Item1 select new OaFeedItem(d));
                FeedViewModel.Instance.TotalPageCount = dataList.Item2;
                this.WriteLog("Feed updated at " + dateString);
            });
        }
        private async Task InnerExecute(int page, bool fp)
        {
            try
            {
                string domain = "https://oa.jlu.edu.cn";
                if (Loader.Vpn != null && Loader.Vpn.IsLogin)
                {
                    domain = "https://vpns.jlu.edu.cn/https/77726476706e69737468656265737421fff60f962b2526557a1dc7af96";
                }

                var lastReport = await WebClient.GetStringAsync(domain + "/defaultroot/PortalInformation!jldxList.action?1=1&channelId=179577&startPage=" + page);
                var dateString = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                
                var dataList = ParseOa(lastReport);
                if (fp)
                {
                    Core.Configure.Write(configOa, dataList.Serialize());
                    Core.Configure.Write(configOaTime, dateString);
                    FeedViewModel.Instance.Clear();
                }

                Core.Platform.EnsureOnMainThread(() =>
                {
                    FeedViewModel.Instance.AddRange(from d in dataList.Item1 select new OaFeedItem(d));
                    FeedViewModel.Instance.TotalPageCount = dataList.Item2;
                    this.WriteLog("Feed updated at " + dateString);
                });
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.NameResolutionFailure)
                    this.WriteLog("App not connected to network. Stop feeding.");
                else throw;
            }
        }

        public Task Execute() => InnerExecute(1, true);

        public Task Search(string word) => SearchByWord(word, 1, true);

        [ToFix("清理逻辑")]
        public async Task<int> Execute(int n)
        {
            if (n < 1) return 0;
            await InnerExecute(n, n == 1);
            return n;
        }

        public async Task<int> Search(string word ,int n)
        {
            if (n < 1) return 0;
            await SearchByWord(word, n, n == 1);
            return n;
        }
        
        static (List<DigResultValue>, int) ParseOa(string htmlSources)
        {
            var html = new HtmlAgilityPack.HtmlDocument();
            var ret = new List<DigResultValue>();
            int pageCount = 0;
            try
            {
                html.LoadHtml(htmlSources);
                var msgs = html.DocumentNode.SelectNodes("//div[@id='itemContainer']/div");
                foreach(var item in msgs)
                {
                    var title = item.SelectSingleNode("./a[@class='font14']");
                    var sender = item.SelectSingleNode("./a[@class='column']");
                    var time = item.SelectSingleNode("./span[@class='time']");
                    var res = new DigResultValue();
                    res.publishdate = time.InnerText.Replace("&nbsp;","");
                    res.content = "";
                    res.depart = sender.InnerText;
                    res.title = title.InnerText.Replace("[置顶]","");
                    res.link = "/defaultroot/" + title.GetAttributeValue("href", null);
                    var top = title.SelectSingleNode("./font");
                    res.flgtop = top != null;
                    ret.Add(res);
                }
                var page = html.DocumentNode.SelectNodes("//div[@class='pages']//a");
                var x = page.Last();
                var pa = x.GetAttributeValue("href", null);
                pageCount = int.Parse(pa.SubStr(pa.LastIndexOf("startPage=") + 10, pa.Length));
            }
            catch
            {
                ret.Clear();
                pageCount = 0;
            }
            return (ret, pageCount);

        }

    }
}