using System.Collections.Generic;
using System.Collections.Specialized;

namespace HandSchool.Internal
{
    interface ISchoolSystem
    {
        string ServerUri { get; }
        Dictionary<string, string> HeaderAttach { get; set; }
        CookieAwareWebClient WebClient { get; set; }
        NameValueCollection AttachInfomation { get; set; }
        List<ISystemEntrance> Methods { get; }
        bool Login(string username, string password);
        string Post(string url, string send);
        string Get(string url);
    }
}
