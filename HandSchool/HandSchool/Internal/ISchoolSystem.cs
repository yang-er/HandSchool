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
        string Tips { get; }
        string Username { get; set; }
        string Password { get; set; }
        bool IsLogin { get; }
        bool Login();
        string PostJson(string url, string send);
        string Get(string url);
    }
}
