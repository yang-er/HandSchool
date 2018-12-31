using System;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 界面导航的视图模型，提供了视图服务。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    public class NavigationViewModel : BaseViewModel
    {
        static readonly Lazy<NavigationViewModel> Lazy =
            new Lazy<NavigationViewModel>(() => new NavigationViewModel());

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static NavigationViewModel Instance => Lazy.Value;

        /// <summary>
        /// 加载预定义的所有菜单内容。
        /// </summary>
        private NavigationViewModel()
        {
            FetchOptions();
        }

        /// <summary>
        /// 添加菜单入口点到主要页面中。
        /// </summary>
        /// <param name="title">入口点菜单的标题。</param>
        /// <param name="dest">目标页面的类名称，将通过反射创建实例。</param>
        /// <param name="category">学校命名空间，如果为空默认为全局类。</param>
        /// <param name="uwp">UWP 的图标。</param>
        /// <param name="ios">iOS 系统展示的图标。为空时收起到信息查询中。</param>
        public void AddMenuEntry(string title, string dest, string category = "", string uwp = "", string ios = "")
        {
            Core.Platform.AddMenuEntry(title, dest, category ?? "", uwp, ios);
        }
        
        /// <summary>
        /// 根据现有的预定义菜单内容添加入口点。
        /// </summary>
        public void FetchOptions()
        {
            Core.Platform.BeginMenu();

            AddMenuEntry("首页", "IndexPage", uwp: "\xE10F", ios: "tab_rec.png");
            AddMenuEntry("课程表", "SchedulePage", uwp: "\xECA5", ios: "tab_sched.png");

            if (Core.Platform.RuntimeName == "Android" &&
                Core.App.Loader.Feed != null &&
                Core.App.Loader.Message != null)
            {
                AddMenuEntry("消息通知", "MessageTabbedPage");
            }
            else
            {
                if (Core.App.Loader.Feed != null)
                    AddMenuEntry("学校通知", "FeedPage", uwp: "\xED0D", ios: "tab_feed.png");
                if (Core.App.Loader.Message != null)
                    AddMenuEntry("站内消息", "MessagePage", uwp: "\xE715");
            }

            if (Core.App.Loader.GradePoint != null)
                AddMenuEntry("学分成绩", "GradePointPage", uwp: "\xE82D");
            if (Core.App.InfoEntrances.Count > 0 || Core.Platform.RuntimeName == "iOS")
                AddMenuEntry("信息查询", "InfoQueryPage", uwp: "\xE946", ios: "tab_about.png");

            Core.Platform.FinalizeMenu();
        }
    }
}
