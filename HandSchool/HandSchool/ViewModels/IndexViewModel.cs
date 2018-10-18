using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 首页内容的视图模型，提供了天气、课时信息和标题信息。
    /// </summary>
    public class IndexViewModel : BaseViewModel
    {
        static IndexViewModel instance = null;

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static IndexViewModel Instance
        {
            get
            {
                if (instance is null)
                    instance = new IndexViewModel();
                return instance;
            }
        }

        /// <summary>
        /// 创建首页信息的视图模型，并更新数据。
        /// </summary>
        private IndexViewModel()
        {
            Title = "掌上校园";
            Core.LoginStateChanged += UpdateWelcome;
            RefreshCommand = new Command(async () => await Refresh());
            RequestLoginCommand = new Command(async () => await RequestLogin());
        }

        #region Weather

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
        /// 从公共API更新天气数据。
        /// </summary>
        [ToFix("JObject无法读取数据")]
        public async Task UpdateWeather()
        {
            try
            {
                var wc = new AwaredWebClient("https://www.sojson.com/open/api/weather/", Encoding.UTF8);
                var weatherJson = await wc.GetAsync("json.shtml?city=" + Core.App.Service.WeatherLocation);
                JObject jo = (JObject)JsonConvert.DeserializeObject(weatherJson);
                if((int)(jo["status"])==304)
                {
                    Weather = "天气信息获取失败";
                    return;
                }
                string high = jo["data"]["forecast"][0]["high"].ToString();
                string low = jo["data"]["forecast"][0]["low"].ToString();
                Weather = jo["data"]["forecast"][0]["type"].ToString();
                WeatherTips = jo["data"]["forecast"][0]["notice"].ToString();
                WeatherRange = $"{high} {low}";
            }
            catch (Exception ex)
            {
                Weather = "天气信息获取失败";
                Core.Log(ex);
            }
        }

        #endregion

        #region Next Curriculum

        CurriculumItem curriculum2;
        CurriculumItem curriculum1;

        /// <summary>
        /// 接下来的课
        /// </summary>
        public CurriculumItem NextClass
        {
            get => curriculum1;
            set => SetProperty(ref curriculum1, value, onChanged: UpdateHasClass);
        }
        
        /// <summary>
        /// 正在进行的课
        /// </summary>
        public CurriculumItem CurrentClass
        {
            get => curriculum2;
            set => SetProperty(ref curriculum2, value, onChanged: UpdateHasClass);
        }
        
        /// <summary>
        /// 更新有无课的显示状态。
        /// </summary>
        private void UpdateHasClass()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged("NextHasClass");
                OnPropertyChanged("CurrentHasClass");
                OnPropertyChanged("NoClass");
            });
        }

        /// <summary>
        /// 当前是否有课
        /// </summary>
        public bool CurrentHasClass => curriculum2 != null;

        /// <summary>
        /// 接下来是否有课
        /// </summary>
        public bool NextHasClass => curriculum1 != null;

        /// <summary>
        /// 当前是否没有课
        /// </summary>
        public bool NoClass => curriculum1 is null && curriculum2 is null;

        /// <summary>
        /// 更新当前时间对应的课程。
        /// </summary>
        private void UpdateNextCurriculum()
        {
            int today = (int)DateTime.Now.DayOfWeek;
            if (today == 0) today = 7;
            int toweek = Core.App.Service.CurrentWeek;
            int tocor = Core.App.Schedule.ClassNext;
            NextClass = ScheduleViewModel.Instance.FindItem((obj) => obj.IfShow(toweek) && obj.WeekDay == today && obj.DayBegin > tocor);
            CurrentClass = ScheduleViewModel.Instance.FindLastItem((obj) => obj.IfShow(toweek) && obj.WeekDay == today && obj.DayBegin > tocor - 3 && obj.DayEnd <= tocor);
        }

        #endregion

        #region Welcome Messages

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
                WelcomeMessage = service.WelcomeMessage;
                CurrentMessage = service.CurrentMessage;
            }
        }

        #endregion

        #region Refresh Command

        /// <summary>
        /// 刷新视图模型数据的命令
        /// </summary>
        public Command RefreshCommand { get; set; }

        /// <summary>
        /// 请求登录的命令
        /// </summary>
        public Command RequestLoginCommand { get; set; }

        /// <summary>
        /// 请求登录，这样更优雅。（？？？）
        /// </summary>
        private async Task RequestLogin()
        {
            if (!Core.Initialized) return;
            if (Core.App.Service.IsLogin) return;
            await LoginViewModel.RequestAsync(Core.App.Service);
        }

        /// <summary>
        /// 与目前教务系统和课程表数据进行同步。
        /// </summary>
        private async Task Refresh()
        {
            if (IsBusy) return;
            IsBusy = true;
            
            if (!ScheduleViewModel.Instance.ItemsLoaded)
            {
                // This time, the main-cost service has not been created.
                // So we can force this method to be asynchronized
                // that won't block the enter of main page.
                await Task.Yield();
            }

            // UpdateWelcome();
            UpdateNextCurriculum();
            IsBusy = false;
            await UpdateWeather();
        }

        #endregion
    }
}
