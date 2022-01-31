using Android.Webkit;
using AndroidX.AppCompat.App;
using HandSchool.Internals;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HandSchool.JLU.Services;
using HandSchool.Pages;
using HandSchool.Services;
using HandSchool.Views;
using Newtonsoft.Json.Linq;
using AppActivity = AndroidX.AppCompat.App.AppCompatActivity;


namespace HandSchool.Droid.Internals
{
    public static class Weather
    {
        private static readonly string[] Status =
        {
            "晴", "多云", "阴", "阵雨", "雷阵雨", "雷阵雨并伴有冰雹", "雨夹雪", "小雨",
            "中雨", "大雨", "暴雨", "大暴雨", "特大暴雨", "阵雪", "小雪", "中雪", "大雪", "暴雪", "雾", "冻雨",
            "沙尘暴", "小雨-中雨", "中雨-大雨", "大雨-暴雨", "暴雨-大暴雨", "大暴雨-特大暴雨", "小雪-中雪", "中雪-大雪",
            "大雪-暴雪", "浮沉", "扬沙", "强沙尘暴", "飑", "龙卷风", "若高吹雪", "轻雾"
        };

        private static readonly IReadOnlyDictionary<string, string> Notices = new Dictionary<string, string>
        {
            {"阴", "不要被阴云遮挡住好心情"},
            {"小雪", "小雪虽美，赏雪别着凉"},
            {"多云", "阴晴之间，谨防紫外线侵扰"},
            {"晴", "愿你拥有比阳光明媚的心情"},
            {"中雨", "记得随身携带雨伞哦"},
            {"小雨", "雨虽小，注意保暖别感冒"},
            {"霾", "雾霾来袭，戴好口罩再出门"},
            {"大雨", "出门最好穿雨衣，勿挡视线"},
            {"中雪", "凉爽勿过度，保暖为主"},
            {"雾", "今日有雾，出行注意安全"},
            {"大雪", "大雪纷飞下，记得早回家"},
        };

        public static string GetStatue(int index)
        {
            return index switch
            {
                53 => "霾",
                99 => "未知",
                _ => Status[index]
            };
        }

        public static string GetNotice(string statue)
        {
            return Notices.ContainsKey(statue) ? Notices[statue] : "愿你拥有比阳光明媚的心情";
        }
        public static bool IsMi()
        {
            return Android.OS.Build.Manufacturer?.ToLower()?.Contains("xiaomi") ?? false;
        }
    }

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
        public override async Task CloseAsync()
        {
            var context = PlatformImplV2.Instance.PeekContext(false);
            if (context is WebLoginActivity)
            {
                await Navigation.PopAsync();
            }

            await base.CloseAsync();
        }

