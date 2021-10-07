using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
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
        const string xslt = "<!DOCTYPE xsl:stylesheet [ <!ENTITY nbsp \"&#x00A0;\"> <!ENTITY middot \"&#x00B7;\"> <!ENTITY times \"&#x00D7;\"> <!ENTITY divide \"&#x00F7;\"> <!ENTITY yen \"&#x00A5;\"> <!ENTITY trade \"&#x2122;\"> ]>";

        private static ISet<XName> InlineElements { get; } = new HashSet<XName>
        {
            "p", "span", "font", "h1", "h2", "h3", "h4", "h5", "h6", "a", "strong", "em", "i", "b", "sup"
        };

        private static ISet<XName> IgnoredElements { get; } = new HashSet<XName>
        {
            "img"
        };

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
                if (Loader.Vpn != null && Loader.Vpn.IsLogin)
                {
                    domain = domain.Replace("https://oa.jlu.edu.cn/", "https://vpns.jlu.edu.cn/https/77726476706e69737468656265737421fff60f962b2526557a1dc7af96/");
                }
                var data = await oa.WebClient.GetStringAsync(domain);
                content = Tools.AnalyzeHtmlToOaDetail(data);
                GC.Collect();
                return content;
            }
            catch(Exception error) { return error.Message; }
        }
    }
}
