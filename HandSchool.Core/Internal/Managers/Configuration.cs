using System.Diagnostics;
using System.IO;

namespace HandSchool.Internal
{
    public class ConfigurationManager
    {
        public ConfigurationManager(string directory)
        {
            Directory = directory;
        }

        /// <summary>
        /// 数据基础目录
        /// </summary>
        public string Directory { get; }

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
            File.WriteAllText(Path.Combine(Directory, config), value);
        }

        /// <summary>
        /// 将配置数据写入文件。
        /// </summary>
        /// <param name="config">即将写入的文件名。</param>
        [DebuggerStepThrough]
        public void Remove(string config)
        {
            var fileName = Path.Combine(Directory, config);
            if (File.Exists(fileName)) File.Delete(fileName);
        }
    }
}