        public override Task ShowAsync()
        {
            var context = PlatformImplV2.Instance.PeekContext();
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
        public void OnReceiveValue(Java.Lang.Object value)
        {
            return;
        }
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
            if (message.StartsWith("<html")) Sources = message; //用来获取网页源代码的Alert
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
                var activity = (AppActivity) chromeClient.activity;

                //等待加载完成，分析网页源码
                while (!chromeClient.LoadState)
                    Thread.Sleep(10);
                var doc = new HtmlDocument();
                var str = chromeClient.Sources;
                doc.LoadHtml(str);
                var error = doc.DocumentNode.SelectNodes("//p[@class='biaotou']");
                if (error == null) //没有错误就填写密码
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
                        new AlertDialog.Builder((AppActivity) chromeClient.activity)
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
            if (url.Contains(vpn
                    ? "https://webvpn.jlu.edu.cn/http/77726476706e69737468656265737421e8ee4ad22d3c7d1e7b0c9ce29b5b/homeLogin.action"
                    : "http://xyk.jlu.edu.cn/homeLogin.action")) //登录页面加载完成, 填密码
            {
                CancelLostWebChromeClient.GetSources(view); //获取页面源码
                AnalyzeHtmlThread(view, () =>
                {
                    view.EvaluateJavascript("document.FormPost.name.value = " + JLU.Loader.Ykt.Username,
                        new JSNoResult());
                    view.EvaluateJavascript("document.FormPost.passwd.value = " + JLU.Loader.Ykt.Password,
                        new JSNoResult());
                }).Start();
            }
            else if (url.Contains(vpn
                         ? "https://webvpn.jlu.edu.cn/http/77726476706e69737468656265737421e8ee4ad22d3c7d1e7b0c9ce29b5b/loginstudent.action"
                         : "http://xyk.jlu.edu.cn/loginstudent.action")) //登录成功
            {
                view.Clickable = false;
                CancelLostWebChromeClient.GetSources(view); //获取页面源码
                AnalyzeHtmlThread(view,
                    () =>
                    {
                        view.LoadUrl(vpn
                            ? "https://webvpn.jlu.edu.cn/http/77726476706e69737468656265737421e8ee4ad22d3c7d1e7b0c9ce29b5b/accountreloss.action"
                            : "http://xyk.jlu.edu.cn/accountreloss.action");
                    }).Start();
            }
            else if (url.Contains(vpn
                         ? "https://webvpn.jlu.edu.cn/http/77726476706e69737468656265737421e8ee4ad22d3c7d1e7b0c9ce29b5b/accountreloss.action"
                         : "http://xyk.jlu.edu.cn/accountreloss.action")) //解挂页面加载完成，自动操作
            {
                CancelLostWebChromeClient.GetSources(view); //获取页面源码
                AnalyzeHtmlThread(view, () =>
                {
                    Thread.Sleep(1000);
                    view.LoadUrl("javascript:document.getElementById(\"passwd\").value = " +
                                 JLU.Loader.Ykt.Password); //输入密码
                    Thread.Sleep(200);
                    view.LoadUrl("javascript:formcheck();"); //提交
                    Thread.Sleep(200);
                    view.Clickable = true; //恢复可点击
                }).Start();
            }

        }
    }

    public class MiWeatherReport : IWeatherReport
    {
        public MiWeatherReport()
        {
            ForecastTemperature = new List<TemperatureInfo>();
        }

        public string Provider => "小米天气";
        public string CityCode { get; set; }

        //当前温度
        public Temperature CurrentTemperature { get; private set; }

        //15天内温度情况
        public List<TemperatureInfo> ForecastTemperature { get; }

        private bool _isUpdating;

        private static Temperature ParseTemp(double? value, string unit, int? statue)
        {
            var temperatureUnit =
                (unit?.Trim() ?? "") switch
                {
                    "℉" => TemperatureUnit.Fahrenheit,
                    "K" => TemperatureUnit.Kelvin,
                    _ => TemperatureUnit.Celsius
                };
            value ??= temperatureUnit switch
            {
                TemperatureUnit.Celsius => -273.15,
                TemperatureUnit.Fahrenheit => -459.67,
                TemperatureUnit.Kelvin => 0,
                _ => throw new InvalidOperationException()
            };
            var desc = "未知";
            if (statue != null)
            {
                desc = Weather.GetStatue((int) statue);
            }

            return new Temperature
            {
                Value = (double) value,
                Description = desc,
                Unit = temperatureUnit
            };
        }

        private void ParseWeather(JObject jObject)
        {
            var cur = jObject?["current"];
            var curTemp = cur?["temperature"];
            var curTempValue = curTemp?["value"]?.ToObject<double?>();
            var curTempUint = curTemp?["unit"]?.ToString();
            var curTempDes = cur?["weather"]?.ToObject<int?>();
            var curT = ParseTemp(curTempValue, curTempUint, curTempDes);
            curT.Notice = Weather.GetNotice(curT.Description);
            CurrentTemperature = curT;

            var fore = jObject?["forecastDaily"];
            var foreTemp = fore?["temperature"];
            var foreTempUnit = foreTemp?["unit"]?.ToString();
            var foreTempValues = foreTemp?["value"]?.ToObject<List<JObject>>();
            var foreDes = fore?["weather"]?["value"]?.ToObject<List<JObject>>();
            ForecastTemperature.Clear();
            if (foreTempValues != null && foreDes != null)
            {
                for (var i = 0; i < foreTempValues.Count; i++)
                {
                    var curTv = foreTempValues[i];
                    var curDes = foreDes[i];
                    var @from = ParseTemp(curTv?["from"]?.ToObject<double?>(), foreTempUnit,
                        curDes?["from"]?.ToObject<int?>());
                    var to = ParseTemp(curTv?["to"]?.ToObject<double?>(), foreTempUnit,
                        curDes?["to"]?.ToObject<int?>());
                    ForecastTemperature.Add(new TemperatureInfo
                    {
                        From = @from,
                        To = to
                    });
                }
            }
        }

        public async Task UpdateWeatherAsync()
        {
            if (_isUpdating) return;
            _isUpdating = true;
            string json = null;
            using (var httpClient = new HttpClientImpl())
            {
                httpClient.Timeout = 2000;
                json = await httpClient.GetStringAsync(
                    $"https://weatherapi.market.xiaomi.com/wtr-v3/weather/all?latitude=0&longitude=0&locale=zh-cn&isGlobal=false&locationKey=weathercn:{CityCode}&appKey=weathercn:{CityCode}&sign=zUFJoAR2ZVrDy1vF3D07&days=15");
            }

            if (string.IsNullOrEmpty(json))
            {
                _isUpdating = false;
                return;
            }

            var jo = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(json);
            ParseWeather(jo);
            _isUpdating = false;
        }
    }
}