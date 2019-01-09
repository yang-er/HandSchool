using HandSchool.Models;
using System;
using Windows.UI.Xaml.Data;

namespace HandSchool.Views
{
    /// <summary>
    /// 单双周枚举与SelectedIndex相互转化的转换器。
    /// </summary>
    public sealed class OenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (WeekOddEvenNone)value;
        }
    }
}
