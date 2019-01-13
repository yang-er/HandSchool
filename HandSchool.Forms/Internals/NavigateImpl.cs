using HandSchool.Views;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Forms
{
    /// <summary>
    /// 基于Xamarin.Forms的导航实现类。
    /// </summary>
    /// <inheritdoc cref="INavigate" />
    /// <inheritdoc cref="IReadOnlyList{T}" />
    internal class NavigateImpl : INavigate, IReadOnlyList<IViewPage>
    {
        public NavigateImpl(NavigationPage navPage)
        {
            InnerNavigation = navPage.Navigation;
            navPage.Pushed += NavigationOccured;
        }

        public void NavigationOccured(object sender, NavigationEventArgs args)
        {
            if (args.Page is TabbedPage tabbed)
            {
                foreach (IViewPage core in tabbed.Children)
                {
                    core.RegisterNavigation(this);
                }
            }
            else if (args.Page is ViewPage core)
            {
                core.RegisterNavigation(this);
            }
        }

        private INavigation InnerNavigation { get; set; }

        public IReadOnlyList<IViewPage> NavigationStack => this;

        public async Task<IViewPage> PopAsync()
        {
            var page = await InnerNavigation.PopAsync();
            return page as ViewPage;
        }

        public Task PushAsync(IViewPage page)
        {
            return InnerNavigation.PushAsync(page as ViewPage);
        }

        public int Count => InnerNavigation.NavigationStack.Count;

        public IViewPage this[int index] => InnerNavigation.NavigationStack[index] as ViewPage;

        private IEnumerable<ViewPage> GetEnumerable()
        {
            foreach (var page in InnerNavigation.NavigationStack)
            {
                yield return page as ViewPage;
            }
        }

        public IEnumerator<IViewPage> GetEnumerator() => GetEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerable().GetEnumerator();
    }
}
