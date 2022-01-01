using Android.Content;
using AndroidX.AppCompat.App;
using HandSchool.Droid.Internals;
using HandSchool.Internals;
using HandSchool.Pages;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SysEnv = System.Environment;

namespace HandSchool.Droid
{
    public sealed class PlatformImplV2 : PlatformBase
    {
        public static PlatformImplV2 Instance { get; private set; }
        
        public UpdateManager UpdateManager { get; }

        public List<Context> ContextStack { get; }

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
            return ContextStack.Count == 0 ? null : ContextStack[^1];
        }

        private PlatformImplV2(Context context)
        {
            RuntimeName = Device.Android;
            StoreLink = "https://www.coolapk.com/apk/com.x90yang.HandSchool";
            ConfigureDirectory = SysEnv.GetFolderPath(SysEnv.SpecialFolder.Personal);
            Core.InitPlatform(Instance = this);

            Core.Reflection.RegisterCtor<AboutPage>();
            Core.Reflection.RegisterCtor<WebViewPage>();
            Core.Reflection.RegisterCtor<LoginFragment>();
            Core.Reflection.RegisterCtor<HttpClientImpl>();
            Core.Reflection.RegisterType<DetailPage, DetailActivity>();
            Core.Reflection.RegisterType<ICurriculumPage, CurriculumDialog>();
            Core.Reflection.RegisterType<IWebViewPage, WebViewPage>();
            Core.Reflection.RegisterType<IWebClient, HttpClientImpl>();
            Core.Reflection.RegisterType<ILoginPage, LoginFragment>();
            Core.Reflection.RegisterType<WebLoginPage, WebLoginPageImpl>();
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
            if (ContextStack.Count == 0) return;
            var MainAct = PeekContext();
            if (MainAct == null) return;
            var waiting = Android.Views.View.Inflate(MainAct, Resource.Layout.alert_waiting, null);
            var alert = new AlertDialog.Builder(MainAct)
                .SetTitle("正在检查更新")
                .SetCancelable(false)
                .SetView(waiting)
                .Create();
            alert.Show();

            new UpdateManager(MainAct)
                .CheckUpdate()
                .ContinueWith(async (x) =>
                {
                    var res = await x;
                    Core.Platform.EnsureOnMainThread(() =>
                    {
                        alert.Dismiss();
                        res.Show();
                    });
                });
            return;
        }
    }
}
