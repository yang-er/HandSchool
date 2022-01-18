using HandSchool.Internals;
using HandSchool.Models;
using System.Collections.Generic;
using System.Windows.Input;
using HandSchool.Services;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 最简单的课程表的视图模型，未实现复杂功能。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    public abstract class ScheduleViewModelBase : BaseViewModel
    {
        /// <summary>
        /// 是否提供了增删改查功能。
        /// </summary>
        public abstract bool IsComposed { get; }

        /// <summary>
        /// 从周的条件渲染课程表。
        /// </summary>
        /// <param name="week">第几周。</param>
        /// <param name="state">是不是寒暑假。</para>
        /// <param name="list">输出列表的枚举器。</param>
        public abstract void RenderWeek(int week, SchoolState state, out IEnumerable<CurriculumItemBase> list);

        /// <summary>
        /// 当前周
        /// </summary>
        public abstract int Week { get; set; }
        
        public abstract SchoolState SchoolState { get; set; }

        /// <summary>
        /// 添加课程的命令
        /// </summary>
        public ICommand AddCommand { get; set; }

        /// <summary>
        /// 刷新课程表的命令
        /// </summary>
        public ICommand RefreshCommand { get; set; }

        /// <summary>
        /// 修改当前周的命令
        /// </summary>
        public ICommand ChangeWeekCommand { get; set; }

        /// <summary>
        /// 修改当前周的命令
        /// </summary>
        public ICommand QuickChangeWeekCommand { get; set; }

        /// <summary>
        /// 获取多节课的课程表列表。
        /// </summary>
        protected static IEnumerable<CurriculumSet> FetchItemsSet(IEnumerable<CurriculumItem> list)
        {
            var controller = new CurriculumMergeAlgorithm(Core.App.DailyClassCount);
            foreach (var i in list)
                controller.AddClass(i);
            return controller.ToList();
        }
    }
}