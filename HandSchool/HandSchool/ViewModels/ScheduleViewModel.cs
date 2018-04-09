using HandSchool.Models;
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
        private RowDefinition DefRow;
        private ColumnDefinition DefCol;
        private GridLength RowHeight, ColWidth;

        public string CurrentWeek => $"第{week}周";
        public SchedulePage BindingPage { get; set; }
        public Command RefreshCommand { get; set; }
        public Command AddCommand { get; set; }

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
            var sys = sender as ISchoolSystem;
            SetProperty(ref week, sys.CurrentWeek, "CurrentWeek");
        }

        private ScheduleViewModel()
        {
            App.Current.Service.LoginStateChanged += SyncData;
            RefreshCommand = new Command(Refresh);
            AddCommand = new Command(Create);
            Title = "课程表";
        }

        async void Refresh()
        {
            if (IsBusy) return;
            IsBusy = true;
                var alert = Internal.Helper.ShowLoadingAlert("正在加载课程表……");
            await App.Current.Schedule.Execute();
            LoadList();
            alert.Invoke();
            IsBusy = false;
        }

        public void LoadList()
        {
            var grid = BindingPage.grid;

            for (int i = grid.Children.Count; i > 7 + App.Current.DailyClassCount; i--)
            {
                grid.Children.RemoveAt(i - 1);
            }

            // Render classes
            App.Current.Schedule.RenderWeek(week, grid.Children);
        }

        async void Create()
        {
            await (new CurriculumPage(new CurriculumItem { IsCustom = true, CourseID = "CUSTOM-" + DateTime.Now.ToString("s") }, true)).ShowAsync(BindingPage.Navigation);
        }
    }
}
