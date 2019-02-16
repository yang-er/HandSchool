namespace HandSchool.Models
{
    /// <summary>
    /// 入口点包装的基本接口。
    /// </summary>
    public class EntranceWrapperBase
    {
        /// <summary>
        /// 入口点名称
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// 入口点描述
        /// </summary>
        public string Detail { get; protected set; }
    }
}
