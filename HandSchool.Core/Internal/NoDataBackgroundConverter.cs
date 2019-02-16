using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class NoDataBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var Value = (int)value;
            if (Value > 0) return "";
            return "nodatabg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
