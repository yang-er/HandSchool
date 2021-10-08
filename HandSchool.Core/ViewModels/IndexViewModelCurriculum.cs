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
            set
            {
                if (curriculum1 != null)
                    curriculum1.State = ClassState.Other;
                SetProperty(ref curriculum1, value, onChanged: UpdateHasClass,mode: SetPropertyMode.Reference);
                if (curriculum1 != null)
                    curriculum1.State = ClassState.Next;
            }
        }

        /// <summary>
        /// 正在进行的课
        /// </summary>
        public CurriculumItem CurrentClass
        {
            get => curriculum2;
            set
            {
                if (curriculum2 != null)
                    curriculum2.State = ClassState.Other;
                SetProperty(ref curriculum2, value, onChanged: UpdateHasClass, mode: SetPropertyMode.Reference);
                if (curriculum2 != null)
                    curriculum2.State = ClassState.Current;
            }
        }

        /// <summary>
        /// 更新有无课的显示状态。
        /// </summary>
        private void UpdateHasClass()
        {
            Core.Platform.EnsureOnMainThread(() =>
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

        public System.Collections.ObjectModel.ObservableCollection<CurriculumItem> ClassToday { get; set; }
            = new System.Collections.ObjectModel.ObservableCollection<CurriculumItem>();

        /// <summary>
        /// 更新当前时间对应的课程。
        /// </summary>
        private System.Collections.Generic.IList<CurriculumItem> UpdateTodayCurriculum()
        {
            var today = (int)DateTime.Now.DayOfWeek;
            if (today == 0) today = 7;
            var week = Core.App.Service.CurrentWeek;
            var ct = ScheduleViewModel.Instance.FindItems((obj) => obj.IfShow(week) && obj.WeekDay == today);

            var cor = Core.App.Schedule.CurrentClass;
            if (cor.section != 0)
            {
                CurrentClass = ScheduleViewModel.Instance.FindLastItem(
                   obj =>
                   {
                       var res = obj.IfShow(week)
                       && obj.WeekDay == today
                       && obj.DayBegin <= cor.section
                       && obj.DayEnd >= cor.section;
                       if (res)
                       {
                           if (obj.DayEnd == cor.section)
                           {
                               if (cor.state == Services.SectionState.ClassOver)
                                   res = false;
                           }
                       }
                       return res;
                   });
            }
            NextClass = ScheduleViewModel.Instance.FindItem(obj => obj.IfShow(week) && obj.WeekDay == today && obj.DayBegin > cor.section);
            return ct;
        }
    }
}