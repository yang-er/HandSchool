using HandSchool.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SelectTypePage : ViewObject
    {
        public SelectTypePage()
        {
            InitializeComponent();
            MySchool.ItemsSource = Core.Schools;
            MySchool.SelectedIndex = 0;
        }

        private void School_Tapped(object sender, EventArgs e)
        {
            var sl = sender as StackLayout;
            sl.BackgroundColor = Color.FromRgb(238, 238, 238);
            MySchool.SelectedItem = Core.Schools.Find((s) => s.SchoolId == "jlu");
            NextButton.IsEnabled = true;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var sch = MySchool.SelectedItem as ISchoolWrapper;
            Core.Configure.Write("hs.school.bin", sch.SchoolId);

            Core.App.InjectService(sch);
            sch.PreLoad();
            sch.PostLoad();

            iOS.App.Current.SetMainPage<MainPage>();
        }
    }
}