using System.Threading.Tasks;

namespace HandSchool.Services
{
    /// <summary>
    /// 绩点获取
    /// </summary>
    public interface IGradeEntrance : ISystemEntrance
    {
        /// <summary>
        /// 发送GPA的方法
        /// </summary>
        string GPAPostValue { get; }

        /// <summary>
        /// 解析GPA
        /// </summary>
        void ParseGPA();

        /// <summary>
        /// 获取GPA
        /// </summary>
        /// <returns></returns>
        Task GatherGPA();

        /// <summary>
        /// 上次GPA返回值
        /// </summary>
        string LastReportGPA { get; }
    }
}
