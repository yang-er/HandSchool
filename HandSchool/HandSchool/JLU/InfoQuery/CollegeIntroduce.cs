using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Views;

namespace HandSchool.JLU.InfoQuery
{
    class CollegeIntroduce : IInfoEntrance
    {
        private WebViewPage webpg;
        private RootObject<CollegeInfo> obj;
        
        public string Name => "学院介绍查询";
        public string Description => "查询各个学院的详细信息。";
        public List<string> TableHeader { get; set; }
        public Bootstrap HtmlDocument { get; set; }
        public string LastReport { get; private set; }
        public string StorageFile => "No storage";
        public bool IsPost => true;
        public string ScriptFileUri => "service/res.do";
        public string PostValue => "{\"tag\":\"school@schoolSearch\",\"branch\":\"byId\",\"params\":{\"schId\":\"{schId}\"}}";

        public WebViewPage Binding
        {
            get => webpg;
            set
            {
                if (value is null)
                {
                    webpg.WebView.Cleanup();
                    webpg = null;
                }
                else
                {
                    webpg = value;
                    value.WebView.RegisterAction(Receive);
                }
            }
        }

        public void Receive(string data)
        {
            throw new NotImplementedException();
        }
        
        public async Task Execute()
        {
            throw new NotImplementedException();
        }

        public void Parse()
        {
            throw new NotImplementedException();
        }
    }
}
