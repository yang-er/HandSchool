using HandSchool.Internals;
using HandSchool.Services;
using HandSchool.ViewModels;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InitializePage : ViewObject
	{
		public InitializePage()
		{
			InitializeComponent();
		}

        public override void SetNavigationArguments(object param)
        {
            base.SetNavigationArguments(param);

            var sch = param as ISchoolWrapper;
            Core.Configure.Write("hs.school.bin", sch.SchoolId);

            Core.App.InjectService(sch);
            sch.PreLoad();
            sch.PostLoad();
            ExcuteLogic();
        }

        private async void ExcuteLogic()
        {
            await System.Threading.Tasks.Task.Delay(1200);

            jwxtCheck.Text = "";
            jwxtProgress.IsRunning = true;
            var result = await LoginViewModel.RequestAsync(Core.App.Service);
            jwxtProgress.IsRunning = false;
            jwxtCheck.Text = result ? "√" : "×";
            jwxtCheck.TextColor = result ? Color.DarkGreen : Color.Red;

            if (result && !((UIMS)Core.App.Service).OutsideSchool)
            {
                kcbCheck.Text = "";
                kcbProgress.IsRunning = true;
                await ScheduleViewModel.Instance.Refresh();
                kcbProgress.IsRunning = false;
                kcbCheck.Text = "√";
                kcbCheck.TextColor = Color.DarkGreen;

                gradeCheck.Text = "";
                gradeProgress.IsRunning = true;
                await GradePointViewModel.Instance.ExecuteLoadItemsCommand();
                gradeProgress.IsRunning = false;
                gradeCheck.Text = "√";
                gradeCheck.TextColor = Color.DarkGreen;
            }

            await System.Threading.Tasks.Task.Delay(500);
            await Navigation.PushAsync<WelcomePage>();
        }
    }
}