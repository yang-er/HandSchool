using HandSchool.Internal;
using System;
using System.Reflection;
using Xamarin.Forms;

namespace HandSchool.Models
{
    /// <summary>
    /// 用于抽象系统导航数据的项目。
    /// </summary>
    /// <remarks>Thanks to 张高兴</remarks>
    /// <see cref="https://www.cnblogs.com/zhanggaoxing/p/7436523.html" />
    public class MasterPageItem : NotifyPropertyChanged
    {
        /// <summary>
        /// 一个判断是否被选中的谓词。
        /// </summary>
        public static Predicate<MasterPageItem> IsSelected = (item) => item.Selected;

        /// <summary>
        /// 创建一个系统导航项目，并提供页面延迟加载的功能。
        /// </summary>
        /// <param name="title">导航项目的名称。</param>
        /// <param name="dest">目标页面类型名。</param>
        /// <param name="icon">Segoe MDL2 Assets 图标。</param>
        /// <param name="select">是否已被选中。</param>
        /// <param name="category">类的父命名空间。</param>
        public MasterPageItem(string title, string dest, string icon = "", bool select = false, string category = "")
        {
            this.title = title;
            Icon = icon;

            if (category != "") category += ".";
            destpg_type = $"HandSchool.{category}Views.{dest}";
            DestinationPageType = Assembly.GetExecutingAssembly().GetType(destpg_type);
            selected = select;
            color = select ? active : inactive;
        }

        /// <summary>
        /// UWP显示的图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 目标页面类型元数据
        /// </summary>
        public Type DestinationPageType { get; }

        /// <summary>
        /// 目标页面
        /// </summary>
        public Page CorePage
        {
            get
            {
                if (_corePage is null)
                {
                    _corePage = Activator.CreateInstance(DestinationPageType) as Page;
                    if (_corePage is null) throw new InvalidOperationException("Error page requested: " + destpg_type);
#if __IOS__
                    if (_corePage is Views.PopContentPage popPg)
                    {
                        tableted = popPg.TabletEnabled && Device.Idiom == TargetIdiom.Tablet;
                    }
#endif
                }

                return _corePage;
            }
        }

        /// <summary>
        /// 目标导航页面
        /// </summary>
        public NavigationPage DestPage
        {
            get
            {
                if (_navPage is null)
                {
                    _navPage = new NavigationPage(CorePage)
                    {
                        Title = title,
                        Icon = AppleIcon
                    };
                }

                return _navPage;
            }
        }

        /// <summary>
        /// 目标平板模式页面
        /// </summary>
        public MasterDetailPage TabletPage
        {
            get
            {
#if __IOS__
                if (_tabletPage is null && tableted)
                {
                    _tabletPage = new Views.TabletPageImpl(_corePage as Views.PopContentPage)
                    {
                        Title = title,
                        Icon = AppleIcon
                    };
                }
#endif
                return _tabletPage;
            }
        }

        /// <summary>
        /// 真实的页面
        /// </summary>
        public Page RealPage
        {
            get 
            {
                CorePage.GetHashCode();
                return TabletPage ?? (Page)DestPage;
            }
        }

        /// <summary>
        /// 展示的标题
        /// </summary>
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool Selected
        {
            get => selected;
            set => SetProperty(ref selected, value, onChanged: () => Color = value ? active : inactive);
        }

        /// <summary>
        /// 展示的颜色
        /// </summary>
        public Color Color
        {
            get => color;
            set => SetProperty(ref color, value);
        }

        /// <summary>
        /// iOS的选项卡图标
        /// </summary>
        public FileImageSource AppleIcon { get; set; }

        private Color color;
        private bool selected = false;
        private bool tableted = false;
        private string title;
        private Page _corePage;
        private MasterDetailPage _tabletPage;
        private NavigationPage _navPage;
        private readonly string destpg_type;

        static Color active = Color.FromRgb(0, 120, 215);
        static Color inactive = Color.Black;

        public override string ToString() => title;
    }
}
