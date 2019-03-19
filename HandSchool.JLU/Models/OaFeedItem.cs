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
        const string xslt = "<!DOCTYPE xsl:stylesheet [ <!ENTITY nbsp \"&#x00A0;\"> ]>";

        public OaFeedItem(DigResultValue rv)
        {
            Link = "https://oa.jlu.edu.cn" + rv.link;
            internalLink = rv.link;
            Category = rv.depart;
            PubDate = rv.publishdate;
            Title = rv.title;
            Top = rv.flgtop;
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
                    var doc = XDocument.Parse(xslt + ro.resultValue.content);
                    var contentDiv = doc.Elements().First().Elements().First();

                    foreach (var xn in contentDiv.Nodes())
                    {
                        if (xn is XElement xe)
                        {
                            if (xe.Name == "p" || xe.Name == "span")
                            {
                                sb.AppendLine(((string)xe).Replace('\x2003', ' '));
                            }
                            else if (xe.Name == "br")
                            {
                                sb.AppendLine();
                            }
                            else
                            {
                                throw new Exception("Error parsing document.\nNot implemented element: " + xe.Name);
                            }
                        }
                        else if (xn is XText xt)
                        {
                            sb.AppendLine(xt.Value.Replace("\n", " ").TrimEnd());
                        }
                        else if (xn is XComment)
                        {
                            // This is a comment. we should ignore it.
                        }
                        else
                        {
                            throw new Exception("Error parsing document.\nNot implemented node: " + xn.GetType());
                        }
                    }
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
