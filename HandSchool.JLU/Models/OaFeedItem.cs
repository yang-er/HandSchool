using System;
using System.Collections.Generic;
using System.Text;
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

        public OaFeedItem(DigResultValue rv)
        {
            Link = "https://oa.jlu.edu.cn" + rv.link;
            internalLink = rv.link;
            Category = rv.depart;
            PubDate = rv.publishdate;
            Title = rv.title;
            if (rv.flgtop) PubDate = "[置顶] " + PubDate;
        }

        public override async Task<string> GetDescriptionAsync()
        {
            var oa = Core.App.Feed as OA;
            var meta = new WebRequestMeta("webservice/m/api/getNewsDetail", WebRequestMeta.Json);
            var data = new KeyValueDict { { "link", internalLink } };
            var resp = await oa.WebClient.PostAsync(meta, data);
            var str = await resp.ReadAsStringAsync();
            var ro = str.ParseJSON<OaItemRootObject>();
            return ro.resultValue.content;
        }
    }
}
