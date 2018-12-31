using HandSchool.Models;
using System;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public sealed partial class IndexViewModel
    {
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
                OnPropertyChanged(nameof(NextHasClass));
                OnPropertyChanged(nameof(CurrentHasClass));
                OnPropertyChanged(nameof(NoClass));
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
            var today = (int)DateTime.Now.DayOfWeek;
            if (today == 0) today = 7;
            var week = Core.App.Service.CurrentWeek;
            var cor = Core.App.Schedule.GetClassNext();
            NextClass = ScheduleViewModel.Instance.FindItem((obj) => obj.IfShow(week) && obj.WeekDay == today && obj.DayBegin > cor);
            CurrentClass = ScheduleViewModel.Instance.FindLastItem((obj) => obj.IfShow(week) && obj.WeekDay == today && obj.DayBegin > cor - 3 && obj.DayEnd <= cor);
        }
    }
}
