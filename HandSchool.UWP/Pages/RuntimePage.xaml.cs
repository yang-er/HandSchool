using System;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HandSchool.Views
{
    public sealed partial class RuntimePage : Page
    {
        public RuntimePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadedAssemblies.ItemsSource = AppDomain.CurrentDomain.GetAssemblies();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (LoadedAssemblies.SelectedItem is Assembly assembly)
            {
                LoadedTypes.ItemsSource = (from s in assembly.DefinedTypes select new RTTIType(s)).ToList();
                AssemblyInfo.DataContext = new RTTIAssembly(assembly);
            }
            else
            {
                LoadedTypes.ItemsSource = null;
                AssemblyInfo.DataContext = null;
            }
        }

        public class RTTIType
        {
            readonly TypeInfo inner;
            public RTTIType(TypeInfo info) => inner = info;
            public string FullName { get => inner.FullName; set => value.ToString(); }
        }

        public class RTTIAssembly
        {
            readonly Assembly inner;
            public RTTIAssembly(Assembly info) => inner = info;
            public string FullName { get => inner.FullName; set => value.ToString(); }
            public string CodeBase { get => inner.CodeBase; set => value.ToString(); }
            public string ImageRuntimeVersion { get => inner.ImageRuntimeVersion; set => value.ToString(); }
        }
    }
}
