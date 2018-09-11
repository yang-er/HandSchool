using HandSchool.Internal;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace HandSchool.JLU
{
    [Entrance("网上教务")]
    class OA : IFeedEntrance
    {
        public string ScriptFileUri => "https://joj.chinacloudsites.cn/feed.xml";
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
            try
            {
                LastReport = "";
                using (var client = new AwaredWebClient("", System.Text.Encoding.UTF8))
                {
                    LastReport = await client.GetAsync(ScriptFileUri, "text/xml");
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                    System.Diagnostics.Debug.WriteLine("App not connected");
                else throw ex;
            }

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
                var items = LastReport.ParseRSS();
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
