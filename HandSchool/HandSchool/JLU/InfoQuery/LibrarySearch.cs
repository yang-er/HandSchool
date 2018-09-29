using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using System.Collections.Generic;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("图书馆藏查询", "查一查想要的书在图书馆的位置吧~", EntranceType.UrlEntrance)]
    class LibrarySearch : BaseController, IUrlEntrance
    {
        public string HtmlUrl { get; set; }

        public IUrlEntrance SubUrlRequested(string sub)
        {
            return new LibrarySearch(sub);
        }

        public override void Receive(string data)
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
