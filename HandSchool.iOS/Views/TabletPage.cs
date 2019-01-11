using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class TabletPageImpl : MasterDetailPage, INavigation
    {
        public TabletPageImpl(PopContentPage insidePage)
        {
            MasterBehavior = MasterBehavior.Split;
            insidePage.Navigation = this;

            Detail = new NavigationPage(insidePage.SetTabletDefaultPage());
            Master = new NavigationPage(insidePage) { Title = insidePage.Title };
            BackgroundColor = Color.DarkGray;
        }

        public INavigation SubNavigation => Detail.Navigation;

        public IReadOnlyList<Page> NavigationStack => SubNavigation.NavigationStack;

        public async Task PushAsync(Page page)
        {
            await SubNavigation.PushAsync(page);

            while (NavigationStack.Count > 1)
            {
                SubNavigation.RemovePage(NavigationStack[0]);
            }
        }

        #region NotImplementedFunctions

        IReadOnlyList<Page> INavigation.ModalStack => throw new NotImplementedException();

        void INavigation.InsertPageBefore(Page page, Page before)
        {
            throw new NotImplementedException();
        }

        Task<Page> INavigation.PopAsync()
        {
            throw new NotImplementedException();
        }

        Task<Page> INavigation.PopAsync(bool animated)
        {
            throw new NotImplementedException();
        }

        Task<Page> INavigation.PopModalAsync()
        {
            throw new NotImplementedException();
        }

        Task<Page> INavigation.PopModalAsync(bool animated)
        {
            throw new NotImplementedException();
        }

        Task INavigation.PopToRootAsync()
        {
            throw new NotImplementedException();
        }

        Task INavigation.PopToRootAsync(bool animated)
        {
            throw new NotImplementedException();
        }

        Task INavigation.PushAsync(Page page, bool animated)
        {
            throw new NotImplementedException();
        }

        Task INavigation.PushModalAsync(Page page)
        {
            throw new NotImplementedException();
        }

        Task INavigation.PushModalAsync(Page page, bool animated)
        {
            throw new NotImplementedException();
        }

        void INavigation.RemovePage(Page page)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
