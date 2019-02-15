using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using HandSchool.Views;

namespace HandSchool.Droid
{
    [BindView(Resource.Layout.layout_tabbed)]
    public class TabbedFragment : ViewFragment
    {
        [BindView(Resource.Id.viewPager)]
        public ViewPager ViewPager { get; set; }

        public TabLayout Tabbar { get; set; }
        public TabbedPagerAdapter Adapter { get; set; }
        public IViewPresenter Presenter { get; }

        public TabbedFragment(IViewPresenter presenter)
        {
            Presenter = presenter;
            Title = presenter.Title;
            RetainInstance = false;
        }
        
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            Adapter = new TabbedPagerAdapter(view.Context, this);
            ViewPager.Adapter = Adapter;
            Tabbar.AddOnTabSelectedListener(Adapter);
            Tabbar.SetupWithViewPager(ViewPager);
        }
    }
}