﻿using HandSchool.Internals;
using HandSchool.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
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
        public event Action<object, Views.ClassLoadEventArgs> CurrentClassesLoadFinished;
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
            WeatherClient = new WeatherClient(Core.App.CityWeatherCode);
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
        
        public WeatherClient WeatherClient { get; set; }

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

            await LoginViewModel.RequestAsync(Core.App.Service);
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
            Core.App.Loader.NoticeChange?.Invoke(Core.App.Service, new LoginStateEventArgs(LoginState.Succeeded));
            IsBusy = false;

            var args = new Views.ClassLoadEventArgs
            {
                Classes = res
            };
            CurrentClassesLoadFinished?.Invoke(this, args);
        }
    }
}