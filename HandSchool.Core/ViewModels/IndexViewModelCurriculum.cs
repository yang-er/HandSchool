using HandSchool.Models;
using System;
using System.Collections.Generic;
using HandSchool.Services;

namespace HandSchool.ViewModels
{
    public sealed partial class IndexViewModel
    {
        CurriculumItem _curriculum2;
        CurriculumItem _curriculum1;

        /// <summary>
        /// 接下来的课
        /// </summary>
        public CurriculumItem NextClass
        {
            get => _curriculum1;
            set
            {
                if (_curriculum1 != null)
                    _curriculum1.State = ClassState.Other;
                SetProperty(ref _curriculum1, value, onChanged: UpdateHasClass,mode: SetPropertyMode.Reference);
                if (_curriculum1 != null)
                    _curriculum1.State = ClassState.Next;
            }
        }

        /// <summary>
        /// 正在进行的课
        /// </summary>
        public CurriculumItem CurrentClass
        {
            get => _curriculum2;
            set
            {
                if (_curriculum2 != null)
                    _curriculum2.State = ClassState.Other;
                SetProperty(ref _curriculum2, value, onChanged: UpdateHasClass, mode: SetPropertyMode.Reference);
                if (_curriculum2 != null)
                    _curriculum2.State = ClassState.Current;
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
        public bool CurrentHasClass => _curriculum2 != null;

        /// <summary>
        /// 接下来是否有课
        /// </summary>
        public bool NextHasClass => _curriculum1 != null;

        /// <summary>
        /// 当前是否没有课
        /// </summary>
        public bool NoClass =>  _curriculum1 is null && _curriculum2 is null;

        public System.Collections.ObjectModel.ObservableCollection<CurriculumItem> ClassToday { get; set; }
            = new System.Collections.ObjectModel.ObservableCollection<CurriculumItem>();

        /// <summary>
        /// 更新当前时间对应的课程。
        /// </summary>
        private IList<CurriculumItem> UpdateTodayCurriculum()
        {
            if (Core.App.Service.SchoolState != SchoolState.Normal)
            {
                _curriculum1 = _curriculum2 = null;
                return new List<CurriculumItem>();
            }
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
                               if (cor.state == SectionState.ClassOver)
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