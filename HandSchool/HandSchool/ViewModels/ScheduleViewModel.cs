using HandSchool.Models;
using HandSchool.Services;
using HandSchool.Views;
using System;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 课程表的视图模型，提供了增删改查功能。
    /// </summary>
    [ToFix("将数据源迁移至视图模型")]
    public class ScheduleViewModel : BaseViewModel
    {
        private int week;

        /// <summary>
        /// 当前周
        /// </summary>
        public int Week => week;

        /// <summary>
        /// 当前周的文字描述
        /// </summary>
        public string CurrentWeek => week == 0 ? "所有周" : $"第{week}周";

        /// <summary>
        /// 课程表刷新完成的事件
        /// </summary>
        public event Action RefreshComplete;

        /// <summary>
        /// 将当前周、增删改查等操作加载。
        /// </summary>
        private ScheduleViewModel()
        {
            Core.App.Service.LoginStateChanged += SyncData;
            week = Core.App.Service.CurrentWeek;
            RefreshCommand = new Command(Refresh);
            AddCommand = new Command(Create);
            ChangeWeekCommand = new Command(ChangeWeek);
            Title = "课程表";
        }

        /// <summary>
        /// 当教务系统登录时，同步目前周。
        /// </summary>
        private void SyncData(object sender, LoginStateEventArgs e)
        {
            if (e.State != LoginState.Succeeded) return;
            var sys = sender as ISchoolSystem;
            SetProperty(ref week, sys.CurrentWeek, nameof(CurrentWeek));
        }

        /// <summary>
        /// 刷新课程表的命令
        /// </summary>
        public Command RefreshCommand { get; set; }

        /// <summary>
        /// 刷新课程表，修改当前周，并通知视图重新绘制。
        /// </summary>
        private async void Refresh()
        {
            if (IsBusy) return;
            IsBusy = true;
            await Core.App.Schedule.Execute();
            RefreshComplete?.Invoke();
            IsBusy = false;
        }

        /// <summary>
        /// 修改当前周的命令
        /// </summary>
        public Command ChangeWeekCommand { get; }

        /// <summary>
        /// 修改当前周，并通知视图重新绘制。
        /// </summary>
        private async void ChangeWeek()
        {
            var paramlist = new string[25];
            paramlist[0] = "所有周";
            for (int i = 1; i < 25; i++)
                paramlist[i] = $"第{i}周";

            var ret = await DisplayActionSheet("显示周", "取消", null, paramlist);

            if (ret != "取消")
            {
                for (int i = 0; i < 25; i++)
                {
                    if (ret == paramlist[i])
                    {
                        SetProperty(ref week, i, nameof(CurrentWeek));
                        break;
                    }
                }

                RefreshComplete?.Invoke();
            }
        }

        /// <summary>
        /// 添加课程的命令
        /// </summary>
        public Command AddCommand { get; set; }

#if __UWP__
        /// <summary>
        /// 将添加课程的页面加载。
        /// </summary>
        async void Create()
        {
            var item = new CurriculumItem
            {
                IsCustom = true,
                CourseID = "CUSTOM-" + DateTime.Now.ToString("s")
            };

            var box = new CurriculumDialog(item, true);
            var result = await box.ShowAsync();
            if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
                RefreshComplete?.Invoke();
        }
#else
        /// <summary>
        /// 将添加课程的页面加载。
        /// </summary>
        /// <param name="param">系统的导航服务。</param>
        async void Create(object param)
        {
            var item = new CurriculumItem
            {
                IsCustom = true,
                CourseID = "CUSTOM-" + DateTime.Now.ToString("s")
            };

            var page = new CurriculumPage(item, true);
            await page.ShowAsync(param as INavigation);
        }
#endif

        static ScheduleViewModel instance = null;

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static ScheduleViewModel Instance
        {
            get
            {
                if (instance is null)
                    instance = new ScheduleViewModel();
                return instance;
            }
        }
    }
}
