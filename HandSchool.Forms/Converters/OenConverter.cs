using HandSchool.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace HandSchool.Views
{
    /// <summary>
    /// WeekOen与int互相转化
    /// </summary>
    public class OenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return (WeekOddEvenNone)value;
        }
    }
}
