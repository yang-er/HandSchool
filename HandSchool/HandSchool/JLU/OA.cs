using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    class OA : IFeedEntrance
    {
        public string Name => "网上教务";
        public string ScriptFileUri => "http://oa.52jida.com/feed";
        public bool IsPost => false;
        public string PostValue => string.Empty;
        public string StorageFile => "oa.jlu.xml";
        public string LastReport { get; private set; } = string.Empty;

        public OA()
        {
        }

        public async Task Execute()
        {
            LastReport = await App.Current.Service.WebClient.GetAsync(ScriptFileUri, "application/rss+xml");
            WriteConfFile(StorageFile, LastReport);
            Parse();
        }

        public void Parse()
        {
            var xdoc = XDocument.Parse(LastReport);
            var id = 0;
            var items = (from item in xdoc.Descendants("item")
                select new FeedItem
                {
                    Title = (string)item.Element("title"),
                    Description = (string)item.Element("description"),
                    PubDate = (string)item.Element("pubDate"),
                    Id = id++
                });
            foreach (var item in items) FeedViewModel.Instance.Items.Add(item);
        }
    }
}
