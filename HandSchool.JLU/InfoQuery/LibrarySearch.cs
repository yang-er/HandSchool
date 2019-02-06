using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        const string OriginalUrl = "https://lib.jlu.xylab.fun/" +
            "sms/opac/search/showiphoneSearch.action";

        public string HtmlUrl { get; set; }
        public byte[] OpenWithPost => null;
        public List<string> Cookie => null;

        public IUrlEntrance SubUrlRequested(string sub)
        {
            return new LibrarySearch(sub);
        }

        public override Task Receive(string data)
        {
            this.WriteLog("Accidently received message <<<EOF\n" + data + "\nEOF;");
            return Task.CompletedTask;
        }

        public LibrarySearch() : this(OriginalUrl)
        {
            var cmd = new Xamarin.Forms.Command(async (o) => await LibraryRent.RequestRentInfo(o));

            Menu.Add(new HandSchool.Views.MenuEntry
            {
                Title = "我的借阅",
                UWPIcon = "\uE7BE",
                Command = cmd
            });
        }

        public LibrarySearch(string subUrl)
        {
            HtmlUrl = subUrl;
        }
    }
}