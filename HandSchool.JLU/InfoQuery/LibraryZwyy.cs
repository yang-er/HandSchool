using HandSchool.Internals;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.JLU.InfoQuery
{
    class LibraryZwyy : BaseController, IUrlEntrance
    {
        const string LibZwyyJluEduCn = "http://libzwyy.jlu.edu.cn";

        public string HtmlUrl { get; set; }
        public byte[] OpenWithPost => null;
        public List<string> Cookie => null;
        
        public override async Task Receive(string data)
        {
            this.WriteLog("Unexpected value received <<<EOF\n" + data + "\nEOF;");
            await Task.CompletedTask;
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
