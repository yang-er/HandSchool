using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandSchool.JLU
{
    partial class UIMS
    {
        class InsideSchoolStrategy : ISideSchoolStrategy
        {
            UIMS UIMS { get; }

            public InsideSchoolStrategy(UIMS handle)
            {
                UIMS = handle;
            }

            public string TimeoutUrl => "error/dispatch.jsp?reason=nologin";

            const string getTermInfo = "{\"tag\":\"search@teachingTerm\",\"branch\":\"byId\",\"params\":{\"termId\":`term`}}";
            const string MousePath = "NCgABNAQBgNAwBjNBQBkNBgBqNBwBtNBwB1NDAB6OEACCPFQCHRHACK" +
                "TIQCUTJQCXWLwCbXNACeYOgClaOgCmcPwCpcQQCqcQwCxcQwC0cRQC2cRgC4cRwC7dRwDPdSAGMd" +
                "SQGNdTAGQdTAGRdTgGUdTwGZdUAGfdVQGidWQGkdWgGpdYgGvdYwGwdZwGzdZwG0daAG0daQG3da" +
                "wG4dbAG6dbwG7dbwG8dcQG8dcgG9ddAHAddQHBddgHCdeAHDdeAHKdfgHLfgQHNfgwHOfhAHPghg" +
                "HRghwHRghwHTgigHUgjAHYgjQHYgjwHZgjwHagkAHagkwHcgkwHdhlgHfhlwHihmAHihmgHihnQH" +
                "lhngHnjoAHpjogHyjqQHzjqwH0jrAH0jrgH3lrwH5lsgH6ltAH7ltwH8ltwH+luQIBluwICluwID" +
                "lvQIIlvwIKlwAILlwgINlxAIPlxAIQlxgISlxwIXlyAIlkyQJ6kygJ+kzQKJkzQKMkzwKPk0QKVj" +
                "0QKaj1gKdj2gKoj2wKrj4QKuj5wKzIqgFL";

            #region LoginInfo

            public LoginValue LoginInfo { get; set; }

            private string studId, studName, adcId, schoolId, term, Nick;

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
                    UIMS.CurrentWeek = (int)Math.Ceiling((decimal)((DateTime.Now - ro.vacationDate).Days + 1) / 7);
                }
                else
                {
                    Nick = ro.year + "学年" + (ro.termSeq == "1" ? "秋季学期" : (ro.termSeq == "2" ? "春季学期" : "短学期"));
                    UIMS.CurrentWeek = (int)Math.Ceiling((decimal)((DateTime.Now - ro.startDate).Days + 1) / 7);
                }
            }

            #endregion

            public async Task<bool> LoginSide()
            {
                if (UIMS.WebClient != null) UIMS.WebClient.Dispose();
                UIMS.WebClient = Core.New<IWebClient>();
                UIMS.WebClient.BaseAddress = UIMS.ServerUri;
                var proxy_server_domain = UIMS.proxy_server.Split(':')[0];
                UIMS.WebClient.AddCookie(new Cookie("loginPage", "userLogin.jsp", "/ntms/", proxy_server_domain));
                UIMS.WebClient.AddCookie(new Cookie("alu", UIMS.Username, "/ntms/", proxy_server_domain));
                UIMS.WebClient.AddCookie(new Cookie("pwdStrength", "1", "/ntms/", proxy_server_domain));

                // Access Main Page To Create a JSESSIONID
                try
                {
                    var activateRequest = await UIMS.WebClient.GetAsync("", "*/*");

                    // Set Login Session
                    var loginData = new KeyValueDict
                    {
                        { "j_username", UIMS.Username },
                        { "j_password", $"UIMS{UIMS.Username}{UIMS.Password}".ToMD5(Encoding.UTF8) },
                        { "mousePath", MousePath }
                    };

                    var reqMeta = new WebRequestMeta("j_spring_security_check", WebRequestMeta.All);
                    reqMeta.SetHeader("Referer", UIMS.ServerUri + "userLogin.jsp?reason=nologin");
                    var response = await UIMS.WebClient.PostAsync(reqMeta, loginData);

                    if (response.Location == "error/dispatch.jsp?reason=loginError")
                    {
                        string result = await UIMS.WebClient.GetStringAsync("userLogin.jsp?reason=loginError", "text/html");
                        UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(LoginState.Failed, Regex.Match(result, @"<span class=""error_message"" id=""error_message"">登录错误：(\S+)</span>").Groups[1].Value));
                        UIMS.IsLogin = false;
                        return false;
                    }
                    else if (response.Location == "index.do")
                    {
                        studId = studName = adcId = schoolId = term = null;

                        // Get User Info
                        reqMeta = new WebRequestMeta("action/getCurrentUserInfo.do", WebRequestMeta.Json);
                        var webResp2 = await UIMS.WebClient.PostAsync(reqMeta, "{}", WebRequestMeta.Json);
                        string resp = await webResp2.ReadAsStringAsync();
                        if (resp.StartsWith("<!")) return false;
                        Core.Configure.Write(configUserCache, UIMS.AutoLogin ? resp : "");
                        ParseLoginInfo(resp);

                        // Get term info
                        reqMeta = new WebRequestMeta("service/res.do", WebRequestMeta.Json);
                        webResp2 = await UIMS.WebClient.PostAsync(reqMeta, FormatArguments(getTermInfo), WebRequestMeta.Json);
                        resp = await webResp2.ReadAsStringAsync();
                        if (resp.StartsWith("<!")) return false;
                        Core.Configure.Write(configTeachTerm, UIMS.AutoLogin ? resp : "");
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
                    UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(ex));
                    return false;
                }

                UIMS.IsLogin = true;
                UIMS.NeedLogin = false;
                UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(LoginState.Succeeded));                
                return true;
            }

            [ToFix("存在性能问题，瓶颈在JSON的解析上")]
            public void OnLoad()
            {
                try
                {
                    ParseLoginInfo(Core.Configure.Read(configUserCache));
                    ParseTermInfo(Core.Configure.Read(configTeachTerm));
                }
                catch (JsonException)
                {
                    UIMS.AutoLogin = false;
                    UIMS.NeedLogin = true;
                }
                catch (NullReferenceException)
                {
                    Core.Configure.Write(configUserCache, "");
                    Core.Configure.Write(configTeachTerm, "");
                    UIMS.AutoLogin = false;
                    UIMS.NeedLogin = true;
                }
            }

            public string FormatArguments(string args)
            {
                return args
                    .Replace("`term`", term)
                    .Replace("`studId`", studId)
                    .Replace("`adcId`", adcId);
            }

            public string WelcomeMessage => UIMS.NeedLogin ? "请登录" : $"欢迎，{studName}。";
            public string CurrentMessage => UIMS.NeedLogin ? "登录后可以查看更多内容" : $"{Nick}第{UIMS.CurrentWeek}周";
        }
    }
}