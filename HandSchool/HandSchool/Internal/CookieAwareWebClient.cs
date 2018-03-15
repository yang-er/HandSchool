using System;
using System.Net;

namespace HandSchool.Internal
{
    // thanks to zhleiyang
    public class CookieAwareWebClient : WebClient
    {
        public CookieContainer Cookie = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest req)
            {
                req.CookieContainer = Cookie;
                req.AllowAutoRedirect = false;

                req.Timeout = 15000;
            }
            return request;
        }
    }
}
