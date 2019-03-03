using HandSchool.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.Services
{
    /// <summary>
    /// 系统消息入口点
    /// </summary>
    /// <inheritdoc cref="ISystemEntrance"/>
    public interface IMessageEntrance
    {
        /// <summary>
        /// 异步加载所有消息。
        /// </summary>
        /// <exception cref="ServiceException" />
        Task<IEnumerable<IMessageItem>> ExecuteAsync();

        /// <summary>
        /// 设置读取状态
        /// </summary>
        /// <param name="id">消息id</param>
        /// <param name="read">是否已读</param>
        /// <exception cref="ServiceException" />
        Task SetReadState(int id, bool read);

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="id">消息id</param>
        /// <exception cref="ServiceException" />
        Task Delete(int id);
    }
}
