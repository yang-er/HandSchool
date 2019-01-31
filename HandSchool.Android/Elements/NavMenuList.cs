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
            MenuItems.Add(PlatformImplV2.NavigationItems);
            MenuItems.Add(PlatformImplV2.NavigationItemsSec);
        }

        public List<List<NavMenuItemV2>> MenuItems { get; }

        public event Func<NavMenuItemV2, IMenuItem, bool> NavigationItemSelected;
        
        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            if (NavigationItemSelected is null) return false;
            return NavigationItemSelected(MenuItems[menuItem.GroupId][menuItem.ItemId % 100], menuItem);
        }

        public void InflateMenus(IMenu menu)
        {
            int itemId = 0;

            for (int i = 0; i < MenuItems.Count; i++)
            {
                for (int j = 0; j < MenuItems[i].Count; j++)
                {
                    var item = menu.Add(i, i * 100 + j, itemId++, MenuItems[i][j].Title);

                    // item.SetIcon()
                }
            }
        }
    }
}