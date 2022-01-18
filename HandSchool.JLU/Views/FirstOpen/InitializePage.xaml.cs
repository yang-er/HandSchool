using HandSchool.Internals;
using HandSchool.JLU.Services;
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
            Core.Configure.Write(Core.ConfigSchool, sch.SchoolId);

            Core.App.InjectService(sch);
            sch.PreLoad();
            sch.PostLoad();
            ExecuteLogic();
        }

        private async void ExecuteLogic()
        {
            var vpn = WebVpn.UseVpn;
            var result2 = false;
            if (vpn)
            {
                await System.Threading.Tasks.Task.Delay(1200);
                vpnCheck.Text = "";
                vpnProgress.IsRunning = true;
                result2 = await LoginViewModel.RequestAsync(Loader.Vpn) == RequestLoginState.Success;
                vpnProgress.IsRunning = false;
                vpnCheck.Text = result2 ? "√" : "×";
                vpnCheck.TextColor = result2 ? Color.DarkGreen : Color.Red;
            }
            else
            {
                vpnCheck.Text = "N";
                vpnCheck.TextColor = Color.Blue;
            }
            if (!vpn || result2)
            {
                await System.Threading.Tasks.Task.Delay(1200);

                jwxtCheck.Text = "";
                jwxtProgress.IsRunning = true;
                var result = await LoginViewModel.RequestAsync(Core.App.Service) == RequestLoginState.Success;
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
                    await GradePointViewModel.Instance.ExecuteLoadNewerItemsCommand();
                    await GradePointViewModel.Instance.ExecuteLoadAllItemsCommand();
                    gradeProgress.IsRunning = false;
                    gradeCheck.Text = "√";
                    gradeCheck.TextColor = Color.DarkGreen;
                }
            }

            await System.Threading.Tasks.Task.Delay(500);
            await Navigation.PushAsync<WelcomePage>();
        }
    }
}