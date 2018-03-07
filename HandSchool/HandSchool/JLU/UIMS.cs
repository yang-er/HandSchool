using HandSchool.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace HandSchool.JLU
{
    class UIMS : ISchoolSystem
    {
        public Dictionary<string, string> HeaderAttach { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Uri ServerUri => new Uri("http://uims.jlu.edu.cn/ntms/");

        public Uri LoginPageUri => new Uri("j_spring_security_check");

        public Dictionary<string, string> AvaliableOperations => throw new NotImplementedException();
        
        public CookieAwareWebClient WebClient { get; set; }

        public bool Login(string username, string password)
        {
            WebClient = new CookieAwareWebClient
            {
                BaseAddress = ServerUri.OriginalString,
                Encoding = Encoding.UTF8
            };

            WebClient.Cookie.Add(new Cookie("loginPage", "userLogin.jsp", "/ntms/", "uims.jlu.edu.cn"));
            WebClient.Cookie.Add(new Cookie("alu", username, "/ntms/", "uims.jlu.edu.cn"));
            WebClient.Cookie.Add(new Cookie("pwdStrength", "1", "/ntms/", "uims.jlu.edu.cn"));

            // Access Main Page To Create a JSESSIONID
            WebClient.DownloadString("");

            // Login
            WebClient.Headers.Set("Referer", "http://uims.jlu.edu.cn/ntms/userLogin.jsp?reason=nologin");
            WebClient.Headers.Set("Content-Type", "application/x-www-form-urlencoded");

            var loginData = new NameValueCollection
            {
                { "j_username", username },
                { "j_password", Helper.MD5("UIMS" + username + password, Encoding.UTF8) },
                { "mousepath", "" }
            };

            WebClient.UploadValues("j_spring_security_check", "POST", loginData);
            
            return int.Parse(WebClient.ResponseHeaders["Content-Length"]) == 7544;
        }

        public string Post(string url, string send)
        {
            WebClient.Headers.Set("Content-Type", "application/json");
            return WebClient.UploadString(url, "POST", send);
        }

        public string Get(string url)
        {
            return WebClient.DownloadString(url);
        }
    }
}
