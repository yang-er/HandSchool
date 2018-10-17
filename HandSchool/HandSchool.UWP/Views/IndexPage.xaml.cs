using System;
using HandSchool.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HandSchool.Views
{
    public sealed partial class IndexPage : ViewPage
    {
        public int TextSize=20;
        public int LineLenth = 500;

        public IndexPage()
        {
            InitializeComponent();
            ViewModel = IndexViewModel.Instance;
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = (bool)value;
            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var val = (Visibility)value;
            return val == Visibility.Visible;
        }
    }
}
