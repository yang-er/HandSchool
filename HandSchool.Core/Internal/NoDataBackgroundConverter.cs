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

            if (targetType == typeof(string))
            {
                return Value > 0 ? "" : "nodatabg";
            }
            else if (targetType == typeof(Color))
            {
                return Value > 0 && Core.Platform.RuntimeName == "Android" ? Color.FromRgb(244, 244, 244) : default(Color);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
