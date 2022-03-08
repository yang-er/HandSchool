using System.Threading.Tasks;

namespace HandSchool.Services
{
    /// <summary>
    /// 绩点获取服务
    /// </summary>
    /// <inheritdoc cref="ISystemEntrance"/>
    public interface IGradeEntrance : ISystemEntrance
    {
        Task EntranceAll();
        Task LoadFromNative();
    }
}
