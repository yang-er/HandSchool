namespace HandSchool.Models
{
    /// <summary>
    /// 入口点包装的基本接口。
    /// </summary>
    public interface IEntranceWrapper
    {
        /// <summary>
        /// 入口点名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 入口点描述
        /// </summary>
        string Description { get; }
    }
}
