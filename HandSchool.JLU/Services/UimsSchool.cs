using HandSchool.Design;
using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HandSchool.JLU.Services;

[assembly: RegisterService(typeof(UimsSchool))]
namespace HandSchool.JLU.Services
{
    [UseStorage("JLU", configTeachTerm, configUserCache)]
    internal sealed class UimsSchool : SchoolBase
    {
        const string getTermInfo = "{\"tag\":\"search@teachingTerm\",\"branch\":\"byId\",\"params\":{\"termId\":`term`}}";
        const string MousePath = "NCgABNAQBgNAwBjNBQBkNBgBqNBwBtNBwB1NDAB6OEACCPFQCHRHACK" +
            "TIQCUTJQCXWLwCbXNACeYOgClaOgCmcPwCpcQQCqcQwCxcQwC0cRQC2cRgC4cRwC7dRwDPdSAGMd" +
            "SQGNdTAGQdTAGRdTgGUdTwGZdUAGfdVQGidWQGkdWgGpdYgGvdYwGwdZwGzdZwG0daAG0daQG3da" +
            "wG4dbAG6dbwG7dbwG8dcQG8dcgG9ddAHAddQHBddgHCdeAHDdeAHKdfgHLfgQHNfgwHOfhAHPghg" +
            "HRghwHRghwHTgigHUgjAHYgjQHYgjwHZgjwHagkAHagkwHcgkwHdhlgHfhlwHihmAHihmgHihnQH" +
            "lhngHnjoAHpjogHyjqQHzjqwH0jrAH0jrgH3lrwH5lsgH6ltAH7ltwH8ltwH+luQIBluwICluwID" +
            "lvQIIlvwIKlwAILlwgINlxAIPlxAIQlxgISlxwIXlyAIlkyQJ6kygJ+kzQKJkzQKMkzwKPk0QKVj" +
            "0QKaj1gKdj2gKoj2wKrj4QKuj5wKzIqgFL";
        const string configUserCache = "jlu.user.json";
        const string configTeachTerm = "jlu.teachingterm.json";

        public LoginValue LoginInfo { get; set; }

        private string studId, studName, adcId, schoolId, term, Nick;

        public override string TimeoutUrl => "error/dispatch.jsp?reason=nologin";
        public override string WelcomeMessage => NeedLogin ? "请登录" : $"欢迎，{studName}。";
        public override string CurrentMessage => NeedLogin ? "登录后可以查看更多内容" : $"{Nick}第{CurrentWeek}周";

        [ToFix("存在性能问题，瓶颈在JSON的解析上")]
        public UimsSchool(IConfigureProvider config, IWebClient wc) : base(config, wc)
        {
            WebClient.BaseAddress = ServerUri;
            
            try
            {
                ParseLoginInfo(Core.Configure.Read(configUserCache));
                ParseTermInfo(Core.Configure.Read(configTeachTerm));
            }
            catch (JsonException)
            {
                AutoLogin = false;
                NeedLogin = true;
            }
            catch (NullReferenceException)
            {
                Configure.Save(configUserCache, "");
                Configure.Save(configTeachTerm, "");
                AutoLogin = false;
                NeedLogin = true;
            }
        }

        private void ParseLoginInfo(string resp)
        {
            LoginInfo = resp.ParseJSON<LoginValue>();
            studId = LoginInfo.userId.ToString();
            studName = LoginInfo.nickName;
            adcId = LoginInfo.defRes.adcId.ToString();
            schoolId = LoginInfo.defRes.school.ToString();
            term = LoginInfo.defRes.teachingTerm.ToString();
        }

        private void ParseTermInfo(string resp)
        {
            var ro = resp.ParseJSON<RootObject<TeachingTerm>>().value[0];
            if (ro.vacationDate < DateTime.Now)
            {
                Nick = ro.year + "学年" + (ro.termSeq == "1" ? "寒假" : "暑假");
                CurrentWeek = (int)Math.Ceiling((decimal)((DateTime.Now - ro.vacationDate).Days + 1) / 7);
            }
            else
            {
                Nick = ro.year + "学年" + (ro.termSeq == "1" ? "秋季学期" : (ro.termSeq == "2" ? "春季学期" : "短学期"));
                CurrentWeek = (int)Math.Ceiling((decimal)((DateTime.Now - ro.startDate).Days + 1) / 7);
            }
        }
        
        public override async Task<bool> LoginSide()
        {
            WebClient.ResetClient();

            var proxy_server_domain = ProxyServer.Split(':')[0];
            WebClient.AddCookie(new Cookie("loginPage", "userLogin.jsp", "/ntms/", proxy_server_domain));
            WebClient.AddCookie(new Cookie("alu", Username, "/ntms/", proxy_server_domain));
            WebClient.AddCookie(new Cookie("pwdStrength", "1", "/ntms/", proxy_server_domain));

            // Access Main Page To Create a JSESSIONID
            try
            {
                var activateRequest = await WebClient.GetAsync("", "*/*");

                // Set Login Session
                var loginData = new KeyValueDict
                    {
                        { "j_username", Username },
                        { "j_password", $"UIMS{Username}{Password}".ToMD5(Encoding.UTF8) },
                        { "mousePath", MousePath }
                    };

                var reqMeta = new WebRequestMeta("j_spring_security_check", WebRequestMeta.All);
                reqMeta.SetHeader("Referer", ServerUri + "userLogin.jsp?reason=nologin");
                var response = await WebClient.PostAsync(reqMeta, loginData);

                if (response.Location == "error/dispatch.jsp?reason=loginError")
                {
                    string result = await WebClient.GetStringAsync("userLogin.jsp?reason=loginError", "text/html");
                    SendLoginStateChanged(new LoginStateEventArgs(LoginState.Failed, Regex.Match(result, @"<span class=""error_message"" id=""error_message"">登录错误：(\S+)</span>").Groups[1].Value));
                    IsLogin = false;
                    return false;
                }
                else if (response.Location == "index.do")
                {
                    studId = studName = adcId = schoolId = term = null;

                    // Get User Info
                    reqMeta = new WebRequestMeta("action/getCurrentUserInfo.do", WebRequestMeta.Json);
                    var webResp2 = await WebClient.PostAsync(reqMeta, "{}", WebRequestMeta.Json);
                    string resp = await webResp2.ReadAsStringAsync();
                    if (resp.StartsWith("<!")) return false;
                    await Configure.SaveAsync(configUserCache, AutoLogin ? resp : "");
                    ParseLoginInfo(resp);

                    // Get term info
                    reqMeta = new WebRequestMeta("service/res.do", WebRequestMeta.Json);
                    webResp2 = await WebClient.PostAsync(reqMeta, FormatArguments(getTermInfo), WebRequestMeta.Json);
                    resp = await webResp2.ReadAsStringAsync();
                    if (resp.StartsWith("<!")) return false;
                    await Configure.SaveAsync(configTeachTerm, AutoLogin ? resp : "");
                    ParseTermInfo(resp);
                }
                else
                {
                    var error = "UIMS服务器似乎出了点问题……\n";
                    if (response.StatusCode == HttpStatusCode.Redirect)
                        error += "服务器未知响应：" + response.Location + "，请联系开发者。";
                    throw new WebsException(error, WebStatus.UnknownError);
                }
            }
            catch (WebsException ex)
            {
                SendLoginStateChanged(new LoginStateEventArgs(ex));
                return false;
            }

            IsLogin = true;
            NeedLogin = false;
            SendLoginStateChanged(new LoginStateEventArgs(LoginState.Succeeded));
            return true;
        }
        
        public override string FormatArguments(string args)
        {
            return args
                .Replace("`term`", term)
                .Replace("`studId`", studId)
                .Replace("`adcId`", adcId);
        }
    }
}
