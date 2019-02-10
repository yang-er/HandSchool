using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using HandSchool.Internals;
using HandSchool.Views;
using Java.Lang;

namespace HandSchool.Droid
{
    public class TabbedPagerAdapter : FragmentPagerAdapter, TabLayout.IOnTabSelectedListener
    {
        public IViewPresenter Presenter { get; }
        public INavigate Navigate { get; }
        public IViewPage[] AllPages { get; }
        public EmbeddedFragment[] Fragments { get; }
        public ToolbarMenuTracker Tracker { get; }

        public Context Context { get; }

        public override int Count => Presenter?.PageCount ?? 0;

        public TabbedPagerAdapter(Context context, TabbedFragment fm)
            : base(fm.ChildFragmentManager)
        {
            Context = context;
            Navigate = fm.Navigation;
            Presenter = fm.Presenter;
            AllPages = Presenter.GetAllPages();
            Fragments = new EmbeddedFragment[Presenter.PageCount];
            Tracker = fm.ToolbarMenu;
        }

        public override Fragment GetItem(int i)
        {
            if (Fragments[i] is null)
            {
                ((IViewLifecycle)AllPages[i]).RegisterNavigation(Navigate);
                Fragments[i] = new EmbeddedFragment((ViewObject)AllPages[i], Context, true);
                if (i == 0) (AllPages[0] as IViewLifecycle)?.SendAppearing();
            }
            
            return Fragments[i];
        }

        public void ClearBindings(TabLayout tl)
        {
            tl.RemoveOnTabSelectedListener(this);
            tl.SetupWithViewPager(null);
        }
        
        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new String(AllPages[position].Title);
        }

        public void OnTabReselected(TabLayout.Tab tab) { }

        public void OnTabSelected(TabLayout.Tab tab)
        {
            Tracker.List = AllPages[tab.Position].ToolbarTracker.List;
            (AllPages[tab.Position] as IViewLifecycle)?.SendAppearing();
        }

        public void OnTabUnselected(TabLayout.Tab tab)
        {
            (AllPages[tab.Position] as IViewLifecycle)?.SendDisappearing();
        }
    }
}