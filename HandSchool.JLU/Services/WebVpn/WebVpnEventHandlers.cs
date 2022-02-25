﻿using System;
using System.Linq;
using System.Threading.Tasks;
using HandSchool.Controls;
using HandSchool.Internals;
using HandSchool.Models;
using Xamarin.Forms;

namespace HandSchool.JLU.Services
{
    public partial class WebVpn
    {
        private void OnReceivingJsData(string data)
        {
            var info = data.Split(' ');
            if (info.Length != 3) return;
            var (uid, pwd, ePwd) = (info[0], info[1], info[2]);
            if (uid.IsBlank() || pwd.IsBlank()) return;
            Username = uid;
            Password = pwd;
            _encryptedPassword = ePwd;
            Core.App.Loader.AccountManager.InsertOrUpdateTable(new UserAccount
            {
                ServerName = ServerName,
                UserName = Username,
                Password = $"{Password} {ePwd}"
            });
        }
        
        private void OnNavigating(object s, WebNavigatingEventArgs e)
        {
            if (!e.Url.Contains("webvpn.jlu.edu.cn"))
            {
                Events.WebViewEvents.WebView.Source = LoginUrl;
            }
        }

        private const string PasswordEncryptKey = "wrdvpnisawesome!";
        private async void OnNavigated(object s, WebNavigatedEventArgs e)
        {
            if (_pageClosed) return;
            if (e.Url == "https://webvpn.jlu.edu.cn/login")
            {
                await Events.WebViewEvents.EvaluateJavaScriptAsync(
                    "document.getElementsByName('remember_cookie')[0].checked=true;\n" +
                    "document.getElementsByClassName('remember-field')[0].hidden=true;\n" + 
                    "$('#login').click(function(){\n" +
                    "let uid = document.getElementById('user_name').value;\n" +
                    "let pwd = document.getElementsByName('password')[0].value;\n" +
                    $"let encryptedPwd = encrypt(pwd, '{PasswordEncryptKey}', '{PasswordEncryptKey}');\n" +
                    $"{HSWebView.NativeMethodInvoker}(uid + ' ' + pwd + ' ' + encryptedPwd);\n" +
                    "})");
                if (!string.IsNullOrWhiteSpace(Username))
                {
                    await Events.WebViewEvents.EvaluateJavaScriptAsync(
                        $"document.getElementById('user_name').value='{Username}'");
                    await Events.WebViewEvents.EvaluateJavaScriptAsync(
                        $"document.getElementsByName('password')[0].value='{Password}'");
                }
            }

            if (e.Url == "https://webvpn.jlu.edu.cn/" || e.Url == "https://webvpn.jlu.edu.cn/m/portal")
            {
                var cookie = Events.WebViewEvents.WebView.HSCookies.GetCookies(new Uri("https://webvpn.jlu.edu.cn/"));
                var updated = SaveCookies(cookie.ToEnumerable());
                if (updated)
                {
                    Core.App.Loader.JsonManager.InsertOrUpdateTable(new ServerJson
                    {
                        JsonName = ConfigCookies,
                        Json = GetLoginCookies()
                            .Select(c => new CookieLite(c))
                            .Serialize()
                    });
                }

                await CheckIsLogin();
                if (!IsLogin) return;
                if (_pageClosed) return;
                _pageClosed = true;
                await (Events?.Page?.CloseAsync() ?? Task.CompletedTask);
            }
        }
    }
}