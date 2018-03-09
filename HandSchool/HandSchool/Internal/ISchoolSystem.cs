using System.Collections.Generic;
using System.Collections.Specialized;

namespace HandSchool.Internal
{
    public interface ISchoolSystem
    {
        string ServerUri { get; }
        CookieAwareWebClient WebClient { get; set; }
        NameValueCollection AttachInfomation { get; set; }
        List<ISystemEntrance> Methods { get; }
        bool IsLogin { get; }
        bool Login(string username, string password);
        string Post(string url, string send);
        string Get(string url);
    }
}
