using HandSchool.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    class UIMS : ISchoolSystem
    {
        public CookieAwareWebClient WebClient { get; set; }
        private List<ISystemEntrance> methodList = new List<ISystemEntrance>();
        public List<ISystemEntrance> Methods => methodList;
        public NameValueCollection AttachInfomation { get; set; }
        public string ServerUri => "http://uims.jlu.edu.cn/ntms/";
        public RootObject LoginInfo { get; set; }
        public bool IsLogin { get; private set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Tips => "用户名为教学号，新生默认密码为身份证后六位（x小写）。";
        public string StorageFile => "jlu.user.json";
        
        public UIMS()
        {
            IsLogin = false;
            Username = App.ReadFile("jlu.uims.username.txt");
            if (Username != "") Password = App.ReadFile("jlu.uims.password.txt");
            string resp = App.ReadFile(StorageFile);
            if (resp == "") throw new NotImplementedException();
            LoginInfo = JSON<RootObject>(resp);
        }

        public bool Login()
        {
            if (Username == "" || Password == "")
            {
                throw new NotImplementedException("Show Login Panel Not Finished");
            }
            else
            {
                App.WriteFile("jlu.uims.username.txt", Username);
                App.WriteFile("jlu.uims.password.txt", Password);
            }
            
            WebClient = new CookieAwareWebClient
            {
                BaseAddress = ServerUri,
                Encoding = Encoding.UTF8
            };
            WebClient.Cookie.Add(new Cookie("loginPage", "userLogin.jsp", "/ntms/", "uims.jlu.edu.cn"));
            WebClient.Cookie.Add(new Cookie("alu", Username, "/ntms/", "uims.jlu.edu.cn"));
            WebClient.Cookie.Add(new Cookie("pwdStrength", "1", "/ntms/", "uims.jlu.edu.cn"));

            // Access Main Page To Create a JSESSIONID
            WebClient.DownloadString("");

            // Login
            WebClient.Headers.Set("Referer", "http://uims.jlu.edu.cn/ntms/userLogin.jsp?reason=nologin");
            WebClient.Headers.Set("Content-Type", "application/x-www-form-urlencoded");

            var loginData = new NameValueCollection
            {
                { "j_username", Username },
                { "j_password", MD5("UIMS" + Username + Password, Encoding.UTF8) },
                { "mousepath", "" }
            };

            WebClient.UploadValues("j_spring_security_check", "POST", loginData);

            // Get User Info
            string resp = WebClient.UploadString("action/getCurrentUserInfo.do", "POST", "");
            if (WebClient.ResponseHeaders["Content-Type"].StartsWith("application/json"))
            {
                App.WriteFile(StorageFile, resp);
                LoginInfo = JSON<RootObject>(resp);
                IsLogin = true;
                return true;
            }
            else
            {
                IsLogin = false;
                return false;
            }
        }
        
        public string PostJson(string url, string send)
        {
            if (!IsLogin) Login();
            if (!IsLogin) throw new NotImplementedException("登录失败");

            WebClient.Headers.Set("Content-Type", "application/json");
            var ret = WebClient.UploadString(url, "POST", send);

            // retry once
            if (!WebClient.ResponseHeaders["Content-Type"].StartsWith("application/json"))
            {
                IsLogin = false;
                return PostJson(url, send);
            }
            
            return ret;
        }

        public string Get(string url)
        {
            if (!IsLogin) Login();
            if (!IsLogin) throw new NotImplementedException("登录失败");

            var ret = WebClient.DownloadString(url);

            // retry once
            if (!WebClient.ResponseHeaders["Content-Type"].StartsWith("application/json"))
            {
                IsLogin = false;
                return Get(url);
            }

            return ret;
        }
        
        public class RootObject
        {
            public string loginMethod { get; set; }
            public CacheUpdate cacheUpdate { get; set; }
            public string[] menusFile { get; set; }
            public int trulySch { get; set; }
            public GroupsInfo[] groupsInfo { get; set; }
            public string firstLogin { get; set; }
            public DefRes defRes { get; set; }
            public string userType { get; set; }
            public DateTime sysTime { get; set; }
            public string nickName { get; set; }
            public int userId { get; set; }
            public string welcome { get; set; }
            public string loginName { get; set; }

            public class CacheUpdate
            {
                public long sysDict { get; set; }
                public long code { get; set; }
            }

            public class DefRes
            {
                public int adcId { get; set; }
                public int term_l { get; set; }
                public int university { get; set; }
                public int teachingTerm { get; set; }
                public int school { get; set; }
                public int department { get; set; }
                public int term_a { get; set; }
                public int schType { get; set; }
                public int personId { get; set; }
                public int year { get; set; }
                public int term_s { get; set; }
                public int campus { get; set; }
            }

            public class GroupsInfo
            {
                public int groupId { get; set; }
                public string groupName { get; set; }
                public string menuFile { get; set; }
            }
        }
    }
}
