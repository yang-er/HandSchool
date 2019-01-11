using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HandSchool.UWP
{
    internal static class ExtensionsImpl
    {
        public static void SetBinding(this FrameworkElement element,
            DependencyProperty dp, string path, object src = null, 
            BindingMode mode = BindingMode.TwoWay, IValueConverter cvt = null)
        {
            element.SetBinding(dp, new Binding
            {
                Path = new PropertyPath(path),
                Mode = mode,
                Source = src,
                Converter = cvt,
            });
        }
    }
}
