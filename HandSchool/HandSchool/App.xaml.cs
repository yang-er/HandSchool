using System.Collections.Generic;
using System.IO;
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

        public Dictionary<string, LoadSchool> Support = new Dictionary<string, LoadSchool>();

        public App ()
		{
			InitializeComponent();
            Support.Add("吉林大学", LoadJLU);
            LoadJLU();
            MainPage = new MainPage();
        }

        private void LoadJLU()
        {
            Service = new JLU.UIMS
            {
                Username = "55170922",
                Password = "252015"
            };
            GradePoint = new JLU.GradeEntrance();
            GPA = new JLU.GPA();
        }

        public delegate void LoadSchool();

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
