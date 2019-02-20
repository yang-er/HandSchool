using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.Views;
using System;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace HandSchool.Droid
{
    public enum NavMenuItemType
    {
        Others,
        FragmentCore,
        Fragment,
        Activity,
        Presenter,
        Object,
    }

    public class NavMenuItemV2 : NavigationMenuItem
    {
        public NavMenuItemType Type { get; }

        public int DrawableId { get; }

        public static readonly int[] IconList = new[]
        {
            Resource.Drawable.ic_nav_home,
            Resource.Drawable.ic_nav_sched,
            Resource.Drawable.ic_nav_feed,
            Resource.Drawable.ic_nav_info,
            Resource.Drawable.aboutpage_feedbackicon,
            Resource.Drawable.ic_nav_grade,
            Resource.Drawable.ic_nav_card,
            Resource.Drawable.ic_nav_settings,
            Resource.Drawable.ic_nav_about,
        };

        public IViewPresenter CreatePresenter()
        {
            if (Type != NavMenuItemType.Presenter) throw new InvalidCastException();
            return Core.Reflection.CreateInstance<IViewPresenter>(PageType);
        }

        public ViewFragment CreateFragment()
        {
            if (Type != NavMenuItemType.Fragment) throw new InvalidCastException();
            return Core.Reflection.CreateInstance<ViewFragment>(PageType);
        }

        public ViewObject CreateObject()
        {
            if (Type != NavMenuItemType.Object) throw new InvalidCastException();
            return Core.Reflection.CreateInstance<ViewObject>(PageType);
        }

        public SupportFragment CreateFragmentCore()
        {
            if (Type != NavMenuItemType.FragmentCore) throw new InvalidCastException();
            return Core.Reflection.CreateInstance<SupportFragment>(PageType);
        }

        public (SupportFragment, IViewCore) FragmentV3 => lazyFm.Value;

        private readonly Lazy<(SupportFragment,IViewCore)> lazyFm;
        
        private (SupportFragment, IViewCore) CreateV3()
        {
            switch (Type)
            {
                case NavMenuItemType.FragmentCore:
                    var fcc = CreateFragmentCore();
                    return (fcc, fcc as IViewCore);

                case NavMenuItemType.Fragment:
                    var fg = CreateFragment();
                    fg.SetNavigationArguments(null);
                    return (fg, fg);

                case NavMenuItemType.Presenter:
                    var pr = CreatePresenter();
                    var tfg = new TabbedFragment(pr);
                    return (tfg, tfg);

                case NavMenuItemType.Object:
                    var viewPage = CreateObject();
                    viewPage.SetNavigationArguments(null);
                    var frg = new EmbeddedFragment(viewPage);
                    return (frg, viewPage);

                default:
                    throw new InvalidOperationException();
            }
        }

        public static NavMenuItemType Judge(Type PageType)
        {
            if (typeof(IViewPresenter).IsAssignableFrom(PageType))
                return NavMenuItemType.Presenter;
            else if (typeof(ViewFragment).IsAssignableFrom(PageType))
                return NavMenuItemType.Fragment;
            else if (typeof(SupportFragment).IsAssignableFrom(PageType))
                return NavMenuItemType.FragmentCore;
            else if (typeof(ViewObject).IsAssignableFrom(PageType))
                return NavMenuItemType.Object;
            else if (typeof(BaseActivity).IsAssignableFrom(PageType))
                return NavMenuItemType.Activity;
            else
                return NavMenuItemType.Others;
        }

        public NavMenuItemV2(string title, string dest, string category, MenuIcon icon) : base(title, dest, category)
        {
            Type = Judge(PageType);
            DrawableId = IconList[(int)icon];
            lazyFm = new Lazy<(SupportFragment, IViewCore)>(CreateV3);
        }
    }
}