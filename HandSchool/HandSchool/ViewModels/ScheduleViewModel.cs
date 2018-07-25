using HandSchool.Models;
using HandSchool.Services;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class ScheduleViewModel : BaseViewModel
    {
        private int week;
        private static ScheduleViewModel instance = null;

        public int Week => week;
        public string CurrentWeek => $"第{week}周";
        public Command RefreshCommand { get; set; }
        public Command AddCommand { get; set; }
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
            SetProperty(ref week, sys.CurrentWeek, "CurrentWeek");
        }

        private ScheduleViewModel()
        {
            Core.App.Service.LoginStateChanged += SyncData;
            week = Core.App.Service.CurrentWeek;
            RefreshCommand = new Command(Refresh);
            AddCommand = new Command(Create);
            Title = "课程表";
        }

        async void Refresh()
        {
            if (IsBusy) return;
            IsBusy = true;
            View.SetIsBusy(true, "正在加载课程表……");
            await Core.App.Schedule.Execute();
            RefreshComplete?.Invoke();
            View.SetIsBusy(false);
            IsBusy = false;
        }

#if __UWP__
        async void Create()
        {
            var box = new UWP.Views.CurriculumDialog(new CurriculumItem { IsCustom = true, CourseID = "CUSTOM-" + DateTime.Now.ToString("s") }, true);
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
