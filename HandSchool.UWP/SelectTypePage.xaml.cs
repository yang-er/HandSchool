using HandSchool.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
    }
}
