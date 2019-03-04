using HandSchool.Design;
using HandSchool.Internals;
using HandSchool.Models;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandSchool.JLU.Services
{
    internal sealed class CjcxSchool : SchoolBase
    {
        const string BaseUrl = "http://cjcx.jlu.edu.cn/score/action/";
        const string ServiceRes = "service_res.php";
        const string SecurityCheck = "security_check.php";
        
        public override string TimeoutUrl => "???";
        public override string WelcomeMessage => "欢迎你哦。";
        public override string CurrentMessage => "不在学校的第n天，想念暖气";
        public override string FormatArguments(string input) => input;

        public CjcxSchool(IConfigureProvider config, IWebClient wc) : base(config, wc)
        {
            WebClient.BaseAddress = BaseUrl;
            NeedLogin = true;
        }

        public override async Task<bool> LoginSide()
        {
            WebClient.ResetClient();
            
            WebClient.AddCookie(new Cookie("loginPage", "userLogin.jsp", "/score/action/", "cjcx.jlu.edu.cn"));
            WebClient.AddCookie(new Cookie("alu", Username, "/score/action/", "cjcx.jlu.edu.cn"));

            try
            {
                var loginData = new KeyValueDict
                {
                    { "j_username", Username },
                    { "j_password", $"UIMS{Username}{Password}".ToMD5(Encoding.UTF8) },
                };

                var loginMeta = new WebRequestMeta(SecurityCheck, "*/*");

                var loginResult = await WebClient.PostAsync(loginMeta, loginData);

                if (loginResult.Location == "../index.php")
                {
                    IsLogin = true;
                    NeedLogin = false;
                    SendLoginStateChanged(new LoginStateEventArgs(LoginState.Succeeded));
                    return true;
                }
                else if (loginResult.Location == "../userLogin.php?reason=loginError")
                {
                    string result = await WebClient.GetStringAsync("../userLogin.php?reason=loginError", "text/html");
                    SendLoginStateChanged(new LoginStateEventArgs(LoginState.Failed, Regex.Match(result, @"<span class=""error_message"" id=""error_message"">登录错误(\S+)</span>").Groups[1].Value));
                    IsLogin = false;
                    return false;
                }
                else
                {
                    SendLoginStateChanged(new LoginStateEventArgs(LoginState.Failed, loginResult.Location));
                    return false;
                }
            }
            catch (WebsException ex)
            {
                SendLoginStateChanged(new LoginStateEventArgs(ex));
                return false;
            }
        }
    }
}
