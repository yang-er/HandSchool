using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HandSchool.Models;

namespace HandSchool.Internals
{
    /// <summary>
    /// 提供配置文件的相关功能。
    /// </summary>
    [ToFix("需要支持多个终端的配置")]
    public class ConfigurationManager
    {
        /// <summary>
        /// 根据目录构造一个配置管理器，提供相关功能。
        /// </summary>
        /// <param name="directory">目录名</param>
        public ConfigurationManager(string directory)
        {
            Directory = directory;
        }
        
        public SQLiteTableManager<UserAccount> AccountManager { get; set; }
        
        public SQLiteTableManager<ServerJson> JsonManager { get; set; }


        /// <summary>
        /// 数据基础目录
        /// </summary>
        public string Directory { get; }

        /// <summary>
        /// 目前的配置上下文
        /// </summary>
        public int Context { get; }

        /// <summary>
        /// 从文件读取配置数据。
        /// </summary>
        /// <param name="config">即将读取的文件名。</param>
        /// <returns>读取得到的内容。</returns>
        [DebuggerStepThrough]
        public string Read(string config)
        {
            string fn = Path.Combine(Directory, config);
            return File.Exists(fn) ? File.ReadAllText(fn) : "";
        }

        /// <summary>
        /// 将配置数据写入文件。
        /// </summary>
        /// <param name="config">即将写入的文件名。</param>
        /// <param name="value">将要写入的内容。</param>
        [DebuggerStepThrough]
        public void Write(string config, string value)
        {
            if (string.IsNullOrWhiteSpace(config))
            {
                throw new ArgumentNullException(nameof(config));
            }
            var paths = 
                config.Split(Path.DirectorySeparatorChar)
                .Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
            if (paths.Length != 1)
            {
                var cur = Directory;
                for (var i = 0; i < paths.Length - 1;i++)
                {
                    cur = Path.Combine(cur, paths[i]);
                    if (!System.IO.Directory.Exists(cur))
                    {
                        System.IO.Directory.CreateDirectory(cur);
                    }
                }
            }
            File.WriteAllText(Path.Combine(Directory, config), value);
        }

        /// <summary>
        /// 将配置数据写入文件。
        /// </summary>
        /// <param name="config">即将写入的文件名。</param>
        [DebuggerStepThrough]
        public void Remove(string config)
        {
            if (string.IsNullOrWhiteSpace(config))
            {
                throw new ArgumentNullException(nameof(config));
            }

            var paths =
                config.Split(Path.DirectorySeparatorChar)
                    .Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
            paths[0] = Path.Combine(Directory, paths[0]);
            for (var i = 1; i < paths.Length; i++)
            {
                paths[i] = Path.Combine(paths[i - 1], paths[i]);
            }

            var fileName = paths[paths.Length - 1];
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
                for (var i = paths.Length - 2; i >= 0; i--)
                {
                    var dirs = System.IO.Directory.GetDirectories(paths[i]);
                    var files = System.IO.Directory.GetFiles(paths[i]);
                    if (dirs.Length + files.Length == 0)
                    {
                        System.IO.Directory.Delete(paths[i]);
                    }
                }
            }
        }
    }
}