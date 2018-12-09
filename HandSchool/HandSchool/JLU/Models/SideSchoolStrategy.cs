using System.Threading.Tasks;

namespace HandSchool.JLU
{
    partial class UIMS
    {
        interface ISideSchoolStrategy
        {
            string TimeoutUrl { get; }
            Task<bool> LoginSide();
            void OnLoad();
            string FormatArguments(string input);
            string WelcomeMessage { get; }
            string CurrentMessage { get; }
        }
    }
}
