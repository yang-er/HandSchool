using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    class UIMS : ISchoolSystem
    {
        public AwaredWebClient WebClient { get; set; }
        private List<ISystemEntrance> methodList = new List<ISystemEntrance>();
        public event PropertyChangedEventHandler PropertyChanged;
        public List<ISystemEntrance> Methods => methodList;
        public NameValueCollection AttachInfomation { get; set; }
        public string ServerUri => "http://uims.jlu.edu.cn/ntms/";
        public LoginValue LoginInfo { get; set; }
        public bool IsLogin { get; private set; }
        public bool RebuildRequest { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Tips => "用户名为教学号，新生默认密码为身份证后六位（x小写）。";
        public bool NeedLogin { get; private set; }
        public string InnerError { get; private set; }
        public int CurrentWeek { get; set; }
        public string WelcomeMessage => NeedLogin ? "请登录" : $"欢迎，{AttachInfomation["studName"]}。";
        public string CurrentMessage => NeedLogin ? DateTime.Now.ToShortDateString() : $"{AttachInfomation["Nick"]}第{CurrentWeek}周";
        public UIMS()
        {
            IsLogin = false;
            NeedLogin = false;
            Username = ReadConfFile("jlu.uims.username.txt");
            AttachInfomation = new NameValueCollection();
            if (Username != "") Password = ReadConfFile("jlu.uims.password.txt");
            if (Password == "") SavePassword = false;

            try
            {
                ParseLoginInfo(ReadConfFile("jlu.user.json"));
                ParseTermInfo(ReadConfFile("jlu.teachingterm.json"));
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                AutoLogin = false;
                NeedLogin = true;
            }
        }

        private void ParseLoginInfo(string resp)
        {
            LoginInfo = JSON<LoginValue>(resp);
            AttachInfomation.Add("studId", LoginInfo.userId.ToString());
            AttachInfomation.Add("studName", LoginInfo.nickName);
            AttachInfomation.Add("term", LoginInfo.defRes.teachingTerm.ToString());
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WelcomeMessage"));
        }

        private void ParseTermInfo(string resp)
        {
            var ro = JSON<RootObject<TeachingTerm>>(resp).value[0];
            if (ro.vacationDate < DateTime.Now)
            {
                AttachInfomation.Add("Nick", ro.year + "学年" + (ro.termSeq == "1" ? "寒假" : "暑假"));
                CurrentWeek = (int) Math.Ceiling((decimal) ((DateTime.Now - ro.vacationDate).Days + 1) / 7);
            }
            else
            {
                AttachInfomation.Add("Nick", ro.year + "学年" + (ro.termSeq == "1" ? "秋季学期" : (ro.termSeq == "2" ? "春季学期" : "短学期")));
                CurrentWeek = (int)Math.Ceiling((decimal) ((DateTime.Now - ro.startDate).Days + 1) / 7);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentWeek"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentMessage"));
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

            WebClient = new AwaredWebClient(ServerUri, Encoding.UTF8);
            WebClient.Cookie.Add(new Cookie("loginPage", "userLogin.jsp", "/ntms/", "uims.jlu.edu.cn"));
            WebClient.Cookie.Add(new Cookie("alu", Username, "/ntms/", "uims.jlu.edu.cn"));
            WebClient.Cookie.Add(new Cookie("pwdStrength", "1", "/ntms/", "uims.jlu.edu.cn"));

            // Access Main Page To Create a JSESSIONID
            try
            {
                await WebClient.GetAsync("", "*/*");
            }
            catch(WebException ex)
            {
                InnerError = GetWebExceptionMessage(ex);
                return false;
            }

            // Set Login Session
            var loginData = new NameValueCollection
            {
                { "j_username", Username },
                { "j_password", MD5("UIMS" + Username + Password, Encoding.UTF8) },
                { "mousepath", "" }
            };

            WebClient.Headers.Set("Referer", ServerUri + "userLogin.jsp?reason=nologin");
            await WebClient.PostAsync("j_spring_security_check", loginData);

            if (WebClient.Location == "error/dispatch.jsp?reason=loginError")
            {
                string result = await WebClient.GetAsync("userLogin.jsp?reason=loginError", "text/html");
                InnerError = Regex.Match(result, @"<span class=""error_message"" id=""error_message"">登录错误：(\S+)</span>").Groups[1].Value;
                IsLogin = false;
                return false;
            }
            else if (WebClient.Location == "index.do")
            {
                AttachInfomation.Clear();

                // Get User Info
                string resp = await WebClient.PostAsync("action/getCurrentUserInfo.do", "", "application/x-www-form-urlencoded");
                if (resp.StartsWith("<!")) return false;
                WriteConfFile("jlu.user.json", AutoLogin ? resp : "");
                ParseLoginInfo(resp);

                // Get term info
                resp = await WebClient.PostAsync("service/res.do", "{\"tag\":\"search@teachingTerm\",\"branch\":\"byId\",\"params\":{\"termId\":" + AttachInfomation["term"] + "}}");
                if (resp.StartsWith("<!")) return false;
                WriteConfFile("jlu.teachingterm.json", AutoLogin ? resp : "");
                ParseTermInfo(resp);
            }
            else
            {
                throw new NotImplementedException("Not implemented response");
            }

            IsLogin = true;
            return true;
        }
        
        public async Task<string> Post(string url, string send)
        {
            if (!IsLogin) await Login();
            if (!IsLogin) await (new LoginPage()).ShowAsync();
            return await WebClient.PostAsync(url, send);
        }

        public async Task<string> Get(string url)
        {
            if (!IsLogin) await Login();
            if (!IsLogin) await (new LoginPage()).ShowAsync();
            return await WebClient.GetAsync(url);
        }
        
        public class LoginValue
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
