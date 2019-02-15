using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Foundation;
using HandSchool.Internal;
using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Views;
using UIKit;

namespace HandSchool.iOS
{
    public sealed partial class PlatformImpl : PlatformBase
    {
        public const string UIViewControllerRequest = "HandSchool.iOS.UIVCReq";

        public override void CheckUpdate() => OpenUrl(StoreLink);

        public List<NavMenuItemImpl> NavigationMenu { get; }

        public InfoEntranceGroup InfoQueryMenu { get; }

        public static PlatformImpl Instance { get; private set; }

        private PlatformImpl()
        {
            RuntimeName = "iOS";
            StoreLink = "itms-apps://itunes.apple.com/cn/app/zhang-shang-ji-da/id1439771819?mt=8";
            ConfigureDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
            Core.InitPlatform(Instance = this);
            NavigationMenu = new List<NavMenuItemImpl>();
            InfoQueryMenu = new InfoEntranceGroup("其他功能");
            NavigationViewModel.FetchComplete += MenuComplete;
            ViewResponseImpl = new ViewResponseImpl();

            Core.Reflection.RegisterCtor<HttpClientImpl>();
            Core.Reflection.RegisterCtor<CurriculumPage>();
            Core.Reflection.RegisterCtor<LoginPage>();
            Core.Reflection.RegisterCtor<AboutPage>();
            Core.Reflection.RegisterCtor<WebViewPage>();
            // Core.Reflection.RegisterCtor<CurriculumPage>();
            Core.Reflection.RegisterType<IWebClient, HttpClientImpl>();
            Core.Reflection.RegisterType<IWebViewPage, WebViewPage>();
            Core.Reflection.RegisterType<ILoginPage, LoginPage>();
            Core.Reflection.RegisterType<ICurriculumPage, CurriculumPage>();
        }

        public static void Register()
        {
            new PlatformImpl();
        }

        private void MenuComplete(object sender, EventArgs args)
        {
            var settingList = new InfoEntranceGroup("设置")
            {
                new TapEntranceWrapper("设置", "调整程序运行的参数。",
                    (nav) => nav.PushAsync<SettingPage>()),
                new TapEntranceWrapper("关于", "程序的版本信息、开发人员、许可证和隐私声明等。",
                    (nav) => nav.PushAsync<AboutPage>()),
            };

            Core.App.InfoEntrances.Add(settingList);
            if (InfoQueryMenu.Count > 0)
                Core.App.InfoEntrances.Insert(0, InfoQueryMenu);
        }

        public override void AddMenuEntry(string title, string dest, string category, MenuIcon icon)
        {
            string ios = NavMenuItemImpl.IconList[(int)icon];
            if (ios is null) InfoQueryMenu.Add(new NavMenuItemImpl(title, dest, category, icon).AsEntrance());
            else NavigationMenu.Add(new NavMenuItemImpl(title, dest, category, icon));
        }
    }
}