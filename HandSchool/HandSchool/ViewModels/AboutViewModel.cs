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
            AboutEntrances.Add(new TapEntranceWrapper("检查更新", "", (nav) => Task.Run(() => CheckUpdate())));
            AboutEntrances.Add(new TapEntranceWrapper("软件评分", "", (nav) => Task.Run(() => OpenMarket())));
            AboutEntrances.Add(new InfoEntranceWrapper(typeof(PrivacyPolicy)));
            AboutEntrances.Add(new InfoEntranceWrapper(typeof(LicenseInfo)));

            InfoEntrances.Add(AboutEntrances);
            
            Documents = new UnorderedList
            {

                Children =
                {
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
                    "<a href=\"https://github.com/yang-er\" target=\"_blank\">GitHub@yang-er</a>",
                    "<a href=\"https://github.com/miasakachenmo\" target=\"_blank\">GitHub@miasakachenmo</a>"
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
                    "<hgroup class=\"mt-3\">" +
                    "<h2>掌上校园 <span onclick=\"invokeCSharpAction('shop')\">#</span></h2>" +
                    $"<h5 onclick=\"invokeCSharpAction('test')\">HandSchool.{Core.RuntimePlatform} v{Core.Version}</h5>" +
                    "</hgroup>".ToRawHtml(),
                    "<p>一个致力于将各个学校不同的教务系统整合成一个手机app的项目。</p>".ToRawHtml(),
                    "<h4>适配模式</h4>".ToRawHtml(),
                    "<p>利用C#和Xamarin.Forms编写，可以运行在 UWP、Andorid、iOS 等平台上。<br>目前已经支持 UWP (1803+)、Android (5.0+)，iOS（10.0+）。<br>学校的接口是全部实现Interface，保证了可以更换学校的文档。<br>有些内容不方便通过Page呈现的，提供了可以通过js交互的WebView</p>".ToRawHtml(),
                    "<h4>贡献</h4>".ToRawHtml(),
                    "<h5>设计与适配</h5>".ToRawHtml(),
                    Developers,
                    "<h5>支持的学校</h5>".ToRawHtml(),
                    SupportedSchool,
                    "<h5>文档资料</h5>".ToRawHtml(),
                    Documents,
                    "<h4>开放源代码</h4>".ToRawHtml(),
                    "<p class=\"mb-3\">采用GPLv2协议，<a href=\"https://github.com/yang-er/HandSchool\" target=\"_blank\">查看源代码</a>。<br>欢迎Pull Request，反馈bug！</p>".ToRawHtml(),
                },
                Css = "*{-ms-user-select:none;-webkit-user-select:none;user-select:none;}"
            };
            
            Version = Core.Version;
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

        private async void TestMode()
        {
            Core.Log("test mode");
            await View.ShowMessage("嘻嘻", "你好像发现了什么彩蛋。", "知道了");
            Core.Log("test mode 2");
            await View.ShowMessage("哈哈", "我不管，你要对我负责。", "知道了");
            Core.Log("test mode 3");
        }

        private void OpenMarket()
        {
            Device.BeginInvokeOnMainThread(() => Device.OpenUri(new Uri(Core.OnPlatform(
                "https://www.coolapk.com/apk/com.x90yang.HandSchool",
                "https://github.com/yang-er/HandSchool", // "itms://itunes.apple.com/cn/app/jie-zou-da-shi/id493901993?mt=8",
                "ms-windows-store://review/?productid=9PD2FR9HHJQP"
            ))));
        }

        private void CheckUpdate()
        {
#if __ANDROID__
            Droid.MainActivity.Instance.Update();
#elif __UWP__
            Device.OpenUri(new Uri("ms-windows-store://pdp/?productid=9PD2FR9HHJQP"));
#elif __IOS__
            OpenMarket();
#endif
        }

        [Entrance("隐私政策", "提供关于本程序如何使用您的隐私的一些说明。", EntranceType.UrlEntrance)]
        public class PrivacyPolicy : IUrlEntrance
        {
            public string HtmlUrl { get; set; } = "privacy.html";
            public IViewResponse Binding { get; set; }
            public Action<string> Evaluate { get; set; }
            public List<InfoEntranceMenu> Menu { get; set; } = new List<InfoEntranceMenu>();
            public void Receive(string data) { }
            public IUrlEntrance SubUrlRequested(string sub) { throw new InvalidOperationException(); }
        }

        [Entrance("开放源代码许可", "提供关于本程序开源许可证的一些说明。", EntranceType.UrlEntrance)]
        public class LicenseInfo : IUrlEntrance
        {
            public string HtmlUrl { get; set; } = "license.html";
            public IViewResponse Binding { get; set; }
            public Action<string> Evaluate { get; set; }
            public List<InfoEntranceMenu> Menu { get; set; } = new List<InfoEntranceMenu>();
            public void Receive(string data) { }
            public IUrlEntrance SubUrlRequested(string sub) { throw new InvalidOperationException(); }
        }
    }
}
