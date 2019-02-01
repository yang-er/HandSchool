using HandSchool.Internal;
using HandSchool.ViewModels;
using System.Threading.Tasks;

namespace HandSchool.Droid
{
    class CodeStore
    {
        public void FinishSettings()
        {
            Core.App.Service.RequestLogin().ContinueWith(LoginSuccessContinue);
        }

        private void LoginSuccessContinue(Task<bool> success)
        {
            if (success.Result)
            {
                if (Core.App.Schedule != null)
                {
                    ScheduleViewModel.Instance.RefreshCommand.Execute(null);
                    if (Core.App.GradePoint != null)
                    {
                        ScheduleViewModel.Instance.RefreshComplete += ScheduleComplete;
                    }
                }
                else if (Core.App.GradePoint != null)
                {
                    GradePointViewModel.Instance.LoadItemsCommand.Execute(null);
                }
            }
        }

        private void ScheduleComplete()
        {
            GradePointViewModel.Instance.LoadItemsCommand.Execute(null);
            ScheduleViewModel.Instance.RefreshComplete -= ScheduleComplete;
        }
    }
}