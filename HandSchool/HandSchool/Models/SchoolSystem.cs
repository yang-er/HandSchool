using HandSchool.Internal;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HandSchool
{
    public interface ISchoolSystem : INotifyPropertyChanged
    {
        string ServerUri { get; }
        AwaredWebClient WebClient { get; set; }
        NameValueCollection AttachInfomation { get; set; }
        List<ISystemEntrance> Methods { get; }
        string Tips { get; }
        string Username { get; set; }
        string Password { get; set; }
        bool IsLogin { get; }
        bool NeedLogin { get; }
        bool RebuildRequest { get; set; }
        string InnerError { get; }
        int CurrentWeek { get; set; }
        string WelcomeMessage { get; }
        string CurrentMessage { get; }
        Task<bool> Login();
        Task<string> Post(string url, string send);
        Task<string> Get(string url);
    }
}
