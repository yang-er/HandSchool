using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Views;
using Xamarin.Forms;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU.InfoQuery
{
    class CollegeIntroduce : IInfoEntrance
    {
        private WebViewPage webpg;
        private RootObject<CollegeInfo> obj;
        private int schId = 101;
        
        public string Name => "学院介绍查询";
        public string Description => "查询各个学院的详细信息。";
        public List<string> TableHeader { get; set; }
        public Bootstrap HtmlDocument { get; set; }
        public string LastReport { get; private set; }
        public string StorageFile => "No storage";
        public bool IsPost => true;
        public string ScriptFileUri => "service/res.do";
        public string PostValue => $"{{\"tag\":\"school@schoolSearch\",\"branch\":\"byId\",\"params\":{{\"schId\":\"{schId}\"}}}}";
        public Dictionary<string, Command> Menu { get; set; } = new Dictionary<string, Command>();

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
            {
                Children =
                {
                    new MasterDetail
                    {
                        InfoGather = new Form
                        {
                            Children =
                            {
                                new FormGroup { Children = { new RawHtml { Raw = "<select class=\"form-control\" id=\"division\" onchange=\"getList()\"><option value=\"*\">全部学部</option><option value=\"1420\">人文学部</option><option value=\"1421\">社会科学学部</option><option value=\"1422\">理学部</option><option value=\"1423\">工学部</option><option value=\"1424\">信息科学学部</option><option value=\"1425\">地球科学学部</option><option value=\"1426\">白求恩医学部</option><option value=\"1428\">农学部</option></select>" } } },
                                new FormGroup { Children = { new RawHtml { Raw = "<select class=\"form-control\" id=\"campus\" onchange=\"getList()\"><option value=\"*\">任意校区</option><option value=\"1401\">前卫校区</option><option value=\"1402\">南岭校区</option><option value=\"1403\">新民校区</option><option value=\"1404\">朝阳校区</option><option value=\"1405\">南湖校区</option><option value=\"1406\">和平校区</option></select>" } } },
                                new FormGroup { Children = { new RawHtml { Raw = "<select class=\"form-control\" id=\"schId\">" +
                                    "<option value=\"174\" data-campus=\"1401\" data-part=\"1420\">哲学社会学院</option>" +
                                    "<option value=\"175\" data-campus=\"1401\" data-part=\"1420\">文学院</option>" +
                                    "<option value=\"104\" data-campus=\"1401\" data-part=\"1420\">外国语学院</option>" +
                                    "<option value=\"105\" data-campus=\"1401\" data-part=\"1420\">艺术学院</option>" +
                                    "<option value=\"106\" data-campus=\"1401\" data-part=\"1420\">体育学院</option>" +
                                    "<option value=\"182\" data-campus=\"1401\" data-part=\"1420\">新闻与传播学院</option>" +
                                    "<option value=\"107\" data-campus=\"1401\" data-part=\"1421\">经济学院</option>" +
                                    "<option value=\"108\" data-campus=\"1401\" data-part=\"1421\">法学院</option>" +
                                    "<option value=\"109\" data-campus=\"1401\" data-part=\"1421\">行政学院</option>" +
                                    "<option value=\"110\" data-campus=\"1401\" data-part=\"1421\">商学院</option>" +
                                    "<option value=\"111\" data-campus=\"1405\" data-part=\"1421\">马克思主义学院</option>" +
                                    "<option value=\"102\" data-campus=\"1401\" data-part=\"1421\">金融学院</option>" +
                                    "<option value=\"181\" data-campus=\"1401\" data-part=\"1421\">公共外交学院</option>" +
                                    "<option value=\"112\" data-campus=\"1401\" data-part=\"1422\">数学学院</option>" +
                                    "<option value=\"113\" data-campus=\"1401\" data-part=\"1422\">物理学院</option>" +
                                    "<option value=\"114\" data-campus=\"1401\" data-part=\"1422\">化学学院</option>" +
                                    "<option value=\"115\" data-campus=\"1401\" data-part=\"1422\">生命科学学院</option>" +
                                    "<option value=\"116\" data-campus=\"1402\" data-part=\"1423\">机械科学与工程学院</option>" +
                                    "<option value=\"117\" data-campus=\"1402\" data-part=\"1423\">汽车工程学院</option>" +
                                    "<option value=\"118\" data-campus=\"1402\" data-part=\"1423\">材料科学与工程学院</option>" +
                                    "<option value=\"119\" data-campus=\"1402\" data-part=\"1423\">交通学院</option>" +
                                    "<option value=\"120\" data-campus=\"1402\" data-part=\"1423\">生物与农业工程学院</option>" +
                                    "<option value=\"121\" data-campus=\"1402\" data-part=\"1423\">管理学院</option>" +
                                    "<option value=\"122\" data-campus=\"1401\" data-part=\"1424\">电子科学与工程学院</option>" +
                                    "<option value=\"123\" data-campus=\"1405\" data-part=\"1424\">通信工程学院</option>" +
                                    "<option value=\"100\" data-campus=\"1401\" data-part=\"1424\">计算机科学与技术学院</option>" +
                                    "<option value=\"101\" data-campus=\"1401\" data-part=\"1424\">软件学院</option>" +
                                    "<option value=\"124\" data-campus=\"1404\" data-part=\"1425\">地球科学学院</option>" +
                                    "<option value=\"125\" data-campus=\"1404\" data-part=\"1425\">地球探测科学与技术学院</option>" +
                                    "<option value=\"126\" data-campus=\"1404\" data-part=\"1425\">建设工程学院</option>" +
                                    "<option value=\"127\" data-campus=\"1404\" data-part=\"1425\">环境与资源学院</option>" +
                                    "<option value=\"128\" data-campus=\"1404\" data-part=\"1425\">仪器科学与电气工程学院</option>" +
                                    "<option value=\"103\" data-campus=\"1403\" data-part=\"1426\">白求恩医学院</option>" +
                                    "<option value=\"129\" data-campus=\"1403\" data-part=\"1426\">基础医学院</option>" +
                                    "<option value=\"130\" data-campus=\"1403\" data-part=\"1426\">公共卫生学院</option>" +
                                    "<option value=\"131\" data-campus=\"1403\" data-part=\"1426\">药学院</option>" +
                                    "<option value=\"132\" data-campus=\"1403\" data-part=\"1426\">护理学院</option>" +
                                    "<option value=\"133\" data-campus=\"1403\" data-part=\"1426\">第一临床医学院</option>" +
                                    "<option value=\"134\" data-campus=\"1403\" data-part=\"1426\">第二临床医学院</option>" +
                                    "<option value=\"135\" data-campus=\"1403\" data-part=\"1426\">第三临床医学院</option>" +
                                    "<option value=\"136\" data-campus=\"1403\" data-part=\"1426\">口腔医学院</option>" +
                                    "<option value=\"176\" data-campus=\"1403\" data-part=\"1426\">临床医学院</option>" +
                                    "<option value=\"137\" data-campus=\"1406\" data-part=\"1428\">畜牧兽医学院</option>" +
                                    "<option value=\"138\" data-campus=\"1406\" data-part=\"1428\">植物科学学院</option>" +
                                    "<option value=\"139\" data-campus=\"1406\" data-part=\"1428\">军需科技学院</option>" +
                                    "<option value=\"177\" data-campus=\"1406\" data-part=\"1428\">动物科学学院</option>" +
                                    "<option value=\"178\" data-campus=\"1406\" data-part=\"1428\">动物医学学院</option>" +
                                    "<option value=\"187\" data-campus=\"1406\" data-part=\"1423\">食品科学与工程学院</option>" +
                                    "</select>" } } }
                            }
                        },
                        Children =
                        {
                            new RawHtml { Raw = "<h4>名称</h4><p id=\"schoolName\">软件学院</p><h4>英文名称</h4><p id=\"englishName\">College of Software</p><h4>外部编号</h4><p id=\"extSchNo\">54</p><h4>校区</h4><p id=\"Icampus\">前卫校区</p><h4>学部</h4><p id=\"Idivision\">信息科学学部</p><h4>负责人</h4><p id=\"staff\">未设置</p><h4>联系电话</h4><p id=\"telephone\">学校很懒，什么也没有留下……</p><h4>院系主页</h4><p id=\"website\">学校很懒，什么也没有留下……</p><h4>院系介绍</h4><p id=\"introduction\">学校很懒，什么也没有留下……</p>" }
                        }
                    }
                },
                JavaScript =
                {
                    "function getList() { var campus = $('#campus').val(); var selector = '#schId option' + (campus == '*' ? '' : '[data-campus=\"' + campus + '\"]'); var division = $('#division').val(); selector += (division == '*' ? '' : '[data-part=\"' + division + '\"]'); $('#schId > option').wrap('<span>').hide(); $(selector).unwrap().show(); }; $('#schId').val('101'); function getSchId() { invokeCSharpAction('schId=' + $('#schId').val()); }; "
                }
            };
            Menu.Add("查询", new Command(() => webpg.WebView.JavaScript("getSchId()")));
        }

        public async void Receive(string data)
        {
            System.Diagnostics.Debug.WriteLine(data);
            if (data.StartsWith("schId"))
            {
                if (data == "schId==null")
                {
                    await Binding.DisplayAlert("信息查询", "错误：未指定查询学院。", "知道了");
                    return;
                }

                schId = int.Parse(data.Split('=')[1]);
                var loading = ShowLoadingAlert("信息查询中……");
                await Execute();
                loading.Invoke();
            }
            else
            {
                await Binding.DisplayAlert("信息查询", "错误：未定义操作。", "知道了");
                throw new NotImplementedException();
            }
        }
        
        public async Task Execute()
        {
            LastReport = await App.Current.Service.Post(ScriptFileUri, PostValue);
            obj = JSON<RootObject<CollegeInfo>>(LastReport);
            var jsBuilder = new StringBuilder();
            if (obj.value[0].schoolName == null) obj.value[0].schoolName = "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#schoolName').text('" + obj.value[0].schoolName + "');");
            if (obj.value[0].englishName == null) obj.value[0].englishName = "School is lazy, left nothing...";
            jsBuilder.Append("$('#englishName').text('" + obj.value[0].englishName + "');");
            if (obj.value[0].extSchNo == null) obj.value[0].extSchNo = "??";
            jsBuilder.Append("$('#extSchNo').text('" + obj.value[0].extSchNo + "');");
            if (obj.value[0].campus == null) obj.value[0].campus = "未知"; else obj.value[0].campus = (App.Current.Service as UIMS).Campus[obj.value[0].campus];
            jsBuilder.Append("$('#Icampus').text('" + obj.value[0].campus + "');");
            if (obj.value[0].division == null) obj.value[0].division = "未知"; else obj.value[0].division = (App.Current.Service as UIMS).Division[obj.value[0].division];
            jsBuilder.Append("$('#Idivision').text('" + obj.value[0].division + "');");
            if (obj.value[0].staff == null) obj.value[0].staff = "未设置";
            jsBuilder.Append("$('#staff').text('" + obj.value[0].staff + "');");
            if (obj.value[0].telephone == null) obj.value[0].telephone = "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#telephone').text('" + obj.value[0].telephone + "');");
            if (obj.value[0].website == null) obj.value[0].website = "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#website').text('" + obj.value[0].website + "');");
            if (obj.value[0].introduction == null) obj.value[0].introduction = "学校很懒，什么也没有留下……";
            jsBuilder.Append("$('#introduction').text('" + obj.value[0].introduction + "');");
            webpg.WebView.JavaScript(jsBuilder.ToString());
        }

        public void Parse() { }
    }
}
