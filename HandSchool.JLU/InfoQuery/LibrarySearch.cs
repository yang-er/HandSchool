using HandSchool.Design;
using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
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
        const string originalUrl = "https://lib.jlu.xylab.fun/" +
            "sms/opac/search/showiphoneSearch.action";

        public string HtmlUrl { get; set; }

        public IUrlEntrance SubUrlRequested(string sub)
        {
            return new LibrarySearch(sub);
        }

        public override Task Receive(string data)
        {
            Logger.Warn("Accidently received message <<<EOF\n" + data + "\nEOF;");
            return Task.CompletedTask;
        }

        private readonly Func<LibraryRent> libraryRentFactory;

        private async Task RentInfoAsync()
        {
            var rentSite = libraryRentFactory();
            if (!await rentSite.RequestLogin()) return;
            SendSubEntrance(new LibrarySearch(rentSite.GetLibraryRent()));
        }

        public LibrarySearch(ILogger<LibrarySearch> logger, Func<LibraryRent> factory) : this(originalUrl)
        {
            Logger = logger;
            libraryRentFactory = factory;

            Menu.Add(new HandSchool.Views.MenuEntry
            {
                Title = "我的借阅",
                UWPIcon = "\uE7BE",
                Command = new CommandAction(RentInfoAsync)
            });
        }

        private LibrarySearch(string subUrl)
        {
            HtmlUrl = subUrl;
        }
    }
}