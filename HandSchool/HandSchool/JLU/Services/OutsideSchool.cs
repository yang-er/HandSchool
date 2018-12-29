using HandSchool.Internal;
using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HandSchool.JLU
{
    partial class UIMS
    {
        class OutsideSchoolStrategy : ISideSchoolStrategy
        {
            UIMS UIMS { get; }

            const string BaseUrl = "http://cjcx.jlu.edu.cn/score/";
            const string ServiceRes = "action/service_res.php";
            const string SecurityCheck = "action/security_check.php";

            public OutsideSchoolStrategy(UIMS handle) => UIMS = handle;

            public string TimeoutUrl => "???";
            public string WelcomeMessage => "欢迎你哦。";
            public string CurrentMessage => "不在学校的第n天，想念暖气";
            public string FormatArguments(string input) => input;

            public async Task<bool> LoginSide()
            {
                if (UIMS.WebClient != null) UIMS.WebClient.Dispose();
                UIMS.WebClient = new AwaredWebClient(BaseUrl, Encoding.UTF8);
                
                try
                {
                    var loginData = new NameValueCollection
                    {
                        { "j_username", UIMS.Username },
                        { "j_password", $"UIMS{UIMS.Username}{UIMS.Password}".ToMD5(Encoding.UTF8) },
                    };
                    
                    await UIMS.WebClient.PostAsync(SecurityCheck, loginData);

                    if (UIMS.WebClient.Location != string.Empty)
                    {
                        return false;
                    }
                }
                catch (WebException ex)
                {
                    UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(ex));
                    return false;
                }
                catch (ContentAcceptException ex)
                {
                    var error = "CJCX服务器似乎出了点问题……";
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

            public void OnLoad() { }
        }
    }
}
