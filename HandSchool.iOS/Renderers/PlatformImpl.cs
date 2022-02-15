using System;
using System.Collections.Generic;
using System.IO;
using HandSchool.Internals;
using HandSchool.iOS.Internals;
using HandSchool.Models;
using HandSchool.Pages;
using HandSchool.ViewModels;
using HandSchool.Views;

namespace HandSchool.iOS
{
    public sealed partial class PlatformImpl : PlatformBase
    {
        public const string UIViewControllerRequest = "HandSchool.iOS.UIVCReq";

        public override void CheckUpdate() => OpenUrl(StoreLink);

        public List<List<NavMenuItemImpl>> NavigationMenus { get; }

        public List<NavMenuItemImpl> MainNavigationMenu { get; }
        public InfoEntranceGroup InfoQueryMenu { get; }

        public static PlatformImpl Instance => Lazy.Value;

        private static readonly Lazy<PlatformImpl> Lazy = new Lazy<PlatformImpl>(() => new PlatformImpl());

        private PlatformImpl()
        {
            StoreLink = "itms-apps://itunes.apple.com/cn/app/zhang-shang-ji-da/id1439771819?mt=8";
            ConfigureDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
            Core.InitPlatform(this);
            MainNavigationMenu = new List<NavMenuItemImpl>();
            NavigationMenus = new List<List<NavMenuItemImpl>> {MainNavigationMenu};
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
            Core.Reflection.RegisterType<WebLoginPage, WebLoginPageImpl>();
            Core.Reflection.RegisterType<ICurriculumPage, CurriculumPage>();
        }
        
        public static void Register()
        {
            Instance.GetType();
        }

        private void MenuComplete(object sender, EventArgs args)
        {
            NavigationMenus.Add(
                new List<NavMenuItemImpl>()
                {
                    new NavMenuItemImpl("设置", "SettingPage", "", MenuIcon.Settings) {IsSingleInstance = true},
                    new NavMenuItemImpl("关于", "AboutPage", "", MenuIcon.AboutUs) {IsSingleInstance = true}
                });

            if (InfoQueryMenu.Count > 0)
                Core.App.InfoEntrances.Insert(0, InfoQueryMenu);
        }

        public override void AddMenuEntry(string title, string dest, string category, MenuIcon icon)
        {
            MainNavigationMenu.Add(new NavMenuItemImpl(title, dest, category, icon){IsSingleInstance = true});
        }
    }
}