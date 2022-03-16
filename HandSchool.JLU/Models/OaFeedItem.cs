using System;
using System.Threading.Tasks;
using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Services;
using HandSchool.Models;

namespace HandSchool.JLU.Models
{
    internal class OaFeedItem : FeedItem
    {
        private readonly string internalLink;
        private string content = "";
        
        public OaFeedItem(DigResultValue rv)
        {
            Link = "https://oa.jlu.edu.cn" + rv.link;
            internalLink = rv.link;
            Category = rv.depart;
            PubDate = rv.publishdate;
            Title = rv.title;
            Top = rv.flgtop;
        }

        public override string SimplifiedCategory
        {
            get
            {
                if (Category.Contains("、")) return Category.Replace('、', '\n');
                return Category.Length > 11 ? $"{Category.Substring(0, 11)}..." : Category;
            }
        }

        public override async Task<string> GetDescriptionAsync()
        {
            if (!string.IsNullOrEmpty(content)) return content;

            var oa = Core.App.Feed as Oa;
            var domain = Link;

            try
            {
                domain = domain.Replace("https://oa.jlu.edu.cn/", "");
                var data = await oa.WebClient.GetStringAsync(domain);
                content = Tools.AnalyzeHtmlToOaDetail(data);
                GC.Collect();
                return content;
            }
            catch (Exception error)
            {
                return error.Message;
            }
        }
    }
}
