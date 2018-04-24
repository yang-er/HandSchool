using HandSchool.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static HandSchool.Internal.Helper;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SelectTypePage : ContentPage
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
            WriteConfFile("hs.school.bin", sch.SchoolId);

            sch.PreLoad();
            sch.PostLoad();

#if !__UWP__
            (Application.Current.MainPage as MainPage).FinishSettings();
#endif
        }
    }
}