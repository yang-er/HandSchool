using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HandSchool.JLU.Services;
using HandSchool.Services;

namespace HandSchool.JLU
{
    partial class UIMS
    {
        class DefaultSchoolStrategy : ISideSchoolStrategy
        {
            UIMS UIMS { get; }

            public DefaultSchoolStrategy(UIMS handle)
            {
                UIMS = handle;
                WebVpn.Instance?.RegisterUrl(_baseUrl, "https://webvpn.jlu.edu.cn/https/77726476706e69737468656265737421e5fe4c8f693a6445300d8db9d6562d/ntms/");
            }

            public string TimeoutUrl => "error/dispatch.jsp?reason=nologin";

            const string getTermInfo = "{\"tag\":\"search@teachingTerm\",\"branch\":\"byId\",\"params\":{\"termId\":`term`}}";

            #region LoginInfo

            public LoginValue LoginInfo { get; set; }

            private string studId, studName, adcId, schoolId, term, Nick;

            private void ParseLoginInfo(string resp)
            {
                LoginInfo = resp.ParseJSON<LoginValue>();
                studId = LoginInfo.userId.ToString();
                studName = LoginInfo.loginInfo?.nickName ?? LoginInfo.nickName ?? "同学";
                adcId = LoginInfo.defRes.adcId.ToString();
                schoolId = LoginInfo.defRes.school.ToString();
                term = LoginInfo.defRes.teachingTerm == 0 ? "139" : LoginInfo.defRes.teachingTerm.ToString();
            }

            private void ParseTermInfo(string resp)
            {
                var rot = resp.ParseJSON<RootObject<TeachingTerm>>();

                if (rot.value.Length == 0)
                {
                    Nick = "接口似乎出了点问题";
                    UIMS.CurrentWeek = 0;
                    return;
                }

                var ro = rot.value[0];
                int.TryParse(ro.weeks, out var ws);
                UIMS.TotalWeek = ws;
                
                if (ro.vacationDate < DateTime.Now)
                {
                    Nick = ro.year + "学年" + (ro.termSeq == "1" ? "寒假" : "暑假");
                    UIMS.CurrentWeek = (int)Math.Ceiling((decimal)((DateTime.Now - ro.vacationDate).Days + 1) / 7);
                    UIMS.SchoolState = SchoolState.Vacation;
                }
                else
                {
                    Nick = ro.year + "学年" + (ro.termSeq == "1" ? "秋季学期" : (ro.termSeq == "2" ? "春季学期" : "短学期"));
                    UIMS.CurrentWeek = (int)Math.Ceiling((decimal)((DateTime.Now - ro.startDate).Days + 1) / 7);
                    UIMS.SchoolState = SchoolState.Normal;
                }
            }

            #endregion

            bool reinit = true;
            private readonly string _baseUrl = "https://uims.jlu.edu.cn/ntms/";
            public async Task<TaskResp> PrepareLogin()
            {
                if (reinit)
                {
                    UIMS.WebClient?.Dispose();
                    UIMS.WebClient = Core.New<IWebClient>();

                    UIMS.WebClient.BaseAddress = _baseUrl;
                    for (int i = 0; i < 15; i++)
                    {
                        try
                        {
                            await UIMS.WebClient.GetAsync("", "*/*");
                            break;
                        }
                        catch (WebsException)
                        {
                            Core.Logger.WriteLine("UIMS", "Timeout #" + i);
                            // continue;
                        }
                    }

                    reinit = false;
                }

                if (!UIMS.IsLogin)
                {
                    try
                    {
                        var captcha = await UIMS.WebClient.GetAsync("open/get-captcha-image.do");
                        if (captcha.StatusCode != HttpStatusCode.OK) return TaskResp.False;
                        UIMS.CaptchaSource = await captcha.ReadAsByteArrayAsync();
                    }
                    catch (WebsException)
                    {
                        UIMS.CaptchaSource = null;
                        UIMS.CaptchaCode = null;
                    }
                }

                return TaskResp.True;
            }

            public async Task<TaskResp> LoginSide()
            {
                try
                {
                    // Set Login Session
                    var loginData = new KeyValueDict
                    {
                        { "username", UIMS.Username },
                        { "password", $"UIMS{UIMS.Username}{UIMS.Password}".ToMD5(Encoding.UTF8) },
                        { "mousePath", "" },
                        { "vcode", UIMS.CaptchaCode }
                    };

                    var reqMeta = new WebRequestMeta("j_spring_security_check", WebRequestMeta.All);
                    reqMeta.SetHeader("Referer", UIMS.ServerUri + "userLogin.jsp?reason=nologin");
                    var response = await UIMS.WebClient.PostAsync(reqMeta, loginData);
                    var loc = response.Location.Replace(UIMS.ServerUri, "");

                    switch (loc)
                    {
                        case "userLogin.jsp?reason=loginError":
                        {
                            string result = await UIMS.WebClient.GetStringAsync("userLogin.jsp?reason=loginError", "text/html");
                            var html = new HtmlAgilityPack.HtmlDocument();
                            html.LoadHtml(result);
                            var msg = html.DocumentNode.SelectSingleNode("//span[@class='error_message' and @id='error_message']")?.InnerText?.Replace("登录错误：","");
                            UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(LoginState.Failed, msg));
                            UIMS.IsLogin = false;
                            UIMS.NeedLogin = true;
                            return TaskResp.False;
                        }
                        case "index.do":
                        {
                            studId = studName = adcId = schoolId = term = null;
                            // Get User Info
                            reqMeta = new WebRequestMeta("action/getCurrentUserInfo.do", WebRequestMeta.Json);
                            // reqMeta.SetHeader("Referer", "https://uims.jlu.edu.cn/ntms/index.do");
                            var webResp2 = await UIMS.WebClient.PostAsync(reqMeta, "", WebRequestMeta.Form);
                            string resp = await webResp2.ReadAsStringAsync();
                            if (resp.StartsWith("<!")) return TaskResp.False;
                            Core.App.Loader.JsonManager.InsertOrUpdateTable(new ServerJson
                            {
                                JsonName = UserCacheColName,
                                Json = UIMS.SavePassword ? resp : ""
                            });
                            ParseLoginInfo(resp);

                            // Get term info
                            reqMeta = new WebRequestMeta("service/res.do", WebRequestMeta.Json);
                            webResp2 = await UIMS.WebClient.PostAsync(reqMeta, FormatArguments(getTermInfo), WebRequestMeta.Json);
                            resp = await webResp2.ReadAsStringAsync();
                            if (resp.StartsWith("<!")) return TaskResp.False;
                            Core.App.Loader.JsonManager.InsertOrUpdateTable(
                                new ServerJson
                                {
                                    JsonName = TeachTermColName,
                                    Json = UIMS.SavePassword ? resp : ""
                                });
                            ParseTermInfo(resp);
                            break;
                        }
                        default:
                        {
                            var error = "UIMS服务器似乎出了点问题……\n";
                            if (response.StatusCode == HttpStatusCode.Redirect)
                                error += "服务器未知响应：" + response.Location + "，请联系开发者。";
                            throw new WebsException(error, WebStatus.UnknownError);
                        }
                    }
                }
                catch (WebsException ex)
                {
                    UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(ex));
                    reinit = true;
                    UIMS.NeedLogin = true;
                    return TaskResp.False;
                }

                UIMS.IsLogin = true;
                UIMS.NeedLogin = false;
                UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(LoginState.Succeeded));                
                return TaskResp.True;
            }

            public async Task LogoutSide()
            {
                try
                {
                    await UIMS.WebClient.GetAsync("logout.do?reason=M"); 
                    UIMS.IsLogin = false;
                }
                catch (WebsException ex)
                {
                    Core.Logger.WriteException(ex);
                }
            }

            [ToFix("存在性能问题，瓶颈在JSON的解析上")]
            public void OnLoad()
            {
                try
                {
                    var loginInfo = Core.App.Loader.JsonManager.GetItemWithPrimaryKey(UserCacheColName)?.Json;
                    var termInfo = Core.App.Loader.JsonManager.GetItemWithPrimaryKey(TeachTermColName)?.Json;
                    if (string.IsNullOrWhiteSpace(loginInfo) || string.IsNullOrWhiteSpace(termInfo))
                    {
                        UIMS.AutoLogin = false;
                        UIMS.NeedLogin = true;
                        return;
                    }

                    ParseLoginInfo(loginInfo);
                    ParseTermInfo(termInfo);
                    UIMS.NeedLogin = !UIMS.SavePassword;
                }
                catch (JsonException)
                {
                    UIMS.AutoLogin = false;
                    UIMS.NeedLogin = true;
                }
            }

            public string FormatArguments(string args)
            {
                return args
                    ?.Replace("`term`", term)
                    ?.Replace("`studId`", studId)
                    ?.Replace("`adcId`", adcId);
            }

            public string WelcomeMessage => UIMS.NeedLogin ? "请登录" : $"欢迎，{studName}。";
            public string CurrentMessage => UIMS.NeedLogin ? "登录后可以查看更多内容" : $"{Nick}第{UIMS.CurrentWeek}周";
        }
    }
}