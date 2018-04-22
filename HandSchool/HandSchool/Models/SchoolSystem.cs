using HandSchool.Internal;
using HandSchool.Models;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace HandSchool.Services
{
    public interface ISchoolSystem : ILoginField
    {
        string ServerUri { get; }
        AwaredWebClient WebClient { get; set; }
        NameValueCollection AttachInfomation { get; set; }
        int CurrentWeek { get; set; }
        string WelcomeMessage { get; }
        string CurrentMessage { get; }
        string WeatherLocation { get; }
        Task<string> Post(string url, string send);
        Task<string> Get(string url);
        Task<bool> RequestLogin();
        void SaveSettings();
    }
}
