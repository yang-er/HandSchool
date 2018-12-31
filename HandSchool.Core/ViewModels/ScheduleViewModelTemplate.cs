using HandSchool.Models;
using System.Collections.Generic;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 分离式的课程表界面，以后使用。
    /// </summary>
    /// <inheritdoc cref="ScheduleViewModelBase" />
    public class TemplateScheduleViewModel : ScheduleViewModelBase
    {
        public override bool IsComposed => false;
        public override int Week { get => 0; set => Core.Log("Error value: " + value); }
        public TemplateScheduleViewModel(string tit) { Title = tit; }
        public IEnumerable<CurriculumItem> Items { get; set; }
        private IEnumerable<CurriculumSet> ItemsSet { get; set; }
        
        public override void RenderWeek(int week, out IEnumerable<CurriculumItemBase> list)
        {
            if (ItemsSet is null)
                ItemsSet = FetchItemsSet(Items);
            list = ItemsSet;
        }
    }
}
