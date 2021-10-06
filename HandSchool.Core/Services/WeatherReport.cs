using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HandSchool.Services
{

    #region Json解析需要
    public class TemperatureSingle
    {
        public string unit { get; set; }
        public double value { get; set; }
    }

    public class TemperatureDouble
    {
        public string unit { get; set; }
        public List<Value<double>> value { get; set; }
    }

    public class Current
    {
        public TemperatureSingle temperature { get; set; }
        public string weather { get; set; }
    }

    public class Value<T>
    {
        public T @from { get; set; }
        public T to { get; set; }

        public bool IsFromEqualsTo()
        {
            return @from.Equals(to);
        }
    }

    public class ForecastDaily
    {
        public TemperatureDouble temperature { get; set; }
        public Weather weather { get; set; }

    }

    public class Weather
    {
        public List<Value<int>> value { get; set; }
    }

    public class WeatherReport
    {

        public Current current { get; set; }
        public ForecastDaily forecastDaily { get; set; }
    }

    public class WeatherStatus
    {
        private static readonly string[] _weatherStatus =
        {
            "晴", "多云", "阴", "阵雨", "雷阵雨", "雷阵雨并伴有冰雹", "雨夹雪", "小雨",
            "中雨", "大雨", "暴雨", "大暴雨", "特大暴雨", "阵雪", "小雪", "中雪", "大雪", "暴雪", "雾", "冻雨",
            "沙尘暴", "小雨-中雨", "中雨-大雨", "大雨-暴雨", "暴雨-大暴雨", "大暴雨-特大暴雨", "小雪-中雪", "中雪-大雪",
            "大雪-暴雪", "浮沉", "扬沙", "强沙尘暴", "飑", "龙卷风", "若高吹雪", "轻雾"
        };

        public string this[int index]
        {
            get
            {
                switch (index)
                {
                    case 53: return "霾";
                    case 99: return "未知";
                    default: return _weatherStatus[index];
                }
            }
        }
    }
    #endregion
    

    public class WeatherClient
    {
        public WeatherClient(string cityNum) => _cityNum = cityNum;

        private readonly string _cityNum;
        private HttpClient _httpClient;
        private readonly WeatherStatus _weatherStatus = new WeatherStatus();
        private WeatherReport _weatherReport;
        
        //当前的天气描述
        public string WeatherDescription => _weatherStatus[int.Parse(_weatherReport.current.weather)];
        //15天内天气描述
        public IList<Value<string>> WeatherDescriptions
        {
            get
            {
                var res = new Value<string>[_weatherReport.forecastDaily.weather.value.Count];
                for (var i = 0; i < res.Length; i++)
                {
                    res[i] = new Value<string>
                    {
                        @from = _weatherStatus[_weatherReport.forecastDaily.weather.value[i].@from],
                        to = _weatherStatus[_weatherReport.forecastDaily.weather.value[i].to]
                    };
                }

                return res;
            }
        }
        
        //当前温度
        public TemperatureSingle CurrentTemperature => _weatherReport.current.temperature;
        //15天内温度情况
        public TemperatureDouble ForecastTemperature => _weatherReport.forecastDaily.temperature;

        private bool _isUpdating = false;
        //异步更新天气
        public async Task UpdateWeatherAsync()
        {
            if (_isUpdating) return;
            _isUpdating = true;
            _httpClient ??= new HttpClient();
            _httpClient.Timeout = new TimeSpan(0, 0, 0, 2);
            var json = await _httpClient.GetStringAsync(
                $"https://weatherapi.market.xiaomi.com/wtr-v3/weather/all?latitude=0&longitude=0&locale=zh-cn&isGlobal=false&locationKey=weathercn:{_cityNum}&appKey=weathercn:{_cityNum}&sign=zUFJoAR2ZVrDy1vF3D07&days=15");
            _weatherReport = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherReport>(json);
            _httpClient.Dispose();
            _httpClient = null;
            _isUpdating = false;
        }
    }
}