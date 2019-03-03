using System.Collections.Generic;
using System.Threading.Tasks;
using HandSchool.Models;

namespace HandSchool.Services
{
    /// <summary>
    /// 课程表获取
    /// </summary>
    public interface IScheduleEntrance
    {
        /// <summary>
        /// 获取下一节课的编号。
        /// </summary>
        int GetClassNext();

        /// <summary>
        /// 获取课程数据。
        /// </summary>
        /// <returns>课程数据的迭代器</returns>
        Task<IEnumerable<CurriculumItem>> ExecuteAsync();
    }
}
