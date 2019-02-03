using Android.Content;
using Android.Support.V4.App;
using Android.Views;
using HandSchool.Internal;
using HandSchool.Views;
using Java.Lang;
using Xamarin.Forms.Platform.Android;

namespace HandSchool.Droid
{
    public class TabbedPagerAdapter : FragmentPagerAdapter
    {
        public IViewPresenter Presenter { get; }
        public INavigate Navigate { get; }
        public IViewPage[] AllPages { get; }
        public Fragment[] Fragments { get; }
        public ToolbarMenuTracker Tracker { get; }
        public Xamarin.Forms.ContentPage[] ContentPages { get; }

        public Context Context { get; }

        public override int Count => Presenter?.PageCount ?? 0;

        public TabbedPagerAdapter(Context context, TabbedFragment fm)
            : base(fm.ChildFragmentManager)
        {
            Context = context;
            Navigate = fm.Navigation;
            Presenter = fm.Presenter;
            AllPages = Presenter.GetAllPages();
            Fragments = new Fragment[Presenter.PageCount];
            Tracker = fm.ToolbarMenu;
            ContentPages = new Xamarin.Forms.ContentPage[Presenter.PageCount];
        }

        public override Fragment GetItem(int i)
        {
            if (Fragments[i] is null)
            {
                AllPages[i].RegisterNavigation(Navigate);

                ContentPages[i] = new Xamarin.Forms.ContentPage
                {
                    Content = AllPages[i].Content,
                    BindingContext = AllPages[i].ViewModel
                };
                
                Fragments[i] = ContentPages[i].CreateSupportFragment(Context);
            }

            return Fragments[i];
        }

        public override void SetPrimaryItem(ViewGroup container, int position, Object @object)
        {
            base.SetPrimaryItem(container, position, @object);
            Tracker.List = ((ViewObject)AllPages[position]).ToolbarTracker.List;
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new String(AllPages[position].Title);
        }
    }
}