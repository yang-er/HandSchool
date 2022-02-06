using System;
using AndroidX.Fragment.App;
using HandSchool.Internals;
using HandSchool.Views;
using AndroidX.ViewPager2.Adapter;

namespace HandSchool.Droid
{
    public class TabbedPagerAdapter : FragmentStateAdapter
    {
        public INavigate Navigate { get; }
        public ToolbarMenuTracker Tracker { get; }
        
        public ValueTuple<IViewPage, Fragment>[] Pages { get; }

        public override int ItemCount => Pages?.Length ?? 0;

        public TabbedPagerAdapter(TabbedFragment fm):base(fm)
        {
            Navigate = fm.Navigation;
            Pages = fm.Pages;
            Tracker = fm.ToolbarMenu;
        }

        public override Fragment CreateFragment(int i)
        {
            var (page, fragment) = Pages[i];
            if (fragment is null)
            {
                if (page is IViewLifecycle lifecycle)
                {
                    lifecycle.RegisterNavigation(Navigate);
                }
                if (page is Fragment ori)
                {
                    fragment = ori;
                }
                else if (page is ViewObject viewObject)
                {
                    fragment = new EmbeddedFragment(viewObject, true);
                }
            }

            Pages[i] = (page, fragment);
            return fragment;
        }
    }
}