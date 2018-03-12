using System.Collections.Generic;
using System.IO;
using HandSchool.Internal;
using HandSchool.Views;
using Xamarin.Forms;

namespace HandSchool
{
	public partial class App : Application
	{

        #region School Components

        public static ISchoolSystem Service;
        public static ISystemEntrance GradePoint;
        public static ISystemEntrance Schedule;
        public static ISystemEntrance GPA;
        public static ISystemEntrance SelectCourse;
        public static string DataBaseDir;
        public static int DailyClassCount;

        public static string ReadFile(string name)
        {
            string fn = Path.Combine(DataBaseDir, name);
            if (File.Exists(fn))
                return File.ReadAllText(Path.Combine(DataBaseDir, name));
            else
                return "";
        }

        public static void WriteFile(string name, string value)
        {
            File.WriteAllText(Path.Combine(DataBaseDir, name), value);
        }

        #endregion
        
        public App ()
		{
			InitializeComponent();
            Support.Add("吉林大学", LoadJLU);
            LoadJLU();
            MainPage = new MainPage();
        }

        #region Load School

        public delegate void LoadSchool();
        public Dictionary<string, LoadSchool> Support = new Dictionary<string, LoadSchool>();

        private void LoadJLU()
        {
            Service = new JLU.UIMS();
            DailyClassCount = 11;
            GradePoint = new JLU.GradeEntrance();
            Schedule = new JLU.Schedule();
            GPA = new JLU.GPA();
        }
        
        #endregion

        #region State Change

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

        #endregion

    }
}
