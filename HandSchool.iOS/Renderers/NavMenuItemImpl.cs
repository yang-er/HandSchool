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
        public FileImageSource Icon { get; }
        private IViewPresenter Presenter { get; }
        private readonly Lazy<TapEntranceWrapper> Wrapper;
        private readonly Lazy<ViewPage> LazyCorePage;
        private readonly Lazy<Page> LazyFullPage;

        public TapEntranceWrapper AsEntrance() => Wrapper.Value;

        public Page Page => LazyFullPage.Value;

        public NavMenuItemImpl(string title, string dest, string category, string icon = "") : base(title, dest, category)
        {
            if (!string.IsNullOrEmpty(icon)) Icon = new FileImageSource { File = icon };
            Wrapper = new Lazy<TapEntranceWrapper>(() => CreateEntrance());
            
            if (typeof(IViewPresenter).IsAssignableFrom(PageType))
            {
                Presenter = Core.Reflection.CreateInstance<IViewPresenter>(PageType);
                if (Presenter.PageCount != 1) throw new NotImplementedException("Not designed");
                LazyCorePage = new Lazy<ViewPage>(Create1);
            }
            else
            {
                LazyCorePage = new Lazy<ViewPage>(Create3);
            }

            LazyFullPage = new Lazy<Page>(CreateFull);
        }

        private Page CreateFull()
        {
            var page = LazyCorePage.Value;
            if ((bool)page.GetValue(PlatformExtensions.UseTabletModeProperty))
            {
                return new TabletPageImpl(page, null);
            }
            else
            {
                return new NavigationPage(page);
            }
        }

        private ViewPage Create1()
        {
            return Presenter.GetAllPages()[0] as ViewPage;
        }

        private ViewPage Create3()
        {
            return Core.Reflection.CreateInstance<ViewPage>(PageType);
        }

        private async Task ExecuteAsEntrance(INavigate navigate)
        {
            await navigate.PushAsync(LazyCorePage.Value);
        }

        private TapEntranceWrapper CreateEntrance()
        {
            return new TapEntranceWrapper(Title, Title + "的入口。", ExecuteAsEntrance);
        }
    }
}