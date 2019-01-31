using Android.Support.Design.Widget;
using Android.Views;
using System;
using System.Collections.Generic;

namespace HandSchool.Droid
{
    public class NavMenuListHandler : Java.Lang.Object,
        NavigationView.IOnNavigationItemSelectedListener
    {
        public NavMenuListHandler()
        {
            MenuItems = new List<List<NavMenuItemV2>>();
            MenuItems.Add(PlatformImplV2.Instance.NavigationItems);
        }

        public List<List<NavMenuItemV2>> MenuItems { get; }

        public event Func<NavMenuItemV2, IMenuItem, bool> NavigationItemSelected;
        
        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            if (NavigationItemSelected is null) return false;
            return NavigationItemSelected(MenuItems[menuItem.GroupId][menuItem.ItemId], menuItem);
        }

        public void InflateMenus(IMenu menu)
        {
            for (int i = 0; i < MenuItems.Count; i++)
            {
                for (int j = 0; j < MenuItems[i].Count; j++)
                {
                    var item = menu.Add(i, j, j, MenuItems[i][j].Title);

                    // item.SetIcon()
                }
            }
        }
    }
}