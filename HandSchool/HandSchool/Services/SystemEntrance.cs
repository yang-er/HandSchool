using System.Threading.Tasks;

namespace HandSchool.Services
{
    /// <summary>
    /// 系统入口点
    /// </summary>
    public interface ISystemEntrance
    {
        /// <summary>
        /// 脚本文件地址
        /// </summary>
        string ScriptFileUri { get; }

        /// <summary>
        /// 是否为POST方法
        /// </summary>
        bool IsPost { get; }

        /// <summary>
        /// 发送的数据
        /// </summary>
        string PostValue { get; }

        /// <summary>
        /// 储存文件
        /// </summary>
        string StorageFile { get; }

        /// <summary>
        /// 上次返回值
        /// </summary>
        string LastReport { get; }

        /// <summary>
        /// 执行操作
        /// </summary>
        Task Execute();

        /// <summary>
        /// 解析数据
        /// </summary>
        void Parse();
    }
}
