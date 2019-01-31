using HandSchool.Forms;
using HandSchool.Models;
using HandSchool.Views;
using System;
using Xamarin.Forms;

namespace HandSchool.Droid
{
    public class NavMenuItemImpl : NavigationMenuItem
    {
        private Lazy<Page> LazyCorePage { get; }
        private Lazy<NavigationPage> LazyNavPage { get; }
        private IViewPresenter Presenter { get; }
        private Forms.NavigateImpl Navigation { get; set; }
        static readonly Color active = Color.FromRgb(0, 120, 215);
        static readonly Color inactive = default(Color);
        private Color color;
        private bool selected = false;

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

        public NavigationPage OutsidePage => LazyNavPage.Value;

        private NavigationPage CreateNavigationPage()
        {
            var navPage = new NavigationPage(LazyCorePage.Value);
            Navigation = new Forms.NavigateImpl(navPage);
            Navigation.NavigationOccured(this, new NavigationEventArgs(LazyCorePage.Value));
            return navPage;
        }

        private Page Create1()
        {
            return Presenter.GetAllPages()[0] as Page;
        }

        private Page Create2()
        {
            var tabPage = new TabbedPage();
            tabPage.Title = Presenter.Title;
            var pages = Presenter.GetAllPages();

            foreach (var page in pages)
            {
                tabPage.Children.Add(page as Page);
            }

            return tabPage;
        }

        private Page Create3()
        {
            return Core.Reflection.CreateInstance<Page>(PageType);
        }
        
        public NavMenuItemImpl(string title, string dest, string category) : base(title, dest, category)
        {
            if (typeof(IViewPresenter).IsAssignableFrom(PageType))
            {
                Presenter = Core.Reflection.CreateInstance<IViewPresenter>(PageType);

                if (Presenter.PageCount == 1)
                    LazyCorePage = new Lazy<Page>(Create1);
                else
                    LazyCorePage = new Lazy<Page>(Create2);
            }
            else
            {
                LazyCorePage = new Lazy<Page>(Create3);
            }

            LazyNavPage = new Lazy<NavigationPage>(CreateNavigationPage);
        }
    }
}
