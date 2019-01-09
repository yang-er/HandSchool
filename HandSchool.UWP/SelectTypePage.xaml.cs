using HandSchool.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace HandSchool.Views
{
    public sealed partial class SelectTypePage : Page
    {
        public SelectTypePage()
        {
            InitializeComponent();
            MySchool.ItemsSource = Core.Schools;
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sch = MySchool.SelectedItem as ISchoolWrapper;
            Core.Configure.Write("hs.school.bin", sch.SchoolId);

            Core.App.InjectService(sch);
            sch.PreLoad();
            sch.PostLoad();

            Frame.Navigate(typeof(MainPage));
        }

        private void MySchool_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void MenuFlyoutItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(RuntimePage));
        }
    }
}
