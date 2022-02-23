using HandSchool.Internals;
using HandSchool.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HandSchool.Internal;
using HandSchool.Services;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 首页内容的视图模型，提供了天气、课时信息和标题信息。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    public sealed partial class IndexViewModel : BaseViewModel
    {
        static readonly Lazy<IndexViewModel> Lazy =
            new Lazy<IndexViewModel>(() => new IndexViewModel());
        public event Action<object, ClassLoadedEventArgs> CurrentClassesLoadFinished;
        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static IndexViewModel Instance => Lazy.Value;
        public ICommand CheckUpdateCommand { get; set; }
        /// <summary>
        /// 创建首页信息的视图模型，并更新数据。
        /// </summary>
        private IndexViewModel()
        {
            Title = "掌上校园";
            Core.App.LoginStateChanged += UpdateWelcome;
            RefreshCommand = new CommandAction(Refresh);
            RequestLoginCommand = new CommandAction(RequestLogin);
            CheckUpdateCommand = new CommandAction(Core.Platform.CheckUpdate);
            WeatherReport = Core.New<IWeatherReport>();
            WeatherReport.CityCode = Core.App.Service.WeatherLocation;
            _currentWeather = _todayWeather = _tomorrowWeather = "正在加载...";
            _weatherProvider = "数据来自：";
            _weatherNotice = "愿你拥有比阳光明媚的心情";
        }
        
        /// <summary>
        /// 刷新视图模型数据的命令
        /// </summary>
        public ICommand RefreshCommand { get; set; }

        /// <summary>
        /// 请求登录的命令
        /// </summary>
        public ICommand RequestLoginCommand { get; set; }

        public static Func<Task<TaskResp>> BeforeOperatingCheck { set; private get; }
        
        public IWeatherReport WeatherReport { get; set; }

        /// <summary>
        /// 请求登录，防止用户有程序没反应的错觉（大雾）
        /// </summary>
        public async Task RequestLogin()
        {
            if (!Core.Initialized) return;
            if (!Core.App.Service.NeedLogin) return;
            if (IsBusy) return;

            IsBusy = true;
            if (BeforeOperatingCheck != null)
            {
                var msg = await BeforeOperatingCheck();
                if (!msg.IsSuccess)
                {
                    await RequestMessageAsync("错误", msg.ToString());
                    IsBusy = false;
                    return;
                }
            }
            IsBusy = false;

            await Core.App.Service.RequestLogin();
        }

        private bool _isWorking;
        private readonly TimeoutManager _weatherTimeoutManager = new TimeoutManager(3600);

        public async Task RefreshWeather()
        {
            if (!_isWorking && (_weatherTimeoutManager.NotInit || _weatherTimeoutManager.IsTimeout()))
            {
                _isWorking = true;
                try
                {
                    WeatherProvider = $"数据来自：{WeatherReport.Provider}";
                    await WeatherReport.UpdateWeatherAsync();
                    if (!string.IsNullOrWhiteSpace(WeatherReport.CurrentTemperature.Notice))
                    {
                        WeatherNotice = WeatherReport.CurrentTemperature.Notice;
                    }
                    CurrentWeather =
                        $"{WeatherReport.CurrentTemperature} {WeatherReport.CurrentTemperature.Description}";
                    var report = WeatherReport.ForecastTemperature;
                    if (report.Count < 2)
                    {
                        TodayWeather = TomorrowWeather = "未知 ~ 未知";
                        return;
                    }

                    TodayWeather =
                        $"{report[0].From} ~ {report[0].To} {(report[0].From.Description == report[0].To.Description ? report[0].From.Description : $"{report[0].From.Description}转{report[0].To.Description}")}";
                    TomorrowWeather =
                        $"{report[1].From} ~ {report[1].To} {(report[1].From.Description == report[1].To.Description ? report[1].From.Description : $"{report[1].From.Description}转{report[1].To.Description}")}";
                    _weatherTimeoutManager.Refresh();
                }
                catch (Exception e)
                {
                    Core.Logger.WriteLine("更新天气错误", e.Message);
                    throw;
                }
                finally
                {
                    _isWorking = false;
                }
            }
        }

        /// <summary>
        /// 与目前教务系统和课程表数据进行同步。
        /// </summary>
        public async Task Refresh()
        {
            if (IsBusy) return;
            IsBusy = true;

            if (!ScheduleViewModel.Instance.ItemsLoaded)
            {
                // This time, the main-cost service has not been created.
                // So we can force this method to be on another execution context
                // that won't block the enter of main page.
                await Task.Yield();
            }

            var res = UpdateTodayCurriculum();
            
            //UIMS加载完成，通知刷新周数信息等
            Core.App.Loader.NoticeChange?.Invoke(Core.App.Service, new LoginStateEventArgs(LoginState.Succeeded));
            IsBusy = false;

            var args = new ClassLoadedEventArgs
            {
                Classes = res
            };
            CurrentClassesLoadFinished?.Invoke(this, args);
        }
    }
}