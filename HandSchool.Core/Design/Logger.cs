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
        void WriteLine(string source, string log, LogLevel level);

        void WriteException(Exception ex, LogLevel level, string path, int line);
    }

    public interface ILogger<T> : ILogger { }

    internal class NestedLogger<T> : ILogger<T>
    {
        private ILogger LoggerBase { get; }

        public NestedLogger(ILogger source)
        {
            LoggerBase = source;
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

        public static void Info<T>(this ILogger<T> logger, string info)
        {
            logger.WriteLine(typeof(T).Name, info, LogLevel.Info);
        }
    }
}
