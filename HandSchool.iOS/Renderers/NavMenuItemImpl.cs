using HandSchool.Models;
using HandSchool.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.iOS
{
    public class NavMenuItemImpl : NavigationMenuItem
    {
        public static readonly string[] IconList = {
            "nav_home.png",
            "nav_sched.png",
            "nav_feed.png",
            "nav_info.png",
            "nav_mail.png",
            "nav_grade.png",
            "nav_card.png",
            "nav_settings.png",
            "nav_about.png",
            "nav_libroom.png"
        };

        public FileImageSource Icon { get; }
        private IViewPresenter Presenter { get; }
        private readonly Lazy<TapEntranceWrapper> _wrapper;
        public TapEntranceWrapper AsEntrance() => _wrapper.Value;
        private readonly Func<Page> _getPage;
        private readonly Func<ViewObject> _getViewObj;
        private (Page, NavigationPage) _instances;
        private bool _used;
        private bool _singleInstance;
        public bool IsSingleInstance
        {
            get => _singleInstance;
            set
            {
                lock (this)
                {
                    if (_used)
                    {
                        throw new InvalidOperationException("Instance has been created! ");
                    }
                    _singleInstance = value;
                }
            }
        }

        public Page GetPageInstance()
        {
            lock (this)
            {
                _used = true;

                if (!_singleInstance)
                {
                    return _getViewObj?.Invoke() ?? _getPage.Invoke();
                }
                
                _instances.Item1 ??= _getViewObj?.Invoke() ?? _getPage.Invoke();
                return _instances.Item1;
            }
        }

        public NavigationPage GetNavigationPage(Page page = null)
        {
            lock (this)
            {
                _used = true;
                if (!_singleInstance)
                {
                    return new NavigationPage(page ?? GetPageInstance())
                    {
                        Title = Title,
                        IconImageSource = Icon
                    };
                }

                _instances.Item2 ??= new NavigationPage(page ?? GetPageInstance())
                {
                    Title = Title,
                    IconImageSource = Icon
                };
                return _instances.Item2;
            }
        }

        public NavMenuItemImpl(string title, string dest, string category, MenuIcon icon2) : base(title, dest, category)
        {
            var icon = IconList[(int)icon2];
            if (!string.IsNullOrEmpty(icon)) Icon = new FileImageSource { File = icon };
            _wrapper = new Lazy<TapEntranceWrapper>(CreateEntrance);
            
            if (typeof(IViewPresenter).IsAssignableFrom(PageType))
            {
                Presenter = Core.Reflection.CreateInstance<IViewPresenter>(PageType);
                _getPage = ConvertPresenter;
            }
            else
            {
                _getViewObj = Create3;
            }
        }
        
        private Page ConvertPresenter()
        {
            if (Presenter.PageCount == 1)
            {
                return (ViewObject) Presenter.GetAllPages()[0];
            }
            else
            {
                return new ViewPresenterConverter(Presenter)
                {
                    Title = Presenter.Title
                };
            }
        }

        private ViewObject Create3()
        {
            var pg = Core.Reflection.CreateInstance<ViewObject>(PageType);
            pg.SetNavigationArguments(null);
            return pg;
        }

        private async Task ExecuteAsEntrance(INavigate navigate)
        {
            await navigate.PushAsync(GetPageInstance(), null);
        }

        private TapEntranceWrapper CreateEntrance()
        {
            return new TapEntranceWrapper(Title, Title + "的入口。", ExecuteAsEntrance);
        }
    }
}