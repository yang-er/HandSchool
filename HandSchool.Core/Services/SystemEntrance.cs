using System.Threading.Tasks;

namespace HandSchool.Services
{
    /// <summary>
    /// 系统入口点，是所有服务的基础接口，提供了主线操作的异步函数。
    /// </summary>
    public interface ISystemEntrance
    {
        /// <summary>
        /// 异步方法，执行主线操作。
        /// </summary>
        Task Execute();
    }
}
