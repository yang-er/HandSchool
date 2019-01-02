using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.JLU.InfoQuery
{
    /// <summary>
    /// 实现图书馆藏查询的功能。
    /// </summary>
    /// <inheritdoc cref="BaseController" />
    /// <inheritdoc cref="IUrlEntrance" />
    [Entrance("JLU", "图书馆藏查询", "查一查想要的书在图书馆的位置吧~", EntranceType.UrlEntrance)]
    internal class LibrarySearch : BaseController, IUrlEntrance
    {
        const string OriginalUrl = "https://lib.jlu.xylab.fun/sms/opac/search/showiphoneSearch.action";

        public string HtmlUrl { get; set; }
        public byte[] OpenWithPost => null;
        public List<string> Cookie => null;

        public IUrlEntrance SubUrlRequested(string sub)
        {
            return new LibrarySearch(sub);
        }

        public override Task Receive(string data)
        {
            Core.Log(data);
            return Task.CompletedTask;
        }

        public LibrarySearch() : this(OriginalUrl)
        {
            var cmd = new Command(async (o) => await LibraryRent.RequestRentInfo(o));
            Menu.Add(new InfoEntranceMenu("我的借阅", cmd, "\uE7BE"));
        }

        public LibrarySearch(string subUrl)
        {
            HtmlUrl = subUrl;
        }
    }
}
