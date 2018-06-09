using HandSchool.Internal;
using HandSchool.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class IndexViewModel : BaseViewModel
    {
        private static IndexViewModel instance = null;
        public static IndexViewModel Instance
        {
            get
            {
                if (instance is null)
                    instance = new IndexViewModel();
                return instance;
            }
        }

        private IndexViewModel()
        {
            RefreshCommand = new Command(Refresh);
            Core.App.Service.LoginStateChanged += (sender, args) => { if (args.State == LoginState.Succeeded) UpdateWelcome(); };
            Title = "掌上校园";
            Refresh();
        }

        #region Weather

        string weather;
        string weatherRange;
        string weatherTips = "愿你拥有比阳光明媚的心情";
        string weatherJson;
        
        public string Weather
        {
            get => weather;
            set => SetProperty(ref weather, value);
        }

        public string WeatherRange
        {
            get => weatherRange;
            set => SetProperty(ref weatherRange, value);
        }

        public string WeatherTips
        {
            get => weatherTips;
            set => SetProperty(ref weatherTips, value);
        }
        
        public async Task UpdateWeather()
        {
            try
            {
                var wc = new AwaredWebClient("https://www.sojson.com/open/api/weather/", Encoding.UTF8);
                weatherJson = await wc.GetAsync("json.shtml?city=" + Core.App.Service.WeatherLocation);
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
                System.Diagnostics.Debug.WriteLine(ex);
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
            curriculum1 = Core.App.Schedule.Items.Find((obj) => obj.IfShow(toweek) && obj.WeekDay == today && obj.DayBegin > tocor);
            curriculum2 = Core.App.Schedule.Items.FindLast((obj) => obj.IfShow(toweek) && obj.WeekDay == today && obj.DayBegin > tocor - 3 && obj.DayEnd <= tocor);
            OnPropertyChanged("NextClass");
            OnPropertyChanged("NextTeacher");
            OnPropertyChanged("NextClassroom");
            OnPropertyChanged("CurrentClass");
            OnPropertyChanged("CurrentTeacher");
            OnPropertyChanged("CurrentClassroom");
            OnPropertyChanged("NextHasClass");
            OnPropertyChanged("CurrentHasClass");
        }

        #endregion

        #region Welcome

        public string WelcomeMessage => Core.App.Service.WelcomeMessage;
        public string CurrentMessage => Core.App.Service.CurrentMessage;

        void UpdateWelcome()
        {
            OnPropertyChanged("WelcomeMessage");
            OnPropertyChanged("CurrentMessage");
        }
        
        #endregion

        #region Refresh Command

        public Command RefreshCommand { get; set; }

        async void Refresh()
        {
            UpdateNextCurriculum();
            UpdateWelcome();
            await UpdateWeather();
        }

        #endregion

    }
}
