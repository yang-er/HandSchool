using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("图书馆藏查询", "查一查想要的书在图书馆的位置吧~", EntranceType.UrlEntrance)]
    class LibrarySearch : BaseController, IUrlEntrance
    {
        const string OriginalUrl = "http://202.198.25.5:8080/sms/opac/search/showiphoneSearch.action";

        public string HtmlUrl { get; set; }

        public IUrlEntrance SubUrlRequested(string sub)
        {
            return new LibrarySearch(sub);
        }

        public override async Task Receive(string data)
        {
            await Task.Run(() => Core.Log(data));
        }

        public LibrarySearch() : this(OriginalUrl) { }

        public LibrarySearch(string suburl)
        {
            HtmlUrl = suburl;
        }
    }
}
