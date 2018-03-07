using System;
using System.Net;

namespace HandSchool.Internal
{
    public class CookieAwareWebClient : WebClient
    {
        public CookieContainer Cookie = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = Cookie;
            }
            return request;
        }
    }
}
