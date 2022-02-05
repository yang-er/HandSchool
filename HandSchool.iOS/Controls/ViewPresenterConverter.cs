using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace HandSchool.Views
{
    public class ViewPresenterConverter : Shell
    {
        public ViewPresenterConverter(IEnumerable<ContentPage> pages)
        {
            var tb = new TabBar();
            Items.Add(tb);
            var tab = new Tab();
            tb.Items.Add(tab);
            pages.ForEach(p =>
            {
                var sc = new ShellContent
                {
                    Title = p.Title,
                    ContentTemplate = new DataTemplate(() => p)
                };
                tab.Items.Add(sc);
            });
            FlyoutWidth = 0;
        }

        public ViewPresenterConverter(IViewPresenter viewPresenter)
            : this(viewPresenter.GetAllPages().Cast<ContentPage>().ToArray())
        {
        }
    }
}