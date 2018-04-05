using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace HandSchool.Views
{
    // Thanks to 张高兴
    public class MasterPageItem : INotifyPropertyChanged
    {
        private Color color = new Color();
        public string FontFamily { get; set; }
        public string Icon { get; set; }
        public NavigationPage DestPage { get; set; }
        private bool selected = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title
        {
            get => DestPage.Title;
            set => DestPage.Title = value;
        }

        public bool Selected
        {
            get { return selected; }
            set { selected = value; OnPropertyChanged("Selected"); }
        }

        public FileImageSource AppleIcon
        {
            get => DestPage.Icon;
            set { if (Device.RuntimePlatform == Device.iOS) DestPage.Icon = value; }
        }
        
        public Color Color
        {
            get { return color; }
            set { color = value; OnPropertyChanged("Color"); }
        }
        
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
