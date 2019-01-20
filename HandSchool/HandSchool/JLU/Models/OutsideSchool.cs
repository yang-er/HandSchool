using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
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
        public class OutsideSchoolStrategy : ISideSchoolStrategy
        {
            public static string token;
            UIMS UIMS { get; }

            const string BaseUrl = "http://jwcjc.jlu.edu.cn/";
            const string SecurityCheck = "textbookSystem/api/login/login";

            public OutsideSchoolStrategy(UIMS handle)
            {
                UIMS = handle;
            }

            public string TimeoutUrl => "???";
            public string WelcomeMessage => UIMS.NeedLogin ? "请登录" : $"欢迎，{UIMS.AttachInfomation["studName"]}。";
            public string CurrentMessage => "不在学校的第n天，想念暖气";
            public string FormatArguments(string input) => input;

            public async Task<bool> LoginSide()
            {
                if (UIMS.WebClient != null) UIMS.WebClient.Dispose();
                UIMS.WebClient = new AwaredWebClient(BaseUrl, Encoding.UTF8);
                UIMS.WebClient.Headers["authorization"] = "65644545454545454";

                try
                {
                    UIMS.WebClient.Headers["Referer"] = BaseUrl + "textbookWap/";
                    var loginData = $"{{\"USERNAME\":\"{UIMS.Username}\",\"PASSWORD\":\"{UIMS.Password}\"}}";
                    var ret = await UIMS.WebClient.PostAsync(SecurityCheck, loginData);

                    var obj = ret.ParseJSON<TMWX>();
                    token = obj.token;
                    UIMS.WebClient.Headers["authorization"] = token;
                    if (obj.status == "error")
                    {
                        UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(LoginState.Failed, obj.message ?? "未知错误"));
                        return false;
                    }

                    UIMS.AttachInfomation.Add("studId", obj.username);
                    UIMS.AttachInfomation.Add("studName", obj.NAME);
                    UIMS.AttachInfomation.Add("adcId", obj.CLASSES);
                    UIMS.AttachInfomation.Add("schoolId", "");
                    UIMS.AttachInfomation.Add("term", "");
                }
                catch (WebException ex)
                {
                    UIMS.LoginStateChanged?.Invoke(UIMS, new LoginStateEventArgs(ex));
                    return false;
                }
                catch (ContentAcceptException ex)
                {
                    var error = "通脉微笑服务器似乎出了点问题……";
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

            public void OnLoad()
            {
                UIMS.Tips = "密码与UIMS无关，用户名为教学号，默认密码为身份证后六位。";
                UIMS.FormName = "通脉微笑平台";
                UIMS.NeedLogin = true;
                Loader.Loader2.GradePoint = new Lazy<Services.IGradeEntrance>(() => new TMXWGrade());
                Loader.Loader2.GradePoint.Value.ToString();
                Loader.Loader2.Message = new Lazy<Services.IMessageEntrance>(() => new NullMsg());
                Loader.InfoList.RemoveAll(t => !t.Name.Contains("图书"));
            }

            class NullMsg : Services.IMessageEntrance
            {
                public string ScriptFileUri => "";
                public bool IsPost => false;
                public string PostValue => "";
                public string StorageFile => "";
                public string LastReport => "";
                public Task Delete(int id) => Task.CompletedTask;
                public async Task Execute()
                {
                    await HandSchool.ViewModels.MessageViewModel.Instance.ShowMessage("错误", "您在校外，暂时不能查看收件箱。");
                }
                public void Parse() { }
                public Task SetReadState(int id, bool read) => Task.CompletedTask;
            }
        }
    }
}
