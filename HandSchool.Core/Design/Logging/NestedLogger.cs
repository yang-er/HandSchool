using System;

namespace HandSchool.Design.Logging
{
    /// <summary>
    /// 默认嵌套日志。
    /// </summary>
    /// <typeparam name="T">为某个类提供支持</typeparam>
    internal class NestedLogger<T> : ILogger<T>
    {
        /// <summary>
        /// 上一层日志
        /// </summary>
        private ILogger LoggerBase { get; }

        /// <summary>
        /// 默认源名称
        /// </summary>
        public string DefaultSourceName { get; }

        /// <summary>
        /// 实例化一个嵌套日志。
        /// </summary>
        /// <param name="source">上层日志</param>
        public NestedLogger(ILogger source)
        {
            LoggerBase = source;
            DefaultSourceName = typeof(T).Name;
        }

        /// <summary>
        /// 写入一个异常信息。
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="level">等级</param>
        /// <param name="path">文件</param>
        /// <param name="line">行号</param>
        public void WriteException(Exception ex, LogLevel level, string path, int line)
        {
            LoggerBase.WriteException(ex, level, path, line);
        }

        /// <summary>
        /// 写入一行字符串信息。
        /// </summary>
        /// <param name="source">来源</param>
        /// <param name="log">日志</param>
        /// <param name="level">等级</param>
        public void WriteLine(string source, string log, LogLevel level)
        {
            LoggerBase.WriteLine(source, log, level);
        }
    }
}
