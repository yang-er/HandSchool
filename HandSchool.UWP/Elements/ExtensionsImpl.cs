using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HandSchool.Internal
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
