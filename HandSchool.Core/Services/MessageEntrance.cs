using System.Threading.Tasks;

namespace HandSchool.Services
{
    /// <summary>
    /// 系统消息入口点
    /// </summary>
    /// <inheritdoc cref="ISystemEntrance"/>
    public interface IMessageEntrance : ISystemEntrance
    {
        /// <summary>
        /// 设置读取状态
        /// </summary>
        /// <param name="id">消息id</param>
        /// <param name="read">是否已读</param>
        Task SetReadState(int id, bool read);

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="id">消息id</param>
        Task Delete(int id);
    }
}
