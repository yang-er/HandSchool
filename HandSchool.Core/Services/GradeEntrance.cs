using HandSchool.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.Services
{
    /// <summary>
    /// 绩点获取服务
    /// </summary>
    public interface IGradeEntrance
    {
        /// <summary>
        /// 使用异步的方式提供成绩信息。
        /// </summary>
        /// <returns>成绩信息</returns>
        /// <exception cref="ServiceException" />
        Task<IEnumerable<IGradeItem>> OnlineAsync();

        /// <summary>
        /// 使用异步的方式提供成绩信息。
        /// </summary>
        /// <returns>成绩信息</returns>
        /// <exception cref="ServiceException" />
        Task<IEnumerable<IGradeItem>> OfflineAsync();
    }
}
