using Xamarin.Forms;

namespace HandSchool.Views
{
	public class TabMainPage : TabbedPage
	{
		public TabMainPage ()
		{
            App.Current.PrimaryItems.ForEach((obj) => Children.Add(obj.DestPage));
            App.Current.SecondaryItems.ForEach((obj) => Children.Add(obj.DestPage));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (App.Current.Service.NeedLogin && !App.Current.Service.IsLogin)
            {
                (new LoginPage()).ShowAsync();
            }
        }
    }
}