using HandSchool.Internal.HtmlObject;
using System;
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
            await BindingContext.ShowMessage("嘻嘻", "你好像发现了什么彩蛋。", "知道了");
            System.Diagnostics.Debug.WriteLine("test mode 2");
            await BindingContext.ShowMessage("哈哈", "我不管，你要对我负责。", "知道了");
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
    }
}
