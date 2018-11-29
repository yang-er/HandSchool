using HandSchool.UWP;
using HandSchool.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace HandSchool.Views
{
    public sealed partial class IndexPage : ViewPage
    {
        static bool FirstOpen = true;
        public int TextSize = 20;
        public int LineLenth = 500;
        IndexPageDataAdapter Adapter = IndexPageDataAdapter.Instance;

        public IndexPage()
        {
            InitializeComponent();
            ViewModel = IndexViewModel.Instance;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (FirstOpen)
            {
                FirstOpen = false;
                IndexViewModel.Instance.RefreshCommand.Execute(null);
            }
        }
    }

    public class VisibilityConverter : IValueConverter
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
