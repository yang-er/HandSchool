using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : TabbedPage
	{
		public MainPage ()
		{
			InitializeComponent();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(App.Service.NeedLogin && !App.Service.IsLogin)
            {
                Navigation.PushModalAsync(new LoginPage());
            }
        }
    }
}