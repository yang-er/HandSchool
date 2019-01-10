using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HandSchool.Internal
{
    /// <summary>
    /// 提供简单的日志写入。
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// 写一行日志，表示消息。
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="content">消息内容</param>
        [DebuggerStepThrough]
        public void WriteLine(string type, string content)
        {
            Trace.WriteLine($"[{type}] {content}");
        }

        /// <summary>
        /// 写一行警告，并指出所在位置等。
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="path">文件目录</param>
        /// <param name="line">文件行号</param>
        [DebuggerStepThrough]
        public void WriteException(
            Exception ex,
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0)
        {
            string type = ex.GetType().Name;
            Debug.WriteLine($"[Warning] {type} caught in " +
                $"Path {path} Line {line}\n" +
                ex.ToString());
        }
    }
}
