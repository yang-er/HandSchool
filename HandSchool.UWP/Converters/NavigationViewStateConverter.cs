using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace HandSchool.Views
{
    /// <summary>
    /// NavigationView的显示状态与标题栏宽度的转换器。
    /// </summary>
    public sealed class NavigationViewStateConverter : IValueConverter
    {
        Thickness MinimalMargin { get; } = new Thickness(-80, 28, 0, 0);
        Thickness OtherMargin { get; } = new Thickness(12, 28, 0, -8);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is NavigationViewDisplayMode _value)
            {
                return _value == NavigationViewDisplayMode.Minimal ? MinimalMargin : OtherMargin;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException();
        }
    }
}
