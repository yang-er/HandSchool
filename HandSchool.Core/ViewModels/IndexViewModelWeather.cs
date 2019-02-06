using HandSchool.Internals;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace HandSchool.ViewModels
{
    public sealed partial class IndexViewModel
    {
        string weather;
        string weatherRange;
        string weatherTips = "愿你拥有比阳光明媚的心情";

        /// <summary>
        /// 天气形容词
        /// </summary>
        public string Weather
        {
            get => weather;
            set => SetProperty(ref weather, value);
        }

        /// <summary>
        /// 气温范围
        /// </summary>
        public string WeatherRange
        {
            get => weatherRange;
            set => SetProperty(ref weatherRange, value);
        }

        /// <summary>
        /// 天气小贴士
        /// </summary>
        public string WeatherTips
        {
            get => weatherTips;
            set => SetProperty(ref weatherTips, value);
        }

        /// <summary>
        /// 网络客户端
        /// </summary>
        private static IWebClient WebClient { get; set; }

        /// <summary>
        /// 从公共API更新天气数据。
        /// </summary>
        public async Task UpdateWeather()
        {
            try
            {
                if (WebClient is null)
                {
                    WebClient = Core.New<IWebClient>();
                    WebClient.BaseAddress = "http://t.weather.sojson.com/api/weather/city/";
                }
                
                var weatherJson = await WebClient.GetStringAsync(Core.App.Service.WeatherLocation);
                var jo = JsonConvert.DeserializeObject(weatherJson) as JObject;

                if ((int)jo["status"] == 304)
                {
                    Weather = "天气信息获取失败";
                    throw new Exception("Status 304");
                }

                var high = jo["data"]["forecast"][0]["high"].ToString();
                var low = jo["data"]["forecast"][0]["low"].ToString();
                Weather = jo["data"]["forecast"][0]["type"].ToString();
                WeatherTips = jo["data"]["forecast"][0]["notice"].ToString();
                WeatherRange = $"{high} {low}";
            }
            catch (Exception ex)
            {
                Weather = "天气信息获取失败";
                this.WriteLog(ex);
            }
        }
    }
}