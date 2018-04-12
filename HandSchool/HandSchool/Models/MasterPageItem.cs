using HandSchool.Internal;
using System;
using System.ComponentModel;
using System.Reflection;
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
        public Type DestinationPageType { get; }
        private bool selected = false;
        private string title;

        public MasterPageItem() { }

        public MasterPageItem(string title, string dest, string icon, string apple, bool select = false)
        {
            this.title = title;

            if (Device.RuntimePlatform == Device.Android)
            {
                DestPage = new NavigationPage(Assembly.GetExecutingAssembly().CreateInstance("HandSchool.Views." + dest) as Page);
                DestPage.Title = title;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                DestPage = new NavigationPage(Assembly.GetExecutingAssembly().CreateInstance("HandSchool.Views." + dest) as Page);
                DestPage.Icon = new FileImageSource { File = apple };
                DestPage.Title = title;
            }
            else if (Device.RuntimePlatform == Device.UWP)
            {
                FontFamily = segoemdl2;
                Icon = icon;
                DestPage = new NavigationPage(Assembly.GetExecutingAssembly().CreateInstance("HandSchool.Views." + dest) as Page);
                DestinationPageType = Assembly.GetExecutingAssembly().GetType("HandSchool.UWP." + dest);
            }

            selected = select;
            color = select ? active : inactive;
        }

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
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

        public override string ToString() => title;
    }
}
