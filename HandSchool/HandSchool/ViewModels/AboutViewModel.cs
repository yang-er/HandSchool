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

        private AboutViewModel()
        {
            AboutEntrances.Add(new TapEntranceWrapper("开源项目", "", (nav) => Task.Run(() => OpenSource())));
#if !__IOS__
            AboutEntrances.Add(new TapEntranceWrapper("检查更新", "", (nav) => Task.Run(() => CheckUpdate())));
#endif
            AboutEntrances.Add(new TapEntranceWrapper("软件评分", "", (nav) => Task.Run(() => OpenMarket())));
            AboutEntrances.Add(new InfoEntranceWrapper(typeof(PrivacyPolicy)));
            AboutEntrances.Add(new InfoEntranceWrapper(typeof(LicenseInfo)));

            InfoEntrances.Add(AboutEntrances);

            Version = Core.Version;
            Title = "关于";
        }

        private void OpenSource()
        {
            Device.BeginInvokeOnMainThread(() => Device.OpenUri(new Uri(
                "https://github.com/yang-er/HandSchool"
            )));
        }

        private void OpenMarket()
        {
            Device.BeginInvokeOnMainThread(() => Device.OpenUri(new Uri(Core.OnPlatform(
                "https://www.coolapk.com/apk/com.x90yang.HandSchool",
                "itms-apps://itunes.apple.com/cn/app/zhang-shang-ji-da/id1439771819?mt=8",
                "ms-windows-store://review/?productid=9PD2FR9HHJQP"
            ))));
        }

        private void CheckUpdate()
        {
#if __ANDROID__
            Droid.MainActivity.UpdateManager.Update();
#elif __UWP__
            Device.OpenUri(new Uri("ms-windows-store://pdp/?productid=9PD2FR9HHJQP"));
#elif __IOS__
            OpenMarket();
#endif
        }

        [Entrance("隐私政策", "提供关于本程序如何使用您的隐私的一些说明。", EntranceType.UrlEntrance)]
        public class PrivacyPolicy : BaseController, IUrlEntrance
        {
            public string HtmlUrl { get; set; } = "privacy.html";
            public override Task Receive(string data) { return Task.Run(() => { }); }
            public byte[] OpenWithPost => null;
            public List<string> Cookie => null;
            public IUrlEntrance SubUrlRequested(string sub) { throw new InvalidOperationException(); }
        }

        [Entrance("开放源代码许可", "提供关于本程序开源许可证的一些说明。", EntranceType.UrlEntrance)]
        public class LicenseInfo : BaseController, IUrlEntrance
        {
            public string HtmlUrl { get; set; } = "license.html";
            public override Task Receive(string data) { return Task.Run(() => { }); }
            public byte[] OpenWithPost => null;
            public List<string> Cookie => null;
            public IUrlEntrance SubUrlRequested(string sub) { throw new InvalidOperationException(); }
        }
    }
}
