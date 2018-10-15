using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
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
            Core.App.LoginStateChanged += Global_LoginStateChanged;
            RefreshCommand = new Command(async () => await Refresh());
            RefreshCommand.Execute(null);
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
        public string CurrentClass => curriculum2?.Name != default(string) ? curriculum2.Name : "无（现在没有课或未刷新）";
        public string CurrentTeacher => curriculum2?.Teacher;
        public string CurrentClassroom => curriculum2?.Classroom;
        public bool CurrentHasClass => curriculum2 != null || curriculum1 is null;

        CurriculumItem curriculum1;
        public string NextClass => curriculum1?.Name != default(string) ? curriculum1.Name : "无（接下来没有课或未刷新）";
        public string NextTeacher => curriculum1?.Teacher;
        public string NextClassroom => curriculum1?.Classroom;
        public bool NextHasClass => curriculum1 != null;

        void UpdateNextCurriculum()
        {
            int today = (int)DateTime.Now.DayOfWeek;
            if (today == 0) today = 7;
            int toweek = Core.App.Service.CurrentWeek;
            int tocor = Core.App.Schedule.ClassNext;
            curriculum1 = ScheduleViewModel.Instance.FindItem((obj) => obj.IfShow(toweek) && obj.WeekDay == today && obj.DayBegin > tocor);
            curriculum2 = ScheduleViewModel.Instance.FindLastItem((obj) => obj.IfShow(toweek) && obj.WeekDay == today && obj.DayBegin > tocor - 3 && obj.DayEnd <= tocor);

            Device.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged("NextClass");
                OnPropertyChanged("NextTeacher");
                OnPropertyChanged("NextClassroom");
                OnPropertyChanged("CurrentClass");
                OnPropertyChanged("CurrentTeacher");
                OnPropertyChanged("CurrentClassroom");
                OnPropertyChanged("NextHasClass");
                OnPropertyChanged("CurrentHasClass");
            });
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
        /// 更新欢迎消息为目前教务系统的实例。
        /// </summary>
        private void UpdateWelcome()
        {
            WelcomeMessage = Core.App.Service.WelcomeMessage;
            CurrentMessage = Core.App.Service.CurrentMessage;
        }
        
        #endregion

        #region Refresh Command

        /// <summary>
        /// 当教务系统服务状态更改时，同步欢迎消息。
        /// </summary>
        /// <param name="sender">正在使用的教务系统。</param>
        /// <param name="args">目前的登录状态。</param>
        private void Global_LoginStateChanged(object sender, LoginStateEventArgs args)
        {
            if (args.State == LoginState.Succeeded) UpdateWelcome();
        }

        /// <summary>
        /// 刷新视图模型数据的命令
        /// </summary>
        public Command RefreshCommand { get; set; }

        /// <summary>
        /// 与目前教务系统和课程表数据进行同步。
        /// </summary>
        private async Task Refresh()
        {
            if (IsBusy) return;
            IsBusy = true;
            
            if (!ScheduleViewModel.Instance.ItemsLoader.IsValueCreated)
            {
                // This time, the main-cost service has not been created.
                // So we can force this method to be asynchronized
                // that won't block the enter of main page.
                await Task.Yield();
            }

            UpdateWelcome();
            UpdateNextCurriculum();
            IsBusy = false;
            await UpdateWeather();
        }

        #endregion
    }
}
