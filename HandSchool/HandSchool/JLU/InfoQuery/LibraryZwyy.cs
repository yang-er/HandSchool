using HandSchool.Services;
using HandSchool.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("图书馆座位预约", "美好的一天从自习开始~", EntranceType.UrlEntrance)]
    class LibraryZwyy : BaseController, IUrlEntrance
    {
        const string LibZwyyJluEduCn = "http://libzwyy.jlu.edu.cn";

        public string HtmlUrl { get; set; }
        public byte[] OpenWithPost => null;
        public List<string> Cookie => null;
        
        public override async Task Receive(string data)
        {
            Core.Log(data); await Task.CompletedTask;
        }

        public IUrlEntrance SubUrlRequested(string sub)
        {
            return new LibraryZwyy(sub);
        }

        public LibraryZwyy(string url)
        {
            HtmlUrl = url;
        }

        public LibraryZwyy() : this(LibZwyyJluEduCn) { }
    }
}
