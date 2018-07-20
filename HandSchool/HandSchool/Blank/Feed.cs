using HandSchool.Internal;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Threading.Tasks;
using static HandSchool.Internal.Helper;

namespace HandSchool.Blank
{
    [Entrance("RSS阅读器")]
    class FeedEntrance : IFeedEntrance
    {
        public string ScriptFileUri { get; }
        public bool IsPost => false;
        public string PostValue => string.Empty;
        public string StorageFile => "blank.feed.xml";
        public string LastReport { get; private set; } = string.Empty;
        public DateTime LastUpdate { get; private set; }

        public FeedEntrance(string url)
        {
            ScriptFileUri = url;
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
            LastReport = LastReport.Trim();
            Core.WriteConfig(StorageFile, LastReport);
            Core.WriteConfig(StorageFile + ".time", DateTime.Now.ToString());
            Parse();
        }

        public void Parse()
        {
            if (LastReport == "") return;
            var items = ParseRSS(LastReport);
            FeedViewModel.Instance.Items.Clear();
            foreach (var item in items) FeedViewModel.Instance.Items.Add(item);
        }
    }
}
