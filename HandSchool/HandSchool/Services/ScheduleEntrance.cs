using HandSchool.Models;
using System.Collections.Generic;

namespace HandSchool.Services
{
    /// <summary>
    /// 课程表获取
    /// </summary>
    public interface IScheduleEntrance : ISystemEntrance
    {
        /// <summary>
        /// 下一节课
        /// </summary>
        int ClassNext { get; }

        /// <summary>
        /// 课程表列表
        /// </summary>
        List<CurriculumItem> Items { get; }

        /// <summary>
        /// 渲染课程表
        /// </summary>
        /// <param name="week">第几周</param>
        /// <param name="list">输出列表</param>
        /// <param name="showAll">显示所有周</param>
        void RenderWeek(int week, out List<CurriculumItem> list, bool showAll = false);

        /// <summary>
        /// 保存课程表
        /// </summary>
        void Save();
    }
}
