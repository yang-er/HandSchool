using HandSchool.Views;
using Xamarin.Forms;

namespace HandSchool.iOS
{
    public class TabletPageImpl : MasterDetailPage
    {
        public TabletPageImpl(ViewPage insidePage, ViewPage defaultPage)
        {
            MasterBehavior = MasterBehavior.Split;
            Master = new NavigationPage(insidePage) { Title = insidePage.Title };
            Detail = new NavigationPage(defaultPage);
            insidePage.RegisterNavigation(defaultPage.Navigation);
            BackgroundColor = Color.DarkGray;
        }
    }
}
