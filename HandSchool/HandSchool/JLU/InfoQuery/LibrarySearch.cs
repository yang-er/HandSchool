using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("图书馆藏查询", "查一查想要的书在图书馆的位置吧~", EntranceType.UrlEntrance)]
    class LibrarySearch : IUrlEntrance
    {
        public string HtmlUrl { get; set; }
        public IViewResponse Binding { get; set; }
        public Action<string> Evaluate { get; set; }
        public List<InfoEntranceMenu> Menu { get; set; }

        public IUrlEntrance SubUrlRequested(string sub)
        {
            return new LibrarySearch(sub);
        }

        public void Receive(string data)
        {
            Core.Log(data);
        }

        public LibrarySearch()
        {
            HtmlUrl = "http://202.198.25.5:8080/sms/opac/search/showiphoneSearch.action";
            Menu = new List<InfoEntranceMenu>();
        }

        public LibrarySearch(string suburl)
        {
            HtmlUrl = suburl;
            Menu = new List<InfoEntranceMenu>();
        }
    }
}
