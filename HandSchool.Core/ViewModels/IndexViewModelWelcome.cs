using System.Diagnostics;
using HandSchool.Models;
using HandSchool.Services;

namespace HandSchool.ViewModels
{
    public sealed partial class IndexViewModel
    {
        private string _welcomeMessage = "正在加载";
        private string _currentMessage = "正在加载个人数据……";

        /// <summary>
        /// 欢迎消息
        /// </summary>
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            private set => SetProperty(ref _welcomeMessage, value);
        }

        /// <summary>
        /// 欢迎消息副标题
        /// </summary>
        public string CurrentMessage
        {
            get => _currentMessage;
            private set => SetProperty(ref _currentMessage, value);
        }

        private string _weatherNotice;
        public string WeatherNotice
        {
            get => _weatherNotice;
            set => SetProperty(ref _weatherNotice, value);
        }

        private string _currentWeather;
        private string _todayWeather;
        private string _tomorrowWeather;
        private string _weatherProvider;
        
        public string CurrentWeather
        {
            get => _currentWeather;
            set => SetProperty(ref _currentWeather, value);
        }

        public string TodayWeather
        {
            get => _todayWeather;
            set => SetProperty(ref _todayWeather, value);
        }

        public string TomorrowWeather
        {
            get => _tomorrowWeather;
            set => SetProperty(ref _tomorrowWeather, value);
        }

        public string WeatherProvider
        {
            get => _weatherProvider;
            set => SetProperty(ref _weatherProvider, value);
        }

        /// <summary>
        /// 当教务系统服务状态更改时，同步欢迎消息。
        /// </summary>
        /// <param name="sender">正在使用的教务系统。</param>
        /// <param name="args">目前的登录状态。</param>
        private void UpdateWelcome(object sender, LoginStateEventArgs args)
        {
            if (args.State == LoginState.Succeeded)
            {
                var service = sender as ISchoolSystem;
                Debug.Assert(service != null);
                WelcomeMessage = service.WelcomeMessage;
                CurrentMessage = service.CurrentMessage;
            }
        }
    }
}
