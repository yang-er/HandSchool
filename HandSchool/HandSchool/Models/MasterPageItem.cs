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
            var destpg_type = $"HandSchool.{category}Views.{dest}";
            DestinationPageType = Assembly.GetExecutingAssembly().GetType(destpg_type);
            corePage = new Lazy<Page>(() => Activator.CreateInstance(DestinationPageType) as Page);
            navPage = new Lazy<NavigationPage>(() => new NavigationPage(CorePage) { Title = title, Icon = AppleIcon });

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
        public Page CorePage => corePage.Value;

        /// <summary>
        /// 目标导航页面
        /// </summary>
        public NavigationPage DestPage => navPage.Value;

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
        private string title;
        private readonly Lazy<Page> corePage;
        private readonly Lazy<NavigationPage> navPage;

        static Color active = Color.FromRgb(0, 120, 215);
        static Color inactive = Color.Black;

        public override string ToString() => title;
    }
}
