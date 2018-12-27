using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("图书馆藏查询", "查一查想要的书在图书馆的位置吧~", EntranceType.UrlEntrance)]
    class LibrarySearch : BaseController, IUrlEntrance
    {
        const string OriginalUrl = "https://lib.jlu.xylab.fun/sms/opac/search/showiphoneSearch.action";

        public string HtmlUrl { get; set; }

        public byte[] OpenWithPost => null;
        public List<string> Cookie => null;

        public IUrlEntrance SubUrlRequested(string sub)
        {
            return new LibrarySearch(sub);
        }

        public override async Task Receive(string data)
        {
            await Task.Run(() => Core.Log(data));
        }

        public LibrarySearch() : this(OriginalUrl)
        {
            var cmd = new Command(async (o) => await RequestRentInfo(o));
            Menu.Add(new InfoEntranceMenu("我的借阅", cmd, "\uE7BE"));
        }

        private async Task RequestRentInfo(object o)
        {
            var rentInfo = new LibraryRent.LoginDispatcher();
            if (await rentInfo.RequestLogin())
            {
                var ops = rentInfo.GetLibraryRent();
                if (o is Action<IWebEntrance> entReq)
                {
                    entReq.Invoke(ops);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public LibrarySearch(string suburl)
        {
            HtmlUrl = suburl;
        }
    }
}
