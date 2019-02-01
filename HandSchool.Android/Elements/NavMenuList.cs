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
            MenuItems = new List<List<NavMenuItemV2>>
            {
                PlatformImplV2.NavigationItems,
                PlatformImplV2.NavigationItemsSec
            };
        }

        public List<List<NavMenuItemV2>> MenuItems { get; }

        public event Func<NavMenuItemV2, IMenuItem, bool> NavigationItemSelected;
        
        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            return NavigationItemSelected?.Invoke(
                MenuItems[menuItem.GroupId][menuItem.ItemId % 100],
                menuItem) ?? false;
        }

        public void InflateMenus(IMenu menu)
        {
            int itemId = 0;

            for (int i = 0; i < MenuItems.Count; i++)
            {
                for (int j = 0; j < MenuItems[i].Count; j++)
                {
                    var item = menu.Add(i, i * 100 + j, itemId++, MenuItems[i][j].Title);
                    item.SetIcon(MenuItems[i][j].DrawableId);
                    item.SetCheckable(true);
                }
            }
        }
    }
}