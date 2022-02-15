using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace HandSchool.Views
{
    public class ViewPresenterConverter : TabbedPage
    {
        public ViewPresenterConverter(IEnumerable<ContentPage> pages)
        {
            SetPages(pages);
        }

        public ViewPresenterConverter(IViewPresenter viewPresenter)
            : this(viewPresenter.GetAllPages().Cast<ContentPage>().ToArray())
        {
        }

        public void SetPages(IEnumerable<ContentPage> pages)
        {
            pages.ForEach(Children.Add);
        }
    }
}