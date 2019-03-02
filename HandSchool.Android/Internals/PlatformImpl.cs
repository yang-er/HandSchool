using Android.Content;
using HandSchool.Internals;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using SysEnv = System.Environment;

namespace HandSchool.Droid
{
    public sealed class PlatformImplV2 : PlatformBase
    {
        public static PlatformImplV2 Instance { get; private set; }
        
        public UpdateManager UpdateManager { get; }

        private List<Context> ContextStack { get; }

        public static List<NavMenuItemV2> NavigationItems { get; } = new List<NavMenuItemV2>();

        private static readonly Lazy<List<NavMenuItemV2>> LazySec =
            new Lazy<List<NavMenuItemV2>>(() => new List<NavMenuItemV2>
            {
                new NavMenuItemV2("设置", "SettingPage", "", MenuIcon.Settings),
                new NavMenuItemV2("关于", "AboutPage", "", MenuIcon.AboutUs)
            });

        public static List<NavMenuItemV2> NavigationItemsSec => LazySec.Value;
        
        public void SetContext(Context impl)
        {
            ContextStack.Add(impl);
        }

        public void RemoveContext(Context impl)
        {
            if (ContextStack.Contains(impl))
                ContextStack.Remove(impl);
        }

        public Context PeekContext(bool force = true)
        {
            if (force && ContextStack.Count == 0)
                throw new InvalidOperationException("No context");
            return ContextStack.Count == 0 ? null : ContextStack[ContextStack.Count - 1];
        }

        private PlatformImplV2(Context context)
        {
            RuntimeName = "Android";
            StoreLink = "https://www.coolapk.com/apk/com.x90yang.HandSchool";
            ConfigureDirectory = SysEnv.GetFolderPath(SysEnv.SpecialFolder.Personal);
            Core.InitPlatform(Instance = this);

            Core.Reflection.RegisterCtor<AboutPage>();
            Core.Reflection.RegisterCtor<IndexPage>();
            Core.Reflection.RegisterCtor<WebViewPage>();
            Core.Reflection.RegisterCtor<LoginFragment>();
            Core.Reflection.RegisterCtor<HttpClientImpl>();
            Core.Reflection.RegisterType<DetailPage, DetailActivity>();
            Core.Reflection.RegisterType<ICurriculumPage, CurriculumDialog>();
            Core.Reflection.RegisterType<IWebViewPage, WebViewPage>();
            Core.Reflection.RegisterType<IWebClient, HttpClientImpl>();
            Core.Reflection.RegisterType<ILoginPage, LoginFragment>();
            ContextStack = new List<Context>();
            UpdateManager = new UpdateManager(context.ApplicationContext);
            ViewResponseImpl = new ViewResponseImpl();
            Density = context.Resources.DisplayMetrics.Density;
        }

        public float Density { get; }

        public static void Register(Context context)
        {
            if (Instance is null)
                new PlatformImplV2(context);
        }
        
        public override void AddMenuEntry(string title, string dest, string category, MenuIcon icon)
        {
            NavigationItems.Add(new NavMenuItemV2(title, dest, category, icon));
        }

        public override void CheckUpdate()
        {
            UpdateManager.Update(true, PeekContext());
        }
    }
}