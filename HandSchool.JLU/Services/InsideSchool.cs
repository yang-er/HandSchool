using HandSchool.Internal;
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
            public LoginValue LoginInfo { get; set; }

            const string MousePath = "NCgABNAQBgNAwBjNBQBkNBgBqNBwBtNBwB1NDAB6OEACCPFQCHRHACK" +
                "TIQCUTJQCXWLwCbXNACeYOgClaOgCmcPwCpcQQCqcQwCxcQwC0cRQC2cRgC4cRwC7dRwDPdSAGMd" +
                "SQGNdTAGQdTAGRdTgGUdTwGZdUAGfdVQGidWQGkdWgGpdYgGvdYwGwdZwGzdZwG0daAG0daQG3da" +
                "wG4dbAG6dbwG7dbwG8dcQG8dcgG9ddAHAddQHBddgHCdeAHDdeAHKdfgHLfgQHNfgwHOfhAHPghg" +
                "HRghwHRghwHTgigHUgjAHYgjQHYgjwHZgjwHagkAHagkwHcgkwHdhlgHfhlwHihmAHihmgHihnQH" +
                "lhngHnjoAHpjogHyjqQHzjqwH0jrAH0jrgH3lrwH5lsgH6ltAH7ltwH8ltwH+luQIBluwICluwID" +
                "lvQIIlvwIKlwAILlwgINlxAIPlxAIQlxgISlxwIXlyAIlkyQJ6kygJ+kzQKJkzQKMkzwKPk0QKVj" +
                "0QKaj1gKdj2gKoj2wKrj4QKuj5wKzIqgFL";

            private void ParseLoginInfo(string resp)
            {
                LoginInfo = resp.ParseJSON<LoginValue>();
                UIMS.AttachInfomation.Add("studId", LoginInfo.userId.ToString());
                UIMS.AttachInfomation.Add("studName", LoginInfo.nickName);
                UIMS.AttachInfomation.Add("adcId", LoginInfo.defRes.adcId.ToString());
                UIMS.AttachInfomation.Add("schoolId", LoginInfo.defRes.school.ToString());
                UIMS.AttachInfomation.Add("term", LoginInfo.defRes.teachingTerm.ToString());
            }

            private void ParseTermInfo(string resp)
            {
                var ro = resp.ParseJSON<RootObject<TeachingTerm>>().value[0];
                if (ro.vacationDate < DateTime.Now)
                {
                    UIMS.AttachInfomation.Add("Nick", ro.year + "学年" + (ro.termSeq == "1" ? "寒假" : "暑假"));
                    UIMS.CurrentWeek = (int)Math.Ceiling((decimal)((DateTime.Now - ro.vacationDate).Days + 1) / 7);
                }
                else
                {
                    UIMS.AttachInfomation.Add("Nick", ro.year + "学年" + (ro.termSeq == "1" ? "秋季学期" : (ro.termSeq == "2" ? "春季学期" : "短学期")));
                    UIMS.CurrentWeek = (int)Math.Ceiling((decimal)((DateTime.Now - ro.startDate).Days + 1) / 7);
                }
            }

            public async Task<bool> LoginSide()
            {
                if (UIMS.WebClient != null) UIMS.WebClient.Dispose();
                UIMS.WebClient = new AwaredWebClient(UIMS.ServerUri, Encoding.UTF8);
                var proxy_server_domain = UIMS.proxy_server.Split(':')[0];
                UIMS.WebClient.Cookie.Add(new Cookie("loginPage", "userLogin.jsp", "/ntms/", proxy_server_domain));
                UIMS.WebClient.Cookie.Add(new Cookie("alu", UIMS.Username, "/ntms/", proxy_server_domain));
                UIMS.WebClient.Cookie.Add(new Cookie("pwdStrength", "1", "/ntms/", proxy_server_domain));

                // Access Main Page To Create a JSESSIONID
                try
                {
                    await UIMS.WebClient.GetAsync("", "*/*");

                    // Set Login Session
                    var loginData = new NameValueCollection
                    {
                        { "j_username", UIMS.Username },
                        { "j_password", $"UIMS{UIMS.Username}{UIMS.Password}".ToMD5(Encoding.UTF8) },
                        { "mousePath", MousePath }
                    };

                    UIMS.WebClient.Headers.Set("Referer", UIMS.ServerUri + "userLogin.jsp?reason=nologin");
                    await UIMS.WebClient.PostAsync("j_spring_security_check", loginData);

                    if (UIMS.WebClient.Location == "error/dispatch.jsp?reason=loginError")
                    {
                        string result = await UIMS.WebClient.GetAsync("userLogin.jsp?reason=loginError", "text/html");
                        UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(LoginState.Failed, Regex.Match(result, @"<span class=""error_message"" id=""error_message"">登录错误：(\S+)</span>").Groups[1].Value));
                        UIMS.IsLogin = false;
                        return false;
                    }
                    else if (UIMS.WebClient.Location == "index.do")
                    {
                        UIMS.AttachInfomation.Clear();

                        // Get User Info
                        string resp = await UIMS.WebClient.PostAsync("action/getCurrentUserInfo.do", "{}");
                        if (resp.StartsWith("<!")) return false;
                        Core.Configure.Write(configUserCache, UIMS.AutoLogin ? resp : "");
                        ParseLoginInfo(resp);

                        // Get term info
                        resp = await UIMS.WebClient.PostAsync("service/res.do", "{\"tag\":\"search@teachingTerm\",\"branch\":\"byId\",\"params\":{\"termId\":" + UIMS.AttachInfomation["term"] + "}}");
                        if (resp.StartsWith("<!")) return false;
                        Core.Configure.Write(configTeachTerm, UIMS.AutoLogin ? resp : "");
                        ParseTermInfo(resp);
                    }
                    else
                    {
                        throw new ContentAcceptException(UIMS.WebClient.Location, null, null);
                    }
                }
                catch (WebException ex)
                {
                    UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(ex));
                    return false;
                }
                catch (ContentAcceptException ex)
                {
                    var error = "UIMS服务器似乎出了点问题……";
                    if (ex.Current != "")
                        error += "服务器未知响应：" + ex.Data + "，请联系开发者。";
                    UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(LoginState.Failed, error));
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
                    .Replace("`term`", UIMS.AttachInfomation["term"])
                    .Replace("`studId`", UIMS.AttachInfomation["studId"])
                    .Replace("`adcId`", UIMS.AttachInfomation["adcId"]);
            }

            public string WelcomeMessage => UIMS.NeedLogin ? "请登录" : $"欢迎，{UIMS.AttachInfomation["studName"]}。";
            public string CurrentMessage => UIMS.NeedLogin ? "登录后可以查看更多内容" : $"{UIMS.AttachInfomation["Nick"]}第{UIMS.CurrentWeek}周";
        }
    }
}
