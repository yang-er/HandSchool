using Android.Views;
using Google.Android.Material.Navigation;
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

        public (int, int)? GetIndex(int index)
        {
            if (index < 0) return null;
            var count = 0;
            var first = 0;
            while (count < index && first < MenuItems.Count)
            {
                if (index - count >= MenuItems[first].Count)
                {
                    count += MenuItems[first].Count;
                    first++;
                }
                else
                {
                    return (first, index - count);
                }
            }
            if (count == index)
                return (first, index - count);
            return null;
        }

        public NavMenuItemV2 GetItem(int index)
        {
            var indexs = GetIndex(index);
            if (indexs == null) return null;
            var (first, sec) = indexs.Value;
            return MenuItems[first][sec];
        }

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