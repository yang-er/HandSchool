using System;
using HandSchool.Internal;
using HandSchool.Views;
using Xamarin.Forms;

namespace HandSchool
{
	public partial class App : Application
	{

        public static ISchoolSystem Service;
        public static ISystemEntrance GradePoint;
        public static ISystemEntrance Schedule;
        public static ISystemEntrance GPA;
        public static ISystemEntrance SelectCourse;
        public static string DataBaseDir;
        
		public App ()
		{
			InitializeComponent();
            LoadJLU();

            MainPage = new MainPage();
        }

        private void LoadJLU()
        {
            Service = new JLU.UIMS();
            Service.Login("55170922", "252015");
            GradePoint = new JLU.GradeEntrance();
            GPA = new JLU.GPA();
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
