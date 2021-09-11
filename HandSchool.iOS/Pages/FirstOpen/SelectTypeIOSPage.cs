using System;
using HandSchool.Services;
using HandSchool.Views;
using Xamarin.Forms;

namespace HandSchool.iOS.Pages
{
    public class SelectTypeIOSPage : SelectTypePage
    {
        public SelectTypeIOSPage(){}
        protected override void Button_Clicked(object sender, EventArgs e)
        {
            var sch = MySch.SelectedItem as ISchoolWrapper;
            var page = Extends.GetViewObjInstance(sch.HelloPage, sch);
            Xamarin.Forms.Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(page));
            //iOS.App.Current.SetMainPage<MainPage>();
        }
    }
}
