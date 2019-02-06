using HandSchool.Internals;
using Windows.UI.Xaml.Controls;

namespace HandSchool.UWP
{
    internal class GridViewItemData : NotifyPropertyChanged
    {
        private Symbol _icon = Symbol.Emoji;
        private string _ln1 = "";
        private string _ln2 = "";
        private string _ln3 = "";
        private string _ln4 = "";

        public Symbol Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        public string Line1
        {
            get => _ln1;
            set => SetProperty(ref _ln1, value);
        }

        public string Line2
        {
            get => _ln2;
            set => SetProperty(ref _ln2, value);
        }

        public string Line3
        {
            get => _ln3;
            set => SetProperty(ref _ln3, value);
        }

        public string Line4
        {
            get => _ln4;
            set => SetProperty(ref _ln4, value);
        }
    }
}
