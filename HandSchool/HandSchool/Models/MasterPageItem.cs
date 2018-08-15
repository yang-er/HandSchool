using HandSchool.Internal;
using System;
using System.Reflection;
using Xamarin.Forms;

namespace HandSchool.Models
{
    /// <summary>
    /// 系统导航项目
    /// </summary>
    /// <remarks>Thanks to 张高兴</remarks>
    /// <see cref="https://www.cnblogs.com/zhanggaoxing/p/7436523.html" />
    public class MasterPageItem : NotifyPropertyChanged
    {
        static Color active = Color.FromRgb(0, 120, 215);
        static Color inactive = Color.Black;
        
        public MasterPageItem() { }

        /// <summary>
        /// 系统导航项目
        /// </summary>
        /// <param name="title">名称</param>
        /// <param name="dest">目标页面类型</param>
        /// <param name="icon">Segoe MDL2 Assets 图标</param>
        /// <param name="apple">苹果图标</param>
        /// <param name="select">是否已选中</param>
        public MasterPageItem(string title, string dest, string icon = "", bool select = false, string category = "")
        {
            this.title = title;
            string destpg_type;
            if (category != "") category += ".";
            Icon = icon;

            destpg_type = $"HandSchool.{category}Views.{dest}";
            DestinationPageType = Assembly.GetExecutingAssembly().GetType(destpg_type);

            selected = select;
            color = select ? active : inactive;
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 目标页面类型元数据
        /// </summary>
        public Type DestinationPageType { get; }

        /// <summary>
        /// 目标页面
        /// </summary>
        public Page CorePage => _destpg2 ?? (_destpg2 = Activator.CreateInstance(DestinationPageType) as Page);

        /// <summary>
        /// 目标导航页面
        /// </summary>
        public NavigationPage DestPage
        {
            get
            {
                System.Diagnostics.Debug.Assert(Core.RuntimePlatform == "Android", "Others do not use this function");
                return _destpg ?? (_destpg = new NavigationPage(CorePage)
                {
                    Title = title,
                    Icon = AppleIcon
                });
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
            set
            {
                SetProperty(ref selected, value);
                Color = value ? active : inactive;
            }
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
        /// iOS的Tab Icon
        /// </summary>
        public FileImageSource AppleIcon { get; set; }

        private Color color = new Color();
        private bool selected = false;
        private string title;
        private NavigationPage _destpg;
        private Page _destpg2;

        public override string ToString() => title;
    }
}
