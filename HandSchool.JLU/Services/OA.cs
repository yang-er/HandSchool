﻿using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.ViewModels;
using HandSchool.Models;

[assembly: RegisterService(typeof(Oa))]
namespace HandSchool.JLU.Services
{
    /// <summary>
    /// 吉林大学的学校通知查询服务。
    /// </summary>
    /// <inheritdoc cref="IFeedEntrance" />
    [Entrance("JLU", "网上教务", "提供了吉林大学电子校务平台上的所有信息。")]
    [UseStorage("JLU")]
    internal sealed class Oa : IFeedEntrance
    {
        private string _baseUrl = "https://oa.jlu.edu.cn/";
        public Oa()
        {
            _lazy = new Lazy<IWebClient>(() =>
            {
                var wc = Core.New<IWebClient>();
                wc.Timeout = 5000;
                wc.StringBaseAddress = _baseUrl;
                return wc;
            });
        }
        public IWebClient WebClient => _lazy.Value;
        private readonly Lazy<IWebClient> _lazy;
        
        private async Task SearchByWord(string word, int page, bool reload)
        {
            await InnerExecute(
                $"defaultroot/PortalInformation!jldxList.action?searchId={word}&startPage={page}", reload);
        }

        private async Task Execute(int page, bool fp)
        {
            await InnerExecute(
                $"defaultroot/PortalInformation!jldxList.action?1=1&channelId=179577&startPage={page}", fp);
        }

        /// <summary>
        /// 给定一个Oa通知列表页面地址，解析出其中的内容并同步到UI
        /// </summary>
        /// <param name="oaPageUrl">通知列表页面地址</param>
        /// <param name="reload">是否清空已有项目</param>
        private async Task InnerExecute(string oaPageUrl, bool reload)
        {
            try
            {
                var lastReport = await WebClient.GetStringAsync(oaPageUrl);
                var dateString = DateTime.Now.ToString("yyyy/MM/dd-hh:mm:ss");
                var dataList = ParseOa(lastReport);

                Core.Platform.EnsureOnMainThread(() =>
                {
                    if (reload)
                    {
                        FeedViewModel.Instance.Clear();
                    }
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
        
        public Task Execute() => Execute(1, true);

        public Task Search(string word) => SearchByWord(word, 1, true);

        [ToFix("清理逻辑")]
        public async Task<int> Execute(int n)
        {
            if (n < 1) return 0;
            await Execute(n, n == 1);
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
                    var res = new DigResultValue
                    {
                        publishdate = time.InnerText.Replace("&nbsp;",""),
                        content = "",
                        depart = sender.InnerText,
                        title = title.InnerText.Replace("[置顶]",""),
                        link = $"/defaultroot/{title.GetAttributeValue("href", null)}"
                    };
                    var top = title.SelectSingleNode("./font");
                    res.flgtop = top != null;
                    ret.Add(res);
                }
                var page = html.DocumentNode.SelectNodes("//div[@class='pages']//a");
                var x = page.Last();
                var pa = x.GetAttributeValue("href", null);
                pageCount = int.Parse(pa.SubStr(pa.LastIndexOf("startPage=", StringComparison.Ordinal) + 10, pa.Length));
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