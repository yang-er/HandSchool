using Android.Content;
using Android.Support.V4.App;
using HandSchool.Views;
using Xamarin.Forms.Platform.Android;

namespace HandSchool.Droid
{
    public class TabbedPagerAdapter : FragmentPagerAdapter
    {
        public IViewPresenter Presenter { get; }
        public INavigate Navigate { get; }
        public IViewPage[] AllPages { get; }
        public Fragment[] Fragments { get; }
        public Xamarin.Forms.ContentPage[] ContentPages { get; }

        public Context Context { get; }

        public override int Count => Presenter?.PageCount ?? 0;

        public TabbedPagerAdapter(Context context, INavigate navigate,
            FragmentManager fm, IViewPresenter presenter) : base(fm)
        {
            Context = context;
            Navigate = navigate;
            Presenter = presenter;
            AllPages = Presenter.GetAllPages();
            Fragments = new Fragment[presenter.PageCount];
            ContentPages = new Xamarin.Forms.ContentPage[presenter.PageCount];
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

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(AllPages[position].Title);
        }
    }
}