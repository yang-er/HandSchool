using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HandSchool.Views
{
    /// <summary>
    /// 布尔值与显示枚举的转换器。
    /// </summary>
    public sealed class VisibilityConverter : IValueConverter
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
