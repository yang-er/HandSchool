using System.Threading.Tasks;
using HandSchool.Internals;
using HandSchool.ViewModels;

namespace HandSchool.ViewModels
{
    public class NoticeCheckViewModel : BaseViewModel
    {
        protected virtual async Task<bool> CheckEnvAndNotice(string actionName)
        {
            IsBusy = true;
            var resp = await CheckEnv(actionName);
            if (!resp)
            {
                var str = resp.ToString();
                if (str.IsNotBlank())
                {
                    await NoticeError(str);
                    return false;
                }
            }

            IsBusy = false;
            return true;
        }
    }
}