using DanielStone.MaterialAboutLibrary.Items;
using System;

namespace HandSchool.Droid
{
    public class AboutMenuItemClick : Java.Lang.Object, IMaterialAboutItemOnClickAction
    {
        public event Action Clicked;

        public AboutMenuItemClick(Action action = null)
        {
            Clicked = action;
        }

        public void OnClick()
        {
            Clicked?.Invoke();
        }
    }
}