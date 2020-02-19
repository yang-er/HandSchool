using HandSchool.Views;
using System.Threading.Tasks;

namespace HandSchool.Internal
{
    public interface ITapEntrace
    {
        Task Action(INavigate navigate);
    }
}
