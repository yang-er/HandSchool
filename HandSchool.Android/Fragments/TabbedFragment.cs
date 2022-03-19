using System;
using System.Linq;
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Widget;
using HandSchool.Views;
using Google.Android.Material.Tabs;

namespace HandSchool.Droid
{
    [BindView(Resource.Layout.layout_tabbed)]
    public class TabbedFragment : ViewFragment, TabLayoutMediator.ITabConfigurationStrategy
    {
        [BindView(Resource.Id.viewPager)] public ViewPager2 ViewPager { get; set; }

        public TabLayoutMediator Mediator { get; private set; }

        public TabLayout Tabbar { get; set; }

        public TabbedPagerAdapter Adapter { get; set; }

        public ValueTuple<IViewPage, Fragment>[] Pages { get; }

        public TabbedFragment(IViewPresenter presenter)
        {
            Pages = presenter.GetAllPages()
                .Select(view => new ValueTuple<IViewPage, Fragment>(view, null))
                .ToArray();
            Title = presenter.Title;
        }

        private class PageChangeCallBack : ViewPager2.OnPageChangeCallback
        {
            private TabbedFragment _father;

            public PageChangeCallBack(TabbedFragment father)
            {
                _father = father;
            }

            internal int? LastIndex
            {
                get => _lastIndex;
                set
                {
                    if (_lastIndex == value) return;
                    var last = _lastIndex;
                    _lastIndex = value;
                    PageChanged(last, value);
                }
            }

            private int? _lastIndex;

            public override void OnPageSelected(int position)
            {
                base.OnPageSelected(position);
                LastIndex = position;
            }

            private void PageChanged(int? oldPage, int? newPage)
            {
                if (oldPage != null)
                {
                    (_father.Pages[oldPage.Value].Item1 as IViewLifecycle)?.SendDisappearing();
                }

                if (newPage != null)
                {
                    (_father.Pages[newPage.Value].Item1 as IViewLifecycle)?.SendAppearing();
                }
            }

            private bool _disposed;

            protected override void Dispose(bool disposing)
            {
                if (_disposed) return;
                base.Dispose(disposing);
                _father = null;
                _disposed = true;
            }
        }

        private PageChangeCallBack _pageChangeCallBack;

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            _refCleared = false;
            ViewPager.SaveEnabled = false;
            Adapter = new TabbedPagerAdapter(this);
            ViewPager.Adapter = Adapter;
            Mediator = new TabLayoutMediator(Tabbar, ViewPager, this);
            Mediator.Attach();
            _pageChangeCallBack ??= new PageChangeCallBack(this);
            ViewPager.RegisterOnPageChangeCallback(_pageChangeCallBack);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _pageChangeCallBack.LastIndex = null;
            ClearReference();
        }

        public void OnConfigureTab(TabLayout.Tab tab, int i)
        {
            tab.SetText(Pages[i].Item1.Title);
        }

        private bool _refCleared;

        public void ClearReference()
        {
            if (_refCleared) return;
            Mediator.Detach();
            ViewPager.UnregisterOnPageChangeCallback(_pageChangeCallBack);
            ViewPager.Adapter = null;
            _refCleared = true;
        }
    }
}