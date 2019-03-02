using HandSchool.Internals;
using HandSchool.JLU.Services;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandSchool.JLU
{
    partial class UIMS
    {
        class OutsideSchoolStrategy : ISideSchoolStrategy
        {
            UIMS UIMS { get; }
            const string BaseUrl = "http://cjcx.jlu.edu.cn/score/action/";
            const string ServiceRes = "service_res.php";
            const string SecurityCheck = "security_check.php";

            public OutsideSchoolStrategy(UIMS handle) => UIMS = handle;

            public string TimeoutUrl => "???";
            public string WelcomeMessage => "欢迎你哦。";
            public string CurrentMessage => "不在学校的第n天，想念暖气";
            public string FormatArguments(string input) => input;

            public async Task<bool> LoginSide()
            {
                if (UIMS.WebClient != null) UIMS.WebClient.Dispose();
                UIMS.WebClient = Core.New<IWebClient>();
                UIMS.WebClient.BaseAddress = BaseUrl;

                var proxy_server_domain = UIMS.proxy_server.Split(':')[0];
                UIMS.WebClient.AddCookie(new Cookie("loginPage", "userLogin.jsp", "/score/action/", "cjcx.jlu.edu.cn"));
                UIMS.WebClient.AddCookie(new Cookie("alu", UIMS.Username, "/score/action/", "cjcx.jlu.edu.cn"));

                try
                {
                    var loginData = new KeyValueDict
                    {
                        { "j_username", UIMS.Username },
                        { "j_password", $"UIMS{UIMS.Username}{UIMS.Password}".ToMD5(Encoding.UTF8) },
                    };

                    var loginMeta = new WebRequestMeta(SecurityCheck, "*/*");

                    var loginResult = await UIMS.WebClient.PostAsync(loginMeta, loginData);

                    if (loginResult.Location == "../index.php")
                    {
                        UIMS.IsLogin = true;
                        UIMS.NeedLogin = false;
                        UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(LoginState.Succeeded));
                        return true;
                    }
                    else if (loginResult.Location == "../userLogin.php?reason=loginError")
                    {
                        string result = await UIMS.WebClient.GetStringAsync("../userLogin.php?reason=loginError", "text/html");
                        UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(LoginState.Failed, Regex.Match(result, @"<span class=""error_message"" id=""error_message"">登录错误(\S+)</span>").Groups[1].Value));
                        UIMS.IsLogin = false;
                        return false;
                    }
                    else
                    {
                        UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(LoginState.Failed, loginResult.Location));
                        return false;
                    }
                }
                catch (WebsException ex)
                {
                    UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(ex));
                    return false;
                }
            }

            public void OnLoad()
            {
                UIMS.NeedLogin = true;
                var Loader = Core.App.Loader as Loader;
                Loader.GradePoint = new Lazy<IGradeEntrance>(() => new CJCXGrade());
                Loader.GradePoint.Value.ToString();
                Loader.Message = new Lazy<IMessageEntrance>(() => new NullMsg());
                Loader.InfoList.RemoveAll(t => !t.Title.Contains("图书"));
            }

            class NullMsg : IMessageEntrance
            {
                public string ScriptFileUri => "";
                public bool IsPost => false;
                public string PostValue => "";
                public string StorageFile => "";
                public string LastReport => "";
                public Task Delete(int id) => Task.CompletedTask;
                public async Task Execute()
                {
                    await HandSchool.ViewModels.MessageViewModel.Instance.RequestMessageAsync("错误", "您在校外，暂时不能查看收件箱。");
                }
                public void Parse() { }
                public Task SetReadState(int id, bool read) => Task.CompletedTask;
            }
        }
    }
}
