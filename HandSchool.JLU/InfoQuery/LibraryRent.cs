using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HandSchool.JLU.InfoQuery;

[assembly: RegisterService(typeof(LibraryRent))]
namespace HandSchool.JLU.InfoQuery
{
    /// <summary>
    /// 实现图书借阅管理功能。
    /// </summary>
    /// <inheritdoc cref="BaseController" />
    /// <inheritdoc cref="IUrlEntrance" />
    [UseStorage("JLU", configUsername, configPassword)]
    [Entrance("JLU", "图书借阅管理", "来看看现在正在借阅中的书吧~", EntranceType.UrlEntrance)]
    internal class LibraryRent : BaseController, IUrlEntrance
    {
        const string Domain = "mc.m.5read.com";
        const string LoginPath = "/irdUser/login/opac/";
        const string PostValueBegin = "backurl=/cmpt/opac/opacLink.jspx?backurl=" +
            "http%3A%2F%2F202.198.25.5%3A8080%2Fsms%2Fopac%2Fuser%2FlendStatus.action" +
            "%3Fxc%3D3&shcoolid=920&userType=0&username=`uname`&password=`pwd`";
        const string PostType = "application/x-www-form-urlencoded";
        const string configUsername = "jlu.lib.username.txt";
        const string configPassword = "jlu.lib.password.txt";
        const string ErrorMessagePattern = @"<font color=""red"">(\S+)</font>";

        public string HtmlUrl { get; set; }
        public byte[] OpenWithPost => null;
        public List<string> Cookie => null;
        
        public IUrlEntrance SubUrlRequested(string sub)
        {
            return new LibraryRent(sub);
        }

        public override Task Receive(string data)
        {
            Core.Log(data);
            return Task.CompletedTask;
        }
        
        public LibraryRent(string subUrl)
        {
            HtmlUrl = subUrl;
        }
        
        public static async Task RequestRentInfo(object o)
        {
            var rentInfo = new LoginDispatcher();
            if (await rentInfo.RequestLogin())
            {
                var ops = rentInfo.GetLibraryRent();
                if (o is Action<IWebEntrance> entReq)
                {
                    entReq.Invoke(ops);
                }
                else
                {
                    throw new ArgumentException(o.GetType().Name);
                }
            }
        }

        private class LoginDispatcher : NotifyPropertyChanged, ILoginField
        {
            private AwaredWebClient WebClient { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public bool IsLogin { get; private set; }
            public bool NeedLogin { get; private set; }
            private string RedirectUrl { get; set; } = "";

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
                Username = Core.Configure.Read(configUsername);
                Password = Core.Configure.Read(configPassword);
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
                    Core.Configure.Write(configUsername, Username);
                    Core.Configure.Write(configPassword, SavePassword ? Password : "");
                }

                WebClient?.Dispose();
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
                    RedirectUrl = WebClient.Location.Replace("http://202.198.25.5:8080", "https://lib.jlu.xylab.fun");
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
