using HandSchool.Internal;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace HandSchool
{
    public interface ISchoolSystem
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
        int CurrentWeek { get; }
        Task<bool> Login();
        Task<string> PostJson(string url, string send);
        Task<string> Get(string url);
    }
}
