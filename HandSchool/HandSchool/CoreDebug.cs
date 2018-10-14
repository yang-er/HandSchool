using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HandSchool
{
    public sealed partial class Core
    {
        /// <summary>
        /// 调试阶段的断言。
        /// </summary>
        /// <param name="cond">条件</param>
        /// <param name="val">断言内容</param>
        public static void Assert(bool cond, string val, [CallerLineNumber] int line = -1, [CallerFilePath] string file = "")
        {
#if DEBUG
            if (!cond)
            {
                Log("断言失败！" + val);
                var ex = new Exception(val);
                Log(ex.StackTrace);
                Log("在 {0} : {1}", file, line);
                throw ex;
            }
#endif
        }

#if DEBUG
        private static DateTime ProfilerLast;
        private static bool IsProfiling = false;
#endif

        /// <summary>
        /// 性能分析的检查点。
        /// </summary>
        /// <param name="name">性能分析输出的注释。</param>
        /// <param name="type">性能分析的类型。0为启动，2为关闭，1为标记。</param>
        public static void Profile(int type, string name = "")
        {
#if DEBUG
            if (type == 0)
            {
                Assert(!IsProfiling, "性能分析任务已启动。");
                IsProfiling = true;
                Log($"[PROFILER] 性能分析任务 {name} 启动。");
                ProfilerLast = DateTime.Now;
            }
            else if (type == 1)
            {
                Log($"[PROFILER] 历时 {(DateTime.Now - ProfilerLast).TotalMilliseconds}ms 过程 {name} 完成。");
                ProfilerLast = DateTime.Now;
            }
            else if (type == 2)
            {
                Assert(IsProfiling, "性能分析任务未启动。");
                IsProfiling = false;
                Log($"[PROFILER] 性能分析任务结束。");
                IsProfiling = false;
            }
            else
            {
                throw new InvalidOperationException();
            }
#endif
        }

        /// <summary>
        /// 向调试器写入调试信息。
        /// </summary>
        /// <param name="output">输出的字符串内容。</param>
        public static void Log(string output)
        {
            Debug.WriteLine(output);
        }

        /// <summary>
        /// 向调试器写入调试信息。
        /// </summary>
        /// <param name="output">产生的异常内容。</param>
        public static void Log(Exception output)
        {
            Debug.WriteLine(output);
        }

        /// <summary>
        /// 向调试器写入调试信息。
        /// </summary>
        /// <param name="format">字符串的输出格式。</param>
        /// <param name="param">对应输出内容的参数。</param>
        public static void Log(string format, params object[] param)
        {
            Debug.WriteLine(format, param);
        }

        public static void NotifyInitialize([CallerFilePath] string name = "")
        {
            Log(name + " Initialized.");
        }
    }
}
