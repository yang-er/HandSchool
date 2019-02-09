using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using HandSchool.Droid;
using HandSchool.Views;

namespace HandSchool.Droid
{
    public class TabbedFragment : ViewFragment
    {
        [BindView(Resource.Id.sliding_tabs)]
        public TabLayout Tabbar { get; set; }

        [BindView(Resource.Id.viewPager)]
        public ViewPager ViewPager { get; set; }

        public TabbedPagerAdapter Adapter { get; set; }
        public IViewPresenter Presenter { get; }

        public TabbedFragment(IViewPresenter presenter)
        {
            FragmentViewResource = Resource.Layout.layout_tabbed;
            Presenter = presenter;
            Title = presenter.Title;
            RetainInstance = false;
        }
        
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            Adapter = new TabbedPagerAdapter(view.Context, this);
            ViewPager.Adapter = Adapter;
            Tabbar.SetupWithViewPager(ViewPager);
            Tabbar.Visibility = ViewStates.Visible;
            Tabbar.AddOnTabSelectedListener(Adapter);
        }

        public override void SendAppearing()
        {
            base.SendAppearing();
            if (Adapter != null) Tabbar.AddOnTabSelectedListener(Adapter);
        }

        public override void SendDisappearing()
        {
            base.SendDisappearing();
            Tabbar.RemoveOnTabSelectedListener(Adapter);
        }
    }
}