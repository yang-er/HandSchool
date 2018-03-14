using HandSchool.Internal;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
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
        public bool RebuildRequest { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Tips => "用户名为教学号，新生默认密码为身份证后六位（x小写）。";
        public string StorageFile => "jlu.user.json";
        public bool NeedLogin { get; private set; }
        public string InnerError { get; private set; }
        
        public UIMS()
        {
            IsLogin = false;
            NeedLogin = false;
            Username = ReadConfFile("jlu.uims.username.txt");
            AttachInfomation = new NameValueCollection();
            if (Username != "") Password = ReadConfFile("jlu.uims.password.txt");
            if (Password == "") SavePassword = false;
            //App.WriteFile(StorageFile, "");
            string resp = ReadConfFile(StorageFile);
            if (resp == "")
            {
                AutoLogin = false;
                NeedLogin = true;
                return;
            }
            LoginInfo = JSON<RootObject>(resp);
            ParseLoginInfo();
        }

        private void ParseLoginInfo()
        {
            AttachInfomation.Clear();
            AttachInfomation.Add("studId", LoginInfo.userId.ToString());
            AttachInfomation.Add("term", LoginInfo.defRes.term_l.ToString());
        }

        public async Task<bool> Login()
        {
            RebuildRequest = false;

            if (Username == "" || Password == "")
            {
                NeedLogin = true;
                throw new NotImplementedException("Show Login Panel Not Finished");
            }
            else
            {
                WriteConfFile("jlu.uims.username.txt", Username);
                WriteConfFile("jlu.uims.password.txt", SavePassword ? Password : "");
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
            try
            {
                await WebClient.DownloadStringTaskAsync("");
            }
            catch(WebException e)
            {
                InnerError = e.Message;
                return false;
            }

            // Set Login Session
            WebClient.Headers.Set("Referer", "http://uims.jlu.edu.cn/ntms/userLogin.jsp?reason=nologin");
            WebClient.Headers.Set("Content-Type", "application/x-www-form-urlencoded");

            var loginData = new NameValueCollection
            {
                { "j_username", Username },
                { "j_password", MD5("UIMS" + Username + Password, Encoding.UTF8) },
                { "mousepath", "" }
            };

            await WebClient.UploadValuesTaskAsync("j_spring_security_check", "POST", loginData);

            // Get User Info
            string resp = await WebClient.UploadStringTaskAsync("action/getCurrentUserInfo.do", "POST", "");
            if (WebClient.ResponseHeaders["Content-Type"].StartsWith("application/json"))
            {
                WriteConfFile(StorageFile, AutoLogin ? resp : "");
                LoginInfo = JSON<RootObject>(resp);
                ParseLoginInfo();
                IsLogin = true;
                return true;
            }
            else
            {
                IsLogin = false;
                return false;
            }
        }
        
        public async Task<string> PostJson(string url, string send)
        {
            if (!IsLogin) await Login();
            if (!IsLogin)
            {
                await (new LoginPage()).ShowAsync();
            }

            WebClient.Headers.Set("Content-Type", "application/json");
            var ret = await WebClient.UploadStringTaskAsync(url, "POST", send);

            // retry once
            if (!WebClient.ResponseHeaders["Content-Type"].StartsWith("application/json"))
            {
                throw new NotImplementedException();
            }
            
            return ret;
        }

        public async Task<string> Get(string url)
        {
            if (!IsLogin) await Login();
            if (!IsLogin) throw new NotImplementedException("登录失败");

            var ret = await WebClient.DownloadStringTaskAsync(url);

            // retry once
            if (!WebClient.ResponseHeaders["Content-Type"].StartsWith("application/json"))
            {
                IsLogin = false;
                if (!RebuildRequest)
                    return await Get(url);
                else
                    throw new NotImplementedException();
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

        public class NotLoginException : Exception {}
    }
}
