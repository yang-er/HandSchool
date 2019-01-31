using HandSchool.Models;
using HandSchool.Views;
using System;

namespace HandSchool.Droid
{
    public class NavMenuItemV2 : NavigationMenuItem
    {
        public bool IsFragment { get; }
        public bool IsTabbedPage { get; }

        public IViewPresenter CreatePresenter()
        {
            if (!IsTabbedPage) throw new InvalidCastException();
            return Core.Reflection.CreateInstance<IViewPresenter>(PageType);
        }

        public ViewFragment CreateFragment()
        {
            if (!IsFragment) throw new InvalidCastException();
            return Core.Reflection.CreateInstance<ViewFragment>(PageType);
        }

        public ViewObject CreateObject()
        {
            if (IsFragment || IsTabbedPage) throw new InvalidCastException();
            return Core.Reflection.CreateInstance<ViewObject>(PageType);
        }

        public NavMenuItemV2(string title, string dest, string category) : base(title, dest, category)
        {
            IsTabbedPage = typeof(IViewPresenter).IsAssignableFrom(PageType);
            IsFragment = typeof(ViewFragment).IsAssignableFrom(PageType);
        }
    }
}