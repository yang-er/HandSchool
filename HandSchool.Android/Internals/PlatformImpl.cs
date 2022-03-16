using Android.Content;
using HandSchool.Droid.Internals;
using HandSchool.Internals;
using HandSchool.Pages;
using HandSchool.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using Android.OS;
using Google.Android.Material.Dialog;

namespace HandSchool.Droid
{
    public sealed class PlatformImplV2 : PlatformBase
    {
        public static PlatformImplV2 Instance { get; private set; }
        
        public UpdateManager UpdateManager { get; }
        

        public static List<NavMenuItemV2> NavigationItems { get; } = new List<NavMenuItemV2>();

        private static readonly Lazy<List<NavMenuItemV2>> LazySec =
            new Lazy<List<NavMenuItemV2>>(() => new List<NavMenuItemV2>
            {
                new NavMenuItemV2("设置", "SettingPage", "", MenuIcon.Settings),
                new NavMenuItemV2("关于", "AboutPage", "", MenuIcon.AboutUs)
            });

        public static List<NavMenuItemV2> NavigationItemsSec => LazySec.Value;

        private readonly List<Android.App.Activity> _activityStack;

        public IReadOnlyList<Android.App.Activity> ActivityStack
        {
            get
            {
                lock (((ICollection) _activityStack).SyncRoot)
                {
                    return _activityStack;
                }
            }
        }

        public void RegisterActivity(Android.App.Activity activity)
        {
            lock (((ICollection) _activityStack).SyncRoot)
            {
                RemoveActivity(activity);
                _activityStack.Add(activity);
            }
        }

        public void RemoveActivity(Android.App.Activity activity)
        {
            lock (((ICollection) _activityStack).SyncRoot)
            {
                if (_activityStack.Contains(activity))
                    _activityStack.Remove(activity);
            }
        }

        public Android.App.Activity PeekAliveActivity(bool force = true)
        {
            lock (((ICollection) _activityStack).SyncRoot)
            {
                int i;
                for (i = _activityStack.Count - 1; i >= 0; i--)
                {
                    var act = _activityStack[i];
                    if (act.IsDestroyed || act.IsFinishing) continue;
                    break;
                }

                if (i >= 0) return _activityStack[i];
                if(force) throw new InvalidOperationException("No context");
                return null;
            }
        }

        private PlatformImplV2(Context context)
        {
            StoreLink = "https://www.coolapk.com/apk/com.x90yang.HandSchool";
            ConfigureDirectory = context.FilesDir.AbsolutePath;
            Core.InitPlatform(Instance = this);

            Core.Reflection.RegisterConstructor<AboutPage>();
            Core.Reflection.RegisterConstructor<WebViewPage>();
            Core.Reflection.RegisterConstructor<LoginFragment>();
            Core.Reflection.RegisterConstructor<HttpClientImpl>();
            Core.Reflection.RegisterImplement<IDetailPage, DetailActivity>();
            Core.Reflection.RegisterImplement<ICurriculumPage, CurriculumDialog>();
            Core.Reflection.RegisterImplement<IWebViewPage, WebViewPage>();
            Core.Reflection.RegisterImplement<IWebClient, HttpClientImpl>();
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                Core.Reflection.RegisterImplement<ILoginPage, LoginFragment>();
            else Core.Reflection.RegisterImplement<ILoginPage, LoginPageImpl>();
            Core.Reflection.RegisterImplement<WebLoginPage, WebLoginPageImpl>();
            _activityStack = new List<Android.App.Activity>();
            UpdateManager = new UpdateManager(context.ApplicationContext);
            ViewResponseImpl = new ViewResponseImpl();
            Density = context.Resources.DisplayMetrics.Density;
        }

        public float Density { get; }

        public static void Register(Context context)
        {
            Instance ??= new PlatformImplV2(context);
        }

        public override void AddMenuEntry(string title, string dest, string category, MenuIcon icon)
        {
            NavigationItems.Add(new NavMenuItemV2(title, dest, category, icon));
        }

        public override void CheckUpdate()
        {
            var topAct = PeekAliveActivity(false);
            if (topAct == null) return;
            var waiting = Android.Views.View.Inflate(topAct, Resource.Layout.alert_waiting, null);
            var alert = new MaterialAlertDialogBuilder(topAct, Resource.Style.MaterialAlertDialog_Rounded)
                .SetTitle("正在检查更新")
                .SetCancelable(false)
                .SetView(waiting)
                .Create();
            alert.Show();

            new UpdateManager(topAct)
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
        }
    }
}
