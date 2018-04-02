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
        private int schId;
        
        public string Name => "学院介绍查询";
        public string Description => "查询各个学院的详细信息。";
        public List<string> TableHeader { get; set; }
        public Bootstrap HtmlDocument { get; set; }
        public string LastReport { get; private set; }
        public string StorageFile => "No storage";
        public bool IsPost => true;
        public string ScriptFileUri => "service/res.do";
        public string PostValue => $"{{\"tag\":\"school@schoolSearch\",\"branch\":\"byId\",\"params\":{{\"schId\":\"{schId}\"}}}}";

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

        public CollegeIntroduce()
        {
            HtmlDocument = new Bootstrap
            { Children = {
                new MasterDetail
                {
                    InfoGather = new Form
                    {
                        Children =
                        {
                            new Check { Title = "含公共教学中心" },
                            new Check { Title = "含其他机构" },
                            new Select
                            {
                                Title = "学部",
                                Options =
                                {
                                    { "0", "全部" },
                                    { "1", "人文学部" },
                                    { "2", "社会科学学部" },
                                    { "3", "理学部" },
                                    { "4", "工学部" },
                                    { "5", "信息科学学部" },
                                    { "6", "地球科学学部" },
                                    { "7", "白求恩医学部" },
                                    { "8", "农学部" }
                                }
                            },
                            new Select
                            {
                                Title = "校区",
                                Options =
                                {
                                    { "7", "任意校区" },
                                    { "0", "前卫校区" },
                                    { "1", "南岭校区" },
                                    { "2", "新民校区" },
                                    { "3", "朝阳校区" },
                                    { "4", "南湖校区" },
                                    { "5", "和平校区" },
                                    { "6", "基础园区" }
                                }
                            },
                            new Select { Title = "学院" }
                        }
                    },
                    Children =
                    {
                        new Div { Id = "result" }
                    }
                }
            }};
        }

        public async void Receive(string data)
        {
            await Task.Run(() => { });
            return;
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
