using System;
using HandSchool.Internals;
using HandSchool.Models;
using System.Collections.Generic;
using HandSchool.Services;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 分离式的课程表界面，以后使用。
    /// </summary>
    /// <inheritdoc cref="ScheduleViewModelBase" />
    public class TemplateScheduleViewModel : ScheduleViewModelBase
    {
        public override bool IsComposed => false;
        public TemplateScheduleViewModel(string tit) { Title = tit; }
        public IEnumerable<CurriculumItem> Items { get; set; }
        private IEnumerable<CurriculumSet> ItemsSet { get; set; }

        public override int Week
        {
            get => 0;
            set => this.WriteLog("Error value was requested to be set: " + value);
        }

        public override SchoolState SchoolState 
        {
            get => SchoolState.Normal;
            set => this.WriteLog("Error value was requested to be set: " + value);
        }


        public override void RenderWeek(int week,SchoolState state, out IEnumerable<CurriculumItemBase> list)
        {
            if (state != SchoolState.Normal)
            {
                list = Array.Empty<CurriculumItemBase>();
                return;
            }
            ItemsSet ??= FetchItemsSet(Items);
            list = ItemsSet;
        }
    }
}
