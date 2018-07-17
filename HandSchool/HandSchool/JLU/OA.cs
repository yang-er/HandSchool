using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
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
        public string StorageFile => "jlu.oa.xml";
        public string LastReport { get; private set; } = string.Empty;
        public DateTime LastUpdate { get; private set; }

        public OA()
        {
            var lu = Core.ReadConfig(StorageFile + ".time");
            if (lu == "" || (LastUpdate = DateTime.Parse(lu)).AddHours(1).CompareTo(DateTime.Now) == -1)
            {
                Task.Run(Execute);
            }
            else
            {
                LastReport = Core.ReadConfig(StorageFile);
                Parse();
            }
        }

        public async Task Execute()
        {
            using (var client = new AwaredWebClient("", System.Text.Encoding.UTF8))
                LastReport = await client.GetAsync(ScriptFileUri, "application/rss+xml");
            if (LastReport == "") return;
            LastReport = LastReport.Substring(LastReport.IndexOf("<?xml ver"));
            Core.WriteConfig(StorageFile, LastReport);
            Core.WriteConfig(StorageFile + ".time", DateTime.Now.ToString());
            Parse();
        }

        public void Parse()
        {
            if (LastReport == "") return;
            try
            {
                var items = ParseRSS(LastReport);
                FeedViewModel.Instance.Items.Clear();
                foreach (var item in items) FeedViewModel.Instance.Items.Add(item);
            }
            catch (XmlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}
