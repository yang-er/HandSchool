using HandSchool.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SelectTypePage : PopContentPage
    {
        public SelectTypePage()
        {
            InitializeComponent();
            MySchool.ItemsSource = Core.Schools;
            MySchool.SelectedIndex = 0;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var sch = MySchool.SelectedItem as ISchoolWrapper;
            Core.WriteConfig("hs.school.bin", sch.SchoolId);

            Core.App.InjectService(sch);
            sch.PreLoad();
            sch.PostLoad();

#if !__UWP__
            (Application.Current.MainPage as MainPage).FinishSettings();
#endif
        }
    }
}