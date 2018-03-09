using System;
using HandSchool.Internal;
using HandSchool.Views;
using Xamarin.Forms;

namespace HandSchool
{
	public partial class App : Application
	{

        public static ISchoolSystem Service;
        


		public App ()
		{
			InitializeComponent();
            Service = new JLU.UIMS();
            Service.Login("55170922", "252015");
            

            MainPage = new GradePointPage();
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
