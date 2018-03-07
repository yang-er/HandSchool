using System;
using System.Collections.Generic;
using System.Net;

namespace HandSchool.Internal
{
    interface ISchoolSystem
    {
        Dictionary<string, string> HeaderAttach { get; set; }
        Uri LoginPageUri { get; }
        Uri ServerUri { get; }
        Dictionary<string, string> AvaliableOperations { get; }
        CookieAwareWebClient WebClient { get; set; }
        bool Login(string username, string password);
        string Post(string url, string send);
        string Get(string url);
    }
}
