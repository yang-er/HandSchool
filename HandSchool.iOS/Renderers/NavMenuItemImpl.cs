using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.iOS
{
    public class NavMenuItemImpl : NavigationMenuItem
    {
        public static readonly string[] IconList = new[]
        {
            "tab_rec.png",
            "tab_sched.png",
            "tab_feed.png",
            "tab_about.png",
            null,
            null,
            null,
            null,
            null,
        };

        public FileImageSource Icon { get; }
        private IViewPresenter Presenter { get; }
        private readonly Lazy<TapEntranceWrapper> Wrapper;
        private readonly Lazy<ViewObject> LazyCorePage;
        private readonly Lazy<Page> LazyFullPage;

        public TapEntranceWrapper AsEntrance() => Wrapper.Value;

        public Page Page => LazyFullPage.Value;

        public NavMenuItemImpl(string title, string dest, string category, MenuIcon icon2) : base(title, dest, category)
        {
            var icon = IconList[(int)icon2];
            if (!string.IsNullOrEmpty(icon)) Icon = new FileImageSource { File = icon };
            Wrapper = new Lazy<TapEntranceWrapper>(() => CreateEntrance());
            
            if (typeof(IViewPresenter).IsAssignableFrom(PageType))
            {
                Presenter = Core.Reflection.CreateInstance<IViewPresenter>(PageType);
                if (Presenter.PageCount != 1) throw new NotImplementedException("Not designed");
                LazyCorePage = new Lazy<ViewObject>(Create1);
            }
            else
            {
                LazyCorePage = new Lazy<ViewObject>(Create3);
            }

            LazyFullPage = new Lazy<Page>(CreateFull);
        }

        private Page CreateFull()
        {
            var page = LazyCorePage.Value;
            Page toReturn;

            if ((bool)page.GetValue(PlatformExtensions.UseTabletModeProperty))
            {
                toReturn = new TabletPageImpl(page, null);
            }
            else
            {
                toReturn = new NavigationPage(page);
            }

            toReturn.Title = Title;
            toReturn.Icon = Icon;
            return toReturn;
        }

        private ViewObject Create1()
        {
            return Presenter.GetAllPages()[0] as ViewObject;
        }

        private ViewObject Create3()
        {
            var pg = Core.Reflection.CreateInstance<ViewObject>(PageType);
            pg.SetNavigationArguments(null);
            return pg;
        }

        private async Task ExecuteAsEntrance(INavigate navigate)
        {
            await navigate.PushAsync(PageType, null);
        }

        private TapEntranceWrapper CreateEntrance()
        {
            return new TapEntranceWrapper(Title, Title + "的入口。", ExecuteAsEntrance);
        }
    }
}