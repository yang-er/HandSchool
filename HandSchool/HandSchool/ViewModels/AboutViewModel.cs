using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        static AboutViewModel instance = null;
        int test_count = 10;
        UnorderedList Developers;
        UnorderedList Documents;
        UnorderedList SupportedSchool;
        public List<InfoEntranceGroup> InfoEntrances { get; set; } = new List<InfoEntranceGroup>();
        public InfoEntranceGroup AboutEntrances { get; set; } = new InfoEntranceGroup { GroupTitle = "关于" };
        public string Version { get; set; }
        public static AboutViewModel Instance
        {
            get
            {
                if (instance is null) instance = new AboutViewModel();
                return instance;
            }
        }
        
        public Bootstrap HtmlDocument { get; }

        private AboutViewModel()
        {
            AboutEntrances.Add(new InfoEntranceWrapper(typeof(UpadteCheck)));
            AboutEntrances.Add(new InfoEntranceWrapper(typeof(PrivacyPolicy)));
            AboutEntrances.Add(new InfoEntranceWrapper(typeof(MarkApp)));

           
            InfoEntrances.Add(AboutEntrances);


            Documents = new UnorderedList
            {
               
                Children =
                {
                    "drcom-generic @ drcoms",
                    "JLULife @ brady",
                    "CookieAwareWebClient @ zhleiyang",
                    "PopContentPage @ shanhongyue",
                    "Xamarin.Forms @ xamarin",
                    "Docs @ microsoft",
                    "Bootstrap v4.0",
                    "etc..."
                }
            };

            Developers = new UnorderedList
            {
                Children =
                {
                    "<a href=\"https://github.com/yang-er\" target=\"_blank\">Github @ yang-er</a>",
                    "<a href=\"https://github.com/miasakachenmo\" target=\"_blank\">Github @ miasakachenmo</a>"
                }
            };

            SupportedSchool = new UnorderedList
            {
                Children =
                {
                    "吉林大学 UIMS"
                }
            };

            HtmlDocument = new Bootstrap
            {
                Children =
                {
                    (RawHtml) $"<hgroup class=\"mt-3\"><h2>掌上校园 <span onclick=\"invokeCSharpAction('shop')\">#</span></h2><h5 onclick=\"invokeCSharpAction('test')\">HandSchool.{Core.RuntimePlatform} v{Core.Version}</h5></hgroup>",
                    (RawHtml) "<p>一个致力于将各个学校不同的教务系统整合成一个手机app的项目。</p>",
                    (RawHtml) "<h4>适配模式</h4>",
                    (RawHtml) "<p>利用C#和Xamarin.Forms编写，可以运行在 UWP、Andorid、iOS 等平台上。<br>",
                    (RawHtml) "目前已经支持 UWP (1803+)、Android (5.0+)，iOS缺少适配，但是可以基本运行。<br>",
                    (RawHtml) "学校的接口是全部实现Interface，保证了可以更换学校的文档。<br>",
                    (RawHtml) "有些内容不方便通过Page呈现的，提供了可以通过js交互的WebView</p>",
                    (RawHtml) "<h4>贡献</h4>",
                    (RawHtml) "<h5>设计与适配</h5>",
                    Developers,
                    (RawHtml) "<h5>支持的学校</h5>",
                    SupportedSchool,
                    (RawHtml) "<h5>文档资料</h5>",
                    Documents,
                    (RawHtml) "<h4>开放源代码</h4>",
                    (RawHtml) "<p class=\"mb-3\">采用GPLv2协议，<a href=\"https://github.com/yang-er/HandSchool\" target=\"_blank\">查看源代码</a>。<br>",
                    (RawHtml) "欢迎Pull Request，反馈bug！</p>",
                },
                Css = "*{-ms-user-select:none;-webkit-user-select:none;user-select:none;}"
            };
#if __ANDROID__
            Version = Droid.MainActivity.ActivityContext.PackageManager.GetPackageInfo(Droid.MainActivity.ActivityContext.PackageName, 0).VersionName;
#endif
            //TODO 苹果版本号
            Title = "关于";
        }

        public void Response(string value)
        {
            if (value.StartsWith("open="))
            {
                Device.OpenUri(new Uri(value.Substring(5)));
            }
            else if (value == "test")
            {
                if ((++test_count - 10) % 20 == 10)
                {
                    TestMode();
                }
            }
            else if (value == "shop")
            {
                OpenMarket();
            }
        }

        async void TestMode()
        {
            System.Diagnostics.Debug.WriteLine("test mode");
            await View.ShowMessage("嘻嘻", "你好像发现了什么彩蛋。", "知道了");
            System.Diagnostics.Debug.WriteLine("test mode 2");
            await View.ShowMessage("哈哈", "我不管，你要对我负责。", "知道了");
            System.Diagnostics.Debug.WriteLine("test mode 3");
        }

        void OpenMarket()
        {
#if __ANDROID__
            // Device.OpenUri(new Uri("market://details?id=com.x90yang.HandSchool"));
#elif __IOS__
            // Device.OpenUri(new Uri("itms://itunes.apple.com/cn/app/jie-zou-da-shi/id493901993?mt=8"));
#elif __UWP__
            Device.OpenUri(new Uri("ms-windows-store://review/?productid=9PD2FR9HHJQP"));
#endif
        }

        [Entrance("隐私政策", "提供关于本程序如何使用您的隐私的一些说明。", EntranceType.UrlEntrance)]
        public class PrivacyPolicy : IUrlEntrance
        {
            public string HtmlUrl { get; set; } = "privacy.html";
            public IViewResponse Binding { get; set; }
            public Action<string> Evaluate { get; set; }
            public List<InfoEntranceMenu> Menu { get; set; }= new List<InfoEntranceMenu>();
            public void Receive(string data) { }
        }

        [Entrance("检查更新", "检查更新", EntranceType.UrlEntrance)]
        public class UpadteCheck : IUrlEntrance
        {
            public string HtmlUrl { get; set; }
            public IViewResponse Binding { get; set; }
            public Action<string> Evaluate { get; set; }
            public List<InfoEntranceMenu> Menu { get; set; }
            public void Receive(string data) { }
        }

        [Entrance("软件评分", "软件评分", EntranceType.UrlEntrance)]
        public class  MarkApp : IUrlEntrance
        {
#if __ANDROID__
            public string HtmlUrl { get; set; } = "https://www.coolapk.com/apk/com.x90yang.HandSchool";
#elif __IOS__
            public string HtmlUrl { get; set; } = "https://www.coolapk.com/apk/com.x90yang.HandSchool";
#elif __UWP__
            public string HtmlUrl { get; set; } = "ms-windows-store://review/?productid=9PD2FR9HHJQP";
#endif
            public IViewResponse Binding { get; set; }
            public Action<string> Evaluate { get; set; }
            public List<InfoEntranceMenu> Menu { get; set; } = new List<InfoEntranceMenu>();
            public void Receive(string data) { }
        }
    }
}
