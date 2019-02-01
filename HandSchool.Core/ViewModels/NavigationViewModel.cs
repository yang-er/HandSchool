using HandSchool.Internal;
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
        /// 结束添加时执行
        /// </summary>
        public static event EventHandler FetchComplete;

        /// <summary>
        /// 加载预定义的所有菜单内容。
        /// </summary>
        private NavigationViewModel()
        {
            FetchOptions();
            FetchComplete?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 添加菜单入口点到主要页面中。
        /// </summary>
        /// <param name="title">入口点菜单的标题。</param>
        /// <param name="dest">目标页面的类名称，将通过反射创建实例。</param>
        /// <param name="category">学校命名空间，如果为空默认为全局类。</param>
        /// <param name="icon">菜单的图标。</param>
        public void AddMenuEntry(string title, string dest, string category, MenuIcon icon)
        {
            Core.Platform.AddMenuEntry(title, dest, category ?? "", icon);
        }
        
        /// <summary>
        /// 根据现有的预定义菜单内容添加入口点。
        /// </summary>
        public void FetchOptions()
        {
            AddMenuEntry("首页", "IndexPage", null, MenuIcon.Index);
            AddMenuEntry("课程表", "SchedulePage", null, MenuIcon.Schedule);

            if (Core.Platform.RuntimeName == "Android" &&
                Core.App.Loader.Feed != null &&
                Core.App.Loader.Message != null)
            {
                AddMenuEntry("消息通知", "MessagePresenter", null, MenuIcon.Feed);
            }
            else
            {
                if (Core.App.Loader.Feed != null)
                    AddMenuEntry("学校通知", "FeedPage", null, MenuIcon.Feed);
                if (Core.App.Loader.Message != null)
                    AddMenuEntry("站内消息", "MessagePage", null, MenuIcon.Message);
            }

            if (Core.App.Loader.GradePoint != null)
                AddMenuEntry("学分成绩", "GradePointPage", null, MenuIcon.GradeChart);
            if (Core.App.InfoEntrances.Count > 0 || Core.Platform.RuntimeName == "iOS")
                AddMenuEntry("信息查询", "InfoQueryPage", null, MenuIcon.InfoQuery);
        }
    }
}