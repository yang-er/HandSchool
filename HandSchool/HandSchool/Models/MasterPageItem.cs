using HandSchool.Internal;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace HandSchool.Views
{
    // Thanks to 张高兴
    public class MasterPageItem : NotifyPropertyChanged
    {
        static Color active = Color.FromRgb(0, 120, 215);
        static Color inactive = Color.Black;
        const string segoemdl2 = "/Assets/segmdl2.ttf#Segoe MDL2 Assets";

        private Color color = new Color();
        public string FontFamily { get; set; }
        public string Icon { get; set; }
        public NavigationPage DestPage { get; set; }
        private bool selected = false;

        public MasterPageItem() { }

        public MasterPageItem(string title, NavigationPage dest, string icon, string apple, bool select = false)
        {
            DestPage = dest;
            dest.Title = title;

            if (Device.RuntimePlatform == Device.iOS)
            {
                DestPage.Icon = new FileImageSource { File = apple };
            }
            else if (Device.RuntimePlatform == Device.UWP)
            {
                FontFamily = segoemdl2;
                Icon = icon;
            }

            selected = select;
            color = select ? active : inactive;
        }

        public string Title
        {
            get => DestPage.Title;
            set => DestPage.Title = value;
        }

        public bool Selected
        {
            get => selected;
            set
            {
                SetProperty(ref selected, value);
                SetProperty(ref color, value ? active : inactive, "Color");
            }
        }

        public FileImageSource AppleIcon
        {
            get => DestPage.Icon;
            set { if (Device.RuntimePlatform == Device.iOS) DestPage.Icon = value; }
        }
        
        public Color Color
        {
            get => color;
            set => SetProperty(ref color, value);
        }
    }
}
