using Android.Content;
using HandSchool.Forms;
using HandSchool.Internal;
using System.Collections.Generic;
using Environment = System.Environment;

namespace HandSchool.Droid
{
    public sealed partial class PlatformImpl : PlatformFormsImpl
    {
        /// <summary>
        /// 平台实现的实例。
        /// </summary>
        public static PlatformImpl Instance { get; private set; }

        /// <summary>
        /// 缩放尺寸
        /// </summary>
        private static float Scale { get; set; } = 1;

        /// <summary>
        /// 安卓系统的上下文
        /// </summary>
        public Context Context { get; }

        /// <summary>
        /// 更新管理器
        /// </summary>
        public UpdateManager UpdateManager { get; }
        
        /// <summary>
        /// 系统导航内容
        /// </summary>
        public List<NavMenuItemImpl> NavigationItems { get; }

        /// <summary>
        /// 初始化平台相关的参数。
        /// </summary>
        /// <param name="context">安卓系统的上下文</param>
        public PlatformImpl(Context context)
        {
            Context = context;
            RuntimeName = "Android";
            Scale = context.Resources.DisplayMetrics.Density;
            StoreLink = "https://www.coolapk.com/apk/com.x90yang.HandSchool";
            ConfigureDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Core.InitPlatform(Instance = this);
            NavigationItems = new List<NavMenuItemImpl>();
            UpdateManager = new UpdateManager(context);
            UpdateManager.Update();
        }

        /// <summary>
        /// 添加菜单入口点到主要页面中。
        /// </summary>
        /// <param name="title">入口点菜单的标题。</param>
        /// <param name="dest">目标页面的类名称，将通过反射创建实例。</param>
        /// <param name="category">学校命名空间，如果为空默认为全局类。</param>
        /// <param name="uwp">UWP 的图标。</param>
        /// <param name="ios">iOS 系统展示的图标。为空时收起到信息查询中。</param>
        public override void AddMenuEntry(string title, string dest, string category, MenuIcon icon)
        {
            NavigationItems.Add(new NavMenuItemImpl(title, dest, category));
        }
        
        /// <summary>
        /// 检查应用程序更新。
        /// </summary>
        public override void CheckUpdate() => UpdateManager.Update(true);
        
        /// <summary>
        /// 将dp值转换为px值
        /// </summary>
        /// <param name="dpValue">dp值</param>
        /// <returns>px值</returns>
        public static int Dip2Px(float dpValue) => (int)(dpValue * Scale + 0.5f);

        /// <summary>
        /// 将px值转换为dp值
        /// </summary>
        /// <param name="pxValue">px值</param>
        /// <returns>dp值</returns>
        public static float Px2Dip(int pxValue) => pxValue / Scale;
    }
}
