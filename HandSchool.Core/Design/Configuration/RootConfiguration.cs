using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace HandSchool.Design.Configuration
{
    /// <summary>
    /// 提供配置文件的相关功能。
    /// </summary>
    public class RootConfiguration : IConfiguration
    {
        /// <summary>
        /// 根据目录构造一个配置管理器，提供相关功能。
        /// </summary>
        /// <param name="directory">目录名</param>
        public RootConfiguration(string directory)
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
        public void Save(string config, string value)
        {
            File.WriteAllText(Path.Combine(Directory, config), value);
        }

        /// <summary>
        /// 将配置数据删除。
        /// </summary>
        /// <param name="config">即将删除的文件名</param>
        [DebuggerStepThrough]
        public void Remove(string config)
        {
            var fileName = Path.Combine(Directory, config);
            if (File.Exists(fileName)) File.Delete(fileName);
        }

        /// <summary>
        /// 从文件读取配置数据。
        /// </summary>
        /// <param name="config">即将读取的文件名。</param>
        /// <returns>读取得到的内容。</returns>
        [DebuggerStepThrough]
        public Task<string> ReadAsync(string name)
        {
            var tcs = new TaskCompletionSource<string>();
            Task.Run(() => tcs.SetResult(Read(name)));
            return tcs.Task;
        }

        /// <summary>
        /// 将配置数据写入文件。
        /// </summary>
        /// <param name="config">即将写入的文件名。</param>
        /// <param name="value">将要写入的内容。</param>
        [DebuggerStepThrough]
        public Task SaveAsync(string name, string value)
        {
            return Task.Run(() => Save(name, value));
        }

        /// <summary>
        /// 将配置数据删除。
        /// </summary>
        /// <param name="config">即将删除的文件名</param>
        [DebuggerStepThrough]
        public Task RemoveAsync(string name)
        {
            return Task.Run(() => Remove(name));
        }
    }
}