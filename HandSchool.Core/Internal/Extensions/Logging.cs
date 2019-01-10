using HandSchool.Models;
using HandSchool.Services;
using HandSchool.Views;
using System;
using System.Runtime.CompilerServices;

namespace HandSchool.Internal
{
    /// <summary>
    /// 提供日志的记录拓展方法。
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// 写入消息日志。
        /// </summary>
        /// <param name="viewModel">操作性对象</param>
        /// <param name="content">日志内容</param>
        public static void WriteLog(this IViewResponse viewModel, string content)
        {
            Core.Logger.WriteLine(viewModel.GetType().Name, content);
        }

        /// <summary>
        /// 写入消息日志。
        /// </summary>
        /// <param name="viewModel">操作性对象</param>
        /// <param name="content">日志内容</param>
        public static void WriteLog(this ILoginField viewModel, string content)
        {
            Core.Logger.WriteLine(viewModel.GetType().Name, content);
        }

        /// <summary>
        /// 写入消息日志。
        /// </summary>
        /// <param name="entrance">操作性对象</param>
        /// <param name="content">日志内容</param>
        public static void WriteLog(this ISystemEntrance entrance, string content)
        {
            Core.Logger.WriteLine(entrance.GetType().Name, content);
        }

        /// <summary>
        /// 写一行警告，并指出所在位置等。
        /// </summary>
        /// <param name="viewModel">操作性对象</param>
        /// <param name="ex">异常信息</param>
        /// <param name="path">文件目录</param>
        /// <param name="line">文件行号</param>
        public static void WriteLog(this IViewResponse viewModel, Exception ex,
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0)
        {
            Core.Logger.WriteException(ex, path, line);
        }

        /// <summary>
        /// 写一行警告，并指出所在位置等。
        /// </summary>
        /// <param name="viewModel">操作性对象</param>
        /// <param name="ex">异常信息</param>
        /// <param name="path">文件目录</param>
        /// <param name="line">文件行号</param>
        public static void WriteLog(this ILoginField viewModel, Exception ex,
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0)
        {
            Core.Logger.WriteException(ex, path, line);
        }

        /// <summary>
        /// 写一行警告，并指出所在位置等。
        /// </summary>
        /// <param name="entrance">操作性对象</param>
        /// <param name="ex">异常信息</param>
        /// <param name="path">文件目录</param>
        /// <param name="line">文件行号</param>
        public static void WriteLog(this ISystemEntrance entrance, Exception ex,
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0)
        {
            Core.Logger.WriteException(ex, path, line);
        }
    }
}
