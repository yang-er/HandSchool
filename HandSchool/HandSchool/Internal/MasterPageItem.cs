using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace HandSchool.Views
{
    // Thanks to 张高兴
    public class MasterPageItem : INotifyPropertyChanged
    {
        public string FontFamily { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public NavigationPage DestPage { get; set; }
        private bool selected = false;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; OnPropertyChanged("Selected"); }
        }
        
        private Color color = new Color();
        public Color Color
        {
            get { return color; }
            set { color = value; OnPropertyChanged("Color"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
