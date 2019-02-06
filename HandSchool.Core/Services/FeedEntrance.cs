using System.Threading.Tasks;

namespace HandSchool.Services
{
    /// <summary>
    /// 信息热点获取服务
    /// </summary>
    /// <inheritdoc cref="ISystemEntrance"/>
    public interface IFeedEntrance : ISystemEntrance
    {
        /// <summary>
        /// 获取第n页新闻。
        /// </summary>
        /// <param name="n">页号</param>
        /// <returns>剩余页数</returns>
        Task<int> Execute(int n);
    }
}