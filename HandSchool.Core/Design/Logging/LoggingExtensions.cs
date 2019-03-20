using System;
using System.Runtime.CompilerServices;

namespace HandSchool.Design
{
    /// <summary>
    /// 提供日志的拓展方法。
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// 写入来自异常的警告。
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="ex">异常</param>
        /// <param name="path">文件</param>
        /// <param name="line">行号</param>
        public static void Warn(
            this ILogger logger,
            Exception ex,
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0)
        {
            logger.WriteException(ex, LogLevel.Warn, path, line);
        }

        /// <summary>
        /// 写入来自异常的错误。
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="ex">异常</param>
        /// <param name="path">文件</param>
        /// <param name="line">行号</param>
        public static void Error(
            this ILogger logger,
            Exception ex,
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0)
        {
            logger.WriteException(ex, LogLevel.Error, path, line);
        }

        /// <summary>
        /// 写入来自字符串的信息。
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="info">信息</param>
        public static void Info(
            this ILogger logger,
            string info)
        {
            logger.WriteLine(logger.DefaultSourceName, info, LogLevel.Info);
        }

        /// <summary>
        /// 写入来自字符串的警告。
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="warn">警告</param>
        public static void Warn(
            this ILogger logger,
            string warn)
        {
            logger.WriteLine(logger.DefaultSourceName, warn, LogLevel.Warn);
        }

        /// <summary>
        /// 写入来自字符串的错误。
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="error">错误</param>
        public static void Error(
            this ILogger logger,
            string error)
        {
            logger.WriteLine(logger.DefaultSourceName, error, LogLevel.Error);
        }
    }
}
