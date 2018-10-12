using HandSchool.Models;
using HandSchool.Services;
using HandSchool.Views;
using System;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class ScheduleViewModel : BaseViewModel
    {
        private int week;
        private static ScheduleViewModel instance = null;

        public int Week => week;
        public string CurrentWeek => week == 0 ? "所有周" : $"第{week}周";
        public Command RefreshCommand { get; set; }
        public Command AddCommand { get; set; }
        public Command ChangeWeekCommand { get; set; }
        public event Action RefreshComplete;

        public static ScheduleViewModel Instance
        {
            get
            {
                if (instance is null)
                    instance = new ScheduleViewModel();
                return instance;
            }
        }

        void SyncData(object sender, LoginStateEventArgs e)
        {
            if (e.State != LoginState.Succeeded) return;
            var sys = sender as ISchoolSystem;
            SetProperty(ref week, sys.CurrentWeek, nameof(CurrentWeek));
        }

        public void SetCurrentWeek(int n)
        {
            SetProperty(ref week, n, nameof(CurrentWeek));
        }

        private ScheduleViewModel()
        {
            Core.App.Service.LoginStateChanged += SyncData;
            week = Core.App.Service.CurrentWeek;
            RefreshCommand = new Command(Refresh);
            AddCommand = new Command(Create);
            ChangeWeekCommand = new Command(ChangeWeek);
            Title = "课程表";
        }

        async void Refresh()
        {
            if (IsBusy) return;
            SetIsBusy(true, "正在加载课程表……");
            await Core.App.Schedule.Execute();
            RefreshComplete?.Invoke();
            SetIsBusy(false);
        }

        async void ChangeWeek()
        {
            var paramlist = new string[25];
            paramlist[0] = "所有周";
            for (int i = 1; i < 25; i++)
                paramlist[i] = $"第{i}周";
            var ret = await View.DisplayActionSheet("显示周", "取消", null, paramlist);

            if (ret != "取消")
            {
                for (int i = 0; i < 25; i++)
                    if (ret == paramlist[i])
                        SetCurrentWeek(i);
                RefreshComplete?.Invoke();
            }
        }

#if __UWP__
        async void Create()
        {
            var box = new CurriculumDialog(new CurriculumItem { IsCustom = true, CourseID = "CUSTOM-" + DateTime.Now.ToString("s") }, true);
            var result = await box.ShowAsync();
            if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary) RefreshComplete?.Invoke();
        }
#else
        async void Create(object param)
        {
            await (new CurriculumPage(new CurriculumItem { IsCustom = true, CourseID = "CUSTOM-" + DateTime.Now.ToString("s") }, true)).ShowAsync(param as INavigation);
        }
#endif
    }
}
