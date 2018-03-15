using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
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
        public LoginValue LoginInfo { get; set; }
        public bool IsLogin { get; private set; }
        public bool RebuildRequest { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Tips => "用户名为教学号，新生默认密码为身份证后六位（x小写）。";
        public string StorageFile => "jlu.user.json";
        public bool NeedLogin { get; private set; }
        public string InnerError { get; private set; }
        public int CurrentWeek { get; private set; }

        public UIMS()
        {
            IsLogin = false;
            NeedLogin = false;
            Username = ReadConfFile("jlu.uims.username.txt");
            AttachInfomation = new NameValueCollection();
            if (Username != "") Password = ReadConfFile("jlu.uims.password.txt");
            if (Password == "") SavePassword = false;
            string resp = ReadConfFile(StorageFile);
            if (resp == "")
            {
                AutoLogin = false;
                NeedLogin = true;
            }
            else
            {
                ParseLoginInfo(resp);
            }
            resp = ReadConfFile("jlu.teachingterm.json");
            if (resp == "")
            {
                AutoLogin = false;
                NeedLogin = true;
            }
            else
            {
                ParseTermInfo(resp);
            }
        }

        private void ParseLoginInfo(string resp)
        {
            LoginInfo = JSON<LoginValue>(resp);
            AttachInfomation.Clear();
            AttachInfomation.Add("studId", LoginInfo.userId.ToString());
            AttachInfomation.Add("studName", LoginInfo.nickName);
            AttachInfomation.Add("term", LoginInfo.defRes.teachingTerm.ToString());
        }

        private void ParseTermInfo(string resp)
        {
            var ro = JSON<RootObject<TeachingTerm>>(resp).value[0];
            if (ro.vacationDate < DateTime.Now)
            {
                AttachInfomation.Add("Nick", (int.Parse(ro.year) + 1) + "年" + (ro.termSeq == "1" ? "寒假" : "暑假"));
                CurrentWeek = (int) Math.Ceiling((decimal) (DateTime.Now - ro.vacationDate).Days / 7);
            }
            else
            {
                AttachInfomation.Add("Nick", ro.year + "年" + (ro.termSeq == "1" ? "秋季学期" : (ro.termSeq == "2" ? "春季学期" : "短学期")));
                CurrentWeek = (int)Math.Ceiling((decimal)(DateTime.Now - ro.startDate).Days / 7);
            }
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
            bool connected = false;
            try
            {
                await WebClient.DownloadStringTaskAsync("index.do");
            }
            catch(WebException e)
            {
                switch (e.Status)
                {
                    case WebExceptionStatus.Success:
                        InnerError = "似乎一切正常。\n" + e.StackTrace;
                        break;
                    case WebExceptionStatus.NameResolutionFailure:
                        InnerError = "域名解析失败，未连接到互联网";
                        break;
                    case WebExceptionStatus.ConnectFailure:
                        InnerError = "连接服务器失败，未连接到校内网络";
                        break;
                    case WebExceptionStatus.ReceiveFailure:
                    case WebExceptionStatus.SendFailure:
                    case WebExceptionStatus.PipelineFailure:
                    case WebExceptionStatus.RequestCanceled:
                    case WebExceptionStatus.ConnectionClosed:
                        InnerError = "数据包传输出现错误";
                        break;
                    case WebExceptionStatus.TrustFailure:
                    case WebExceptionStatus.SecureChannelFailure:
                    case WebExceptionStatus.ServerProtocolViolation:
                    case WebExceptionStatus.KeepAliveFailure:
                        InnerError = "网络沟通出现错误";
                        break;
                    case WebExceptionStatus.Pending:
                    case WebExceptionStatus.Timeout:
                        InnerError = "连接超时，可能是您的网络不太好";
                        break;
                    case WebExceptionStatus.ProxyNameResolutionFailure:
                    case WebExceptionStatus.UnknownError:
                    case WebExceptionStatus.MessageLengthLimitExceeded:
                    case WebExceptionStatus.CacheEntryNotFound:
                    case WebExceptionStatus.RequestProhibitedByCachePolicy:
                    case WebExceptionStatus.RequestProhibitedByProxy:
                    default:
                        InnerError = e.Status.ToString();
                        break;
                }
                if (!connected) return false;
            }

            // Set Login Session
            var loginData = new NameValueCollection
            {
                { "j_username", Username },
                { "j_password", MD5("UIMS" + Username + Password, Encoding.UTF8) },
                { "mousepath", "" }
            };

            WebClient.Headers.Set("Referer", "http://uims.jlu.edu.cn/ntms/userLogin.jsp?reason=nologin");
            await WebClient.UploadValuesTaskAsync("j_spring_security_check", "POST", loginData);

            if (WebClient.ResponseHeaders["Location"] == "http://uims.jlu.edu.cn/ntms/error/dispatch.jsp?reason=loginError")
            {
                string result = await WebClient.DownloadStringTaskAsync("http://uims.jlu.edu.cn/ntms/userLogin.jsp?reason=loginError");
                InnerError = System.Text.RegularExpressions.Regex.Match(result, @"<span class=""error_message"" id=""error_message"">登录错误：(\S+)</span>").Groups[1].Value;
                IsLogin = false;
                return false;
            }

            // Get User Info
            WebClient.Headers.Set("Accept", "application/json");
            string resp = await WebClient.UploadStringTaskAsync("action/getCurrentUserInfo.do", "POST", "");
            WriteConfFile(StorageFile, AutoLogin ? resp : "");
            // retry once
            if (!WebClient.ResponseHeaders["Content-Type"].StartsWith("application/json"))
            {
                throw new NotImplementedException("LOGIN JSON 1");
            }
            ParseLoginInfo(resp);

            WebClient.Headers.Set("Accept", "application/json");
            WebClient.Headers.Set("Content-Type", "application/json");
            resp = await WebClient.UploadStringTaskAsync("service/res.do", "POST", "{\"tag\":\"search@teachingTerm\",\"branch\":\"byId\",\"params\":{\"termId\":" + AttachInfomation["term"] + "}}");
            WriteConfFile("jlu.teachingterm.json", AutoLogin ? resp : "");
            // retry once
            if (!WebClient.ResponseHeaders["Content-Type"].StartsWith("application/json"))
            {
                throw new NotImplementedException("LOGIN JSON 2");
            }
            ParseTermInfo(resp);

            IsLogin = true;
            return true;
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
                throw new NotImplementedException("POST JSON 1");
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
