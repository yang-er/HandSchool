using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        const string xslt = "<!DOCTYPE xsl:stylesheet [ <!ENTITY nbsp \"&#x00A0;\"> <!ENTITY middot \"&#x00B7;\"> ]>";

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

        private void SolveDiv(XElement div, StringBuilder sb)
        {
            foreach (var xn in div.Nodes())
            {
                switch (xn)
                {
                    case XElement xe when InlineElements.Contains(xe.Name):
                        sb.AppendLine(((string)xe).Replace('\x2003', ' ').TrimEnd());
                        break;
                    case XElement xe when xe.Name == "br":
                        sb.AppendLine();
                        break;
                    case XElement xe when xe.Name == "div":
                        SolveDiv(xe, sb);
                        break;
                    case XElement xe when xe.Name == "table":
                        sb.AppendLine("表格暂时无法显示。");
                        break;
                    case XElement xe when IgnoredElements.Contains(xe.Name):
                    case XComment _:
                        // This is a comment. we should ignore it.
                        break;
                    case XText xt:
                        sb.AppendLine(xt.Value.Replace("\n", " ").TrimEnd());
                        break;
                    case XElement xe:
                        throw new Exception("Error parsing document.\nNot implemented element: " + xe.Name);
                    default:
                        throw new Exception("Error parsing document.\nNot implemented node: " + xn.GetType());
                }
            }
        }

        public override async Task<string> GetDescriptionAsync()
        {
            if (!string.IsNullOrEmpty(content)) return content;

            var oa = Core.App.Feed as OA;
            var meta = new WebRequestMeta("http://202.98.18.57:18080/webservice/m/api/getNewsDetail", WebRequestMeta.Json);
            var data = new KeyValueDict { { "link", internalLink } };
            var resp = await oa.WebClient.PostAsync(meta, data);
            var str = await resp.ReadAsStringAsync();
            var sb = new StringBuilder();

            await Task.Run(() =>
            {
                try
                {
                    var ro = str.ParseJSON<OaItemRootObject>();
                    var toParse = xslt + ro.resultValue.content;
                    toParse = toParse.Replace("<o:p>", "<p>")
                                     .Replace("</o:p>", "</p>");
                    var doc = XDocument.Parse(toParse);
                    var contentDiv = doc.Elements().First().Elements().First();
                    SolveDiv(contentDiv, sb);
                }
                catch (Exception ex)
                {
                    sb.Clear();
                    sb.AppendLine("解析引擎出现错误，请联系开发者。可以通过右上角链接直接查看原文。");
                    sb.Append(ex);
                }
            });

            return content = sb.ToString().Trim();
        }
    }
}
