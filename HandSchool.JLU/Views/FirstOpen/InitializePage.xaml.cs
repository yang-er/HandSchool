using System.Threading.Tasks;
using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InitializePage
    {
        public InitializePage()
        {
            InitializeComponent();
        }

        public override void SetNavigationArguments(object param)
        {
            base.SetNavigationArguments(param);

            var sch = (ISchoolWrapper) param;
            Core.Configure.Configs.InsertOrUpdateTable(new Config
            {
                ConfigName = "school",
                Value = sch.SchoolId
            });
            Core.App.InjectService(sch);
            sch.PreLoad();
            sch.PostLoad();
            ExecuteLogic();
        }

        private async void ExecuteLogic()
        {
            var vpn = Vpn.UseVpn;
            var vpnLogin = false;
            if (vpn)
            {
                await Task.Delay(1200);
                vpnCheck.Text = "";
                vpnProgress.IsRunning = true;
                vpnLogin = await LoginViewModel.RequestAsync(Loader.Vpn) == RequestLoginState.Success;
                vpnProgress.IsRunning = false;
                vpnCheck.Text = vpnLogin ? "√" : "×";
                vpnCheck.TextColor = vpnLogin ? Color.DarkGreen : Color.Red;
            }
            else
            {
                vpnCheck.Text = "N";
                vpnCheck.TextColor = Color.Blue;
            }

            if (!vpn || vpnLogin)
            {
                await Task.Delay(1200);

                jwxtCheck.Text = "";
                jwxtProgress.IsRunning = true;
                var result = await LoginViewModel.RequestAsync(Core.App.Service) == RequestLoginState.Success;
                jwxtProgress.IsRunning = false;
                jwxtCheck.Text = result ? "√" : "×";
                jwxtCheck.TextColor = result ? Color.DarkGreen : Color.Red;

                if (result && !((UIMS) Core.App.Service).OutsideSchool)
                {
                    kcbCheck.Text = "";
                    kcbProgress.IsRunning = true;
                    gradeCheck.Text = "";
                    gradeProgress.IsRunning = true;
                    await Task.WhenAll(
                        ScheduleViewModel.Instance.Refresh()
                            .ContinueWith(async t =>
                            {
                                await t;
                                Core.Platform.EnsureOnMainThread(() =>
                                {
                                    kcbProgress.IsRunning = false;
                                    kcbCheck.Text = "√";
                                    kcbCheck.TextColor = Color.DarkGreen;
                                });
                            }),
                        Task.WhenAll(Core.App.GradePoint.Execute(), Core.App.GradePoint.EntranceAll())
                            .ContinueWith(async t =>
                            {
                                await t;
                                Core.Platform.EnsureOnMainThread(() =>
                                {
                                    gradeProgress.IsRunning = false;
                                    gradeCheck.Text = "√";
                                    gradeCheck.TextColor = Color.DarkGreen;
                                });
                            })
                    );
                }
            }

            await Task.Delay(500);
            await Navigation.PushAsync<WelcomePage>();
        }
    }
}