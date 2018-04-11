using HandSchool.Internal;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace HandSchool.Views
{
    // Thanks to 张高兴
    public class MasterPageItem : NotifyPropertyChanged
    {
        private Color color = new Color();
        public string FontFamily { get; set; }
        public string Icon { get; set; }
        public NavigationPage DestPage { get; set; }
        private bool selected = false;

        public string Title
        {
            get => DestPage.Title;
            set => DestPage.Title = value;
        }

        public bool Selected
        {
            get => selected;
            set => SetProperty(ref selected, value);
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
