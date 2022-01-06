using Android.Webkit;
using AndroidX.AppCompat.App;
using HandSchool.Internals;
using HandSchool.JLU.ViewModels;
using HtmlAgilityPack;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using HandSchool.JLU.Services;
using HandSchool.Pages;
using HandSchool.Views;
using AppActivity = AndroidX.AppCompat.App.AppCompatActivity;


namespace HandSchool.Droid.Internals
{
    class ObjectRes : Java.Lang.Object, IValueCallback
    {
        public object Value { get; set; }

        public void OnReceiveValue(Java.Lang.Object value)
        {
            Value = value;
        }
    }
    public class WebLoginPageImpl : WebLoginPage
    {
        public override Task CloseAsync()
        {
            var context  = PlatformImplV2.Instance.PeekContext(false);
            if (context is WebLoginActivity)
            {
                return Navigation.PopAsync();
            }
            return Task.CompletedTask;
        }

        public override Task ShowAsync()
        {
            var context  = PlatformImplV2.Instance.PeekContext();
            var navigate = context as INavigate;
            return navigate.PushAsync<WebLoginActivity>(this);
        }
    }
    public class AndroidWebDialogAdditionalArgs : WebDialogAdditionalArgs
    {
        public WebViewClient WebViewClient { get; set; }
        public WebChromeClient WebChromeClient { get; set; }
    }
    public class JSNoResult : Java.Lang.Object, IValueCallback
    {
        public void OnReceiveValue(Java.Lang.Object value) { return; }
    }
    public class CancelLostWebChromeClient : BaseWebChromeClient
    {
        public string Sources;
        public bool LoadState = false;
        public CancelLostWebChromeClient(object activity)
        {
            this.activity = activity;
        }
        public static void GetSources(WebView v)
        {
            (v.WebChromeClient as CancelLostWebChromeClient).LoadState = false;
            v.LoadUrl("javascript:alert(document.getElementsByTagName(\"html\")[0].outerHTML)");
            //网站编码问题，如果用Evaluate方法会乱码
        }
        public override bool OnJsAlert(WebView view, string url, string message, JsResult result)
        {
            if (message.StartsWith("<html")) Sources = message;//用来获取网页源代码的Alert
            result.Cancel();
            LoadState = true;
            return true;
        }
        public override bool OnJsConfirm(WebView view, string url, string message, JsResult result)
        {
            if (message.Contains("要解挂"))
                result.Confirm();
            else result.Cancel();
            return true;
        }
    }
    public class CancelLostWebClient : WebViewClient
    {
        public static string state = "正常";
        CancelLostWebChromeClient chromeClient;

        public CancelLostWebClient(CancelLostWebChromeClient chromeClient)
        {
            this.chromeClient = chromeClient;
        }
        public Thread AnalyzeHtmlThread(WebView view, Action uiThread)
        {
            return new Thread(new ThreadStart(() =>
            {
                var activity = (AppActivity)chromeClient.activity;

                //等待加载完成，分析网页源码
                while (!chromeClient.LoadState)
                    Thread.Sleep(10);
                var doc = new HtmlDocument();
                var str = chromeClient.Sources;
                doc.LoadHtml(str);
                var error = doc.DocumentNode.SelectNodes("//p[@class='biaotou']");
                if (error == null)//没有错误就填写密码
                {
                    activity.RunOnUiThread(uiThread);
                }
                else
                {
                    var sb = new StringBuilder();
                    foreach (var e in error)
                    {
                        sb.Append(e.InnerText).Append('\n');
                    }
                    var msg = sb.ToString();
                    if (msg.Contains("成功"))
                    {
                        activity.RunOnUiThread(() => { view.Clickable = true; });
                        return;
                    }
                    activity.RunOnUiThread(() =>
                    {
                        new AlertDialog.Builder((AppActivity)chromeClient.activity)
                        .SetTitle("错误")
                        .SetMessage(msg.Trim())
                        .SetPositiveButton("知道了", listener: null)
                        .SetCancelable(false).Show();
                        view.GoBack();
                    });
                }
            }));
        }
        public override void OnPageFinished(WebView view, string url)
        {
            var vpn = WebVpn.UseVpn;
            if (url.Contains(vpn ? "https://webvpn.jlu.edu.cn/http/77726476706e69737468656265737421e8ee4ad22d3c7d1e7b0c9ce29b5b/homeLogin.action" : "http://xyk.jlu.edu.cn/homeLogin.action"))//登录页面加载完成, 填密码
            {
                CancelLostWebChromeClient.GetSources(view);//获取页面源码
                AnalyzeHtmlThread(view, () =>
                {
                    view.EvaluateJavascript("document.FormPost.name.value = " + JLU.Loader.Ykt.Username, new JSNoResult());
                    view.EvaluateJavascript("document.FormPost.passwd.value = " + JLU.Loader.Ykt.Password, new JSNoResult());
                }).Start();
            }
            else if (url.Contains(vpn ? "https://webvpn.jlu.edu.cn/http/77726476706e69737468656265737421e8ee4ad22d3c7d1e7b0c9ce29b5b/loginstudent.action" : "http://xyk.jlu.edu.cn/loginstudent.action"))//登录成功
            {
                view.Clickable = false;
                CancelLostWebChromeClient.GetSources(view);//获取页面源码
                AnalyzeHtmlThread(view, () => { view.LoadUrl(vpn ? "https://webvpn.jlu.edu.cn/http/77726476706e69737468656265737421e8ee4ad22d3c7d1e7b0c9ce29b5b/accountreloss.action" : "http://xyk.jlu.edu.cn/accountreloss.action"); }).Start();
            }
            else if (url.Contains(vpn ? "https://webvpn.jlu.edu.cn/http/77726476706e69737468656265737421e8ee4ad22d3c7d1e7b0c9ce29b5b/accountreloss.action" : "http://xyk.jlu.edu.cn/accountreloss.action"))//解挂页面加载完成，自动操作
            {
                CancelLostWebChromeClient.GetSources(view);//获取页面源码
                AnalyzeHtmlThread(view, () =>
                {
                    Thread.Sleep(1000);
                    view.LoadUrl("javascript:document.getElementById(\"passwd\").value = " + JLU.Loader.Ykt.Password);//输入密码
                    Thread.Sleep(200);
                    view.LoadUrl("javascript:formcheck();");//提交
                    Thread.Sleep(200);
                    view.Clickable = true;//恢复可点击
                }).Start();
            }

        }
    }
}