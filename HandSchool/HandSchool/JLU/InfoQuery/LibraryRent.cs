using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("图书借阅管理", "查一查想要的书在图书馆的位置吧~", EntranceType.UrlEntrance)]
    class LibraryRent : BaseController, IUrlEntrance
    {
        const string Domain = "mc.m.5read.com";
        const string LoginPath = "/irdUser/login/opac/";
        const string PostValueBegin = "backurl=/cmpt/opac/opacLink.jspx?backurl=" +
            "http%3A%2F%2F202.198.25.5%3A8080%2Fsms%2Fopac%2Fuser%2FlendStatus.action" +
            "%3Fxc%3D3&shcoolid=920&userType=0&username=`uname`&password=`pwd`";
        const string PostType = "application/x-www-form-urlencoded";
        internal const string conf_username = "jlu.lib.username.txt";
        internal const string conf_password = "jlu.lib.password.txt";
        const string ErrorMessagePattern = @"<font color=""red"">(\S+)</font>";

        public string HtmlUrl { get; set; }
        public byte[] OpenWithPost { get; }
        public List<string> Cookie { get; }
        
        public IUrlEntrance SubUrlRequested(string sub)
        {
            return new LibraryRent(sub);
        }

        public override async Task Receive(string data)
        {
            await Task.Run(() => Core.Log(data));
        }
        
        public LibraryRent(string suburl)
        {
            HtmlUrl = suburl;
        }


        public class LoginDispatcher : NotifyPropertyChanged, ILoginField
        {
            public AwaredWebClient WebClient { get; set; }
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
            public bool IsLogin { get; set; }
            public bool NeedLogin { get; private set; }
            public string RedirectUrl { get; set; } = "";

            private bool auto_login = true;
            public bool AutoLogin
            {
                get => auto_login;
                set
                {
                    SetProperty(ref auto_login, value);
                    if (value) SetProperty(ref save_password, true, nameof(SavePassword));
                }
            }

            private bool save_password = true;
            public bool SavePassword
            {
                get => save_password;
                set
                {
                    SetProperty(ref save_password, value);
                    if (!value) SetProperty(ref auto_login, true, nameof(AutoLogin));
                }
            }

            public string FormName => "数字图书馆";
            public string Tips => "数字图书馆账号为一卡通号码，默认登陆密码为1234。";
            public string CaptchaCode { get; set; } = "";
            public byte[] CaptchaSource => null;

            public event EventHandler<LoginStateEventArgs> LoginStateChanged;

            public LoginDispatcher()
            {
                Username = Core.ReadConfig(conf_username);
                Password = Core.ReadConfig(conf_password);
            }

            public async Task<bool> Login()
            {
                if (Username == "" || Password == "")
                {
                    NeedLogin = true;
                    return false;
                }
                else
                {
                    Core.WriteConfig(conf_username, Username);
                    Core.WriteConfig(conf_password, SavePassword ? Password : "");
                }

                if (WebClient != null) WebClient.Dispose();
                WebClient = new AwaredWebClient("http://" + Domain, Encoding.UTF8);
                WebClient.Cookie.Add(new Cookie("xc", "5", "/", Domain));
                WebClient.Cookie.Add(new Cookie("mgid", "274", "/", Domain));
                WebClient.Cookie.Add(new Cookie("maid", "920", "/", Domain));

                // Access Main Page To Create a JSESSIONID
                try
                {
                    var realPost = PostValueBegin.Replace("`uname`", Username)
                                                 .Replace("`pwd`", Password);
                    var result = await WebClient.PostAsync(LoginPath + "opacLogin.jspx", realPost, PostType, "*/*");

                    if (result != "" && WebClient.Location == "")
                    {
                        var errMsg = Regex.Match(result, ErrorMessagePattern).Groups[1].Value;
                        var eventArgs = new LoginStateEventArgs(LoginState.Failed, errMsg);
                        LoginStateChanged?.Invoke(this, eventArgs);
                        IsLogin = false;
                        return false;
                    }
                    
                    await WebClient.GetAsync(WebClient.Location, "*/*");
                    RedirectUrl = WebClient.Location.Replace("http://202.198.25.5:8080", "https://lib.cdn.90yang.com");
                }
                catch (WebException ex)
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(ex));
                    return false;
                }
                catch (ContentAcceptException ex)
                {
                    var error = "图书馆的服务器似乎出了点问题……";
                    if (ex.Current != "")
                        error += "服务器未知响应：" + ex.Data + "，请联系开发者。";
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, error));
                    return false;
                }

                IsLogin = true;
                NeedLogin = false;
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Succeeded));
                return true;
            }

            public Task<bool> PrepareLogin()
            {
                return Task.FromResult(true);
            }
            
            public IUrlEntrance GetLibraryRent()
            {
                return new LibraryRent(RedirectUrl);
            }
        }
    }
}
