using System;
using System.Runtime.CompilerServices;

namespace HandSchool.Design
{
    public enum LogLevel
    {
        Info,
        Warn,
        Error,
    }

    public interface ILogger
    {
        string DefaultSourceName { get; }

        void WriteLine(string source, string log, LogLevel level);

        void WriteException(Exception ex, LogLevel level, string path, int line);
    }

    public interface ILogger<out T> : ILogger { }

    internal class NestedLogger<T> : ILogger<T>
    {
        private ILogger LoggerBase { get; }

        public string DefaultSourceName { get; }

        public NestedLogger(ILogger source)
        {
            LoggerBase = source;
            DefaultSourceName = typeof(T).Name;
        }

        public void WriteException(Exception ex, LogLevel level, string path, int line)
        {
            LoggerBase.WriteException(ex, level, path, line);
        }

        public void WriteLine(string source, string log, LogLevel level)
        {
            LoggerBase.WriteLine(source, log, level);
        }
    }

    public static class LoggerExtensions
    {
        public static void Warn(this ILogger logger, Exception ex, [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            logger.WriteException(ex, LogLevel.Warn, path, line);
        }

        public static void Error(this ILogger logger, Exception ex, [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            logger.WriteException(ex, LogLevel.Error, path, line);
        }

        public static void Info(this ILogger logger, string info)
        {
            logger.WriteLine(logger.DefaultSourceName, info, LogLevel.Info);
        }

        public static void Warn(this ILogger logger, string warn)
        {
            logger.WriteLine(logger.DefaultSourceName, warn, LogLevel.Warn);
        }

        public static void Error(this ILogger logger, string error)
        {
            logger.WriteLine(logger.DefaultSourceName, error, LogLevel.Error);
        }
    }
}
