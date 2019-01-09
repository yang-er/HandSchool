using System;
using Windows.UI.Xaml.Data;

namespace HandSchool.Views
{
    /// <summary>
    /// 布尔值翻转转换器。
    /// </summary>
    public sealed class BoolReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }
    }
}
