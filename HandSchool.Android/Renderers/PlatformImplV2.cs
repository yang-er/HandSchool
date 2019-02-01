using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HandSchool.Droid.Elements;
using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Views;
using SysEnv = System.Environment;

namespace HandSchool.Droid
{
    public sealed class PlatformImplV2 : PlatformBase
    {
        public static PlatformImplV2 Instance { get; private set; }

        public Context Context { get; }

        public UpdateManager UpdateManager { get; }

        private Stack<ViewResponseImpl> ImplStack { get; }

        public static List<NavMenuItemV2> NavigationItems { get; } = new List<NavMenuItemV2>();

        private static readonly Lazy<List<NavMenuItemV2>> LazySec =
            new Lazy<List<NavMenuItemV2>>(() => new List<NavMenuItemV2>
            {
                new NavMenuItemV2("设置", "SettingPage", ""),
                new NavMenuItemV2("关于", "AboutPage", "")
            });

        public static List<NavMenuItemV2> NavigationItemsSec => LazySec.Value;

        public override IViewResponseImpl ViewResponseImpl
        {
            get => ImplStack.Peek();
            protected set { /* No response! */ }
        }

        public void SetViewResponseImpl(ViewResponseImpl impl)
        {
            if (impl is null && ImplStack.Count > 0)
                ImplStack.Pop();
            else if (impl != null)
                ImplStack.Push(impl);
        }

        public PlatformImplV2(Context context)
        {
            Context = context;
            RuntimeName = "Android";
            StoreLink = "https://www.coolapk.com/apk/com.x90yang.HandSchool";
            ConfigureDirectory = SysEnv.GetFolderPath(SysEnv.SpecialFolder.Personal);
            Core.InitPlatform(Instance = this);
            Core.Reflection.RegisterType<AboutPage>();
            Core.Reflection.RegisterType<WebViewPage>();
            Core.Reflection.RegisterType<MessagePresenter>();
            ImplStack = new Stack<ViewResponseImpl>();
            UpdateManager = new UpdateManager(context);
        }
        
        public override void AddMenuEntry(string title, string dest, string category, string uwp, string ios)
        {
            NavigationItems.Add(new NavMenuItemV2(title, dest, category));
        }

        public override void CheckUpdate()
        {
            UpdateManager.Update(true, ImplStack.Peek().Context);
        }

        public override ILoginPage CreateLoginPage(LoginViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> ShowNewCurriculumPageAsync(CurriculumItem item, INavigate navigationContext)
        {
            throw new NotImplementedException();
        }
    }
}