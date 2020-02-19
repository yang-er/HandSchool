using HandSchool.Internal;
using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.Views;
using System.Threading.Tasks;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("JLU", "健康状态填报", "填报每天的健康状况~", EntranceType.InfoEntrance)]
    class EhallFill : ITapEntrace
    {
        const string EhallFills = "https://ehall.jlu.edu.cn/infoplus/form/JLDX_BK_XNYQSB/start";
        
        public Task Action(INavigate navigate)
        {
            Core.Platform.OpenUrl(EhallFills);
            return Task.CompletedTask;
        }
    }
}
