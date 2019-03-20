using System;

namespace HandSchool.Design
{
    /// <summary>
    /// 提供日志记录功能。
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 默认源名称
        /// </summary>
        string DefaultSourceName { get; }

        /// <summary>
        /// 写入一行字符串信息。
        /// </summary>
        /// <param name="source">来源</param>
        /// <param name="log">日志</param>
        /// <param name="level">等级</param>
        void WriteLine(string source, string log, LogLevel level);

        /// <summary>
        /// 写入一个异常信息。
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="level">等级</param>
        /// <param name="path">文件</param>
        /// <param name="line">行号</param>
        void WriteException(Exception ex, LogLevel level, string path, int line);
    }

    /// <summary>
    /// 提供日志记录功能，通常通过依赖注入创建。
    /// </summary>
    public interface ILogger<out T> : ILogger
    {
    }
}
