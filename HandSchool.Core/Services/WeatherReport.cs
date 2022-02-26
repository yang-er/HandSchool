using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HandSchool.Internals;
using Newtonsoft.Json.Linq;

namespace HandSchool.Services
{
    public enum TemperatureUnit
    {
        Celsius,
        Fahrenheit,
        Kelvin
    }
    public struct Temperature
    {
        public double Value { get; set; }
        public TemperatureUnit Unit { get; set; }

        public string Description { get; set; }

        public string Notice { get; set; }

        public override string ToString()
        {
            var unit = Unit switch
            {
                TemperatureUnit.Celsius => '℃',
                TemperatureUnit.Fahrenheit => '℉',
                TemperatureUnit.Kelvin => 'K',
                _ => throw new ArgumentOutOfRangeException()
            };
            return $"{Value}{unit}";
        }
    }
    public struct TemperatureInfo
    {
        public Temperature From { get; set; }
        public Temperature To { get; set; }
    }
    
    public static class WeatherUtil
    {
        public static double GetDefaultValue(TemperatureUnit unit)
        {
            return unit switch
            {
                TemperatureUnit.Celsius => -273.15,
                TemperatureUnit.Fahrenheit => -459.67,
                TemperatureUnit.Kelvin => 0,
                _ => throw new InvalidOperationException()
            };
        }
    }
    public interface IWeatherReport
    {
        public string CityCode { get; set; }
        public string Provider { get; }
        //当前温度
        public Temperature CurrentTemperature { get;}
        //15天内温度情况
        public List<TemperatureInfo> ForecastTemperature { get; }
        public Task UpdateWeatherAsync();
    }
    
    public class DefaultWeatherReport : IWeatherReport
    {
        public string Provider => "SOJOSN";
        public DefaultWeatherReport()
        {
            ForecastTemperature = new List<TemperatureInfo>();
        }

        public string CityCode { get; set; }

        //当前温度
        public Temperature CurrentTemperature { get; private set; }

        //15天内温度情况
        public List<TemperatureInfo> ForecastTemperature { get; }

        private bool _isUpdating;

        private void ParseWeather(JObject jObject)
        {
            var data = jObject?["data"];
            var curTemp = data?["wendu"]?.ToObject<double?>();
            var curT = new Temperature
            {
                Value = curTemp ?? WeatherUtil.GetDefaultValue(TemperatureUnit.Celsius),
                Unit = TemperatureUnit.Celsius,
            };
            var fores = data?["forecast"]?.ToObject<List<JObject>>();
            if ((fores?.Count ?? 0) > 0)
            {
                curT.Notice = fores[0]["notice"]?.ToString();
            }
            CurrentTemperature = curT;

            fores?.ForEach(j =>
            {
                var high = j?["high"]?.ToString().Replace("高温 ", "").Replace("℃", "");
                double.TryParse(high, out var heightNum);
                var low = j?["low"]?.ToString().Replace("低温 ", "").Replace("℃", "");
                double.TryParse(low, out var lowNum);
                var dec = j?["type"]?.ToString();
                ForecastTemperature.Add(new TemperatureInfo
                {
                    From = new Temperature
                    {
                        Value = heightNum,
                        Unit = TemperatureUnit.Celsius,
                        Description = dec,
                    },
                    To = new Temperature
                    {
                        Value = lowNum,
                        Unit = TemperatureUnit.Celsius,
                        Description = dec,
                    }
                });
            });
        }

        public async Task UpdateWeatherAsync()
        {
            if (_isUpdating) return;
            _isUpdating = true;
            string json = null;
            using (var httpClient = new HttpClientImpl())
            {
                httpClient.Timeout = 2000;
                httpClient.StringBaseAddress = "http://t.weather.sojson.com/api/weather/city/";
                json = await httpClient.GetStringAsync(CityCode);
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