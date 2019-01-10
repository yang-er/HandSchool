using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace HandSchool.Internal
{
    /// <summary>
    /// 模块热更新的标记数据。
    /// </summary>
    /// <example>
    /// 将热更新的内容保存在某个公开地址上。
    ///   [1] 文件内容
    ///     (ver)1;url=balabala...
    ///   [2] 保存文件
    /// </example>
    /// <inheritdoc cref="Attribute"/>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class HotfixAttribute : Attribute
    {
        /// <summary>
        /// 更新源
        /// </summary>
        public string UpdateSource { get; }

        /// <summary>
        /// 本地存储内容
        /// </summary>
        public string LocalStorage { get; }

        /// <summary>
        /// 提供热更新的元数据。
        /// </summary>
        /// <param name="uri">更新源。</param>
        /// <param name="prefix">本地存储文件前缀。</param>
        public HotfixAttribute(string uri, string prefix)
        {
            UpdateSource = uri;
            LocalStorage = prefix;
        }

        /// <summary>
        /// 检查更新并存储结果。
        /// </summary>
        /// <param name="force">是否强制更新。</param>
        public async void CheckUpdate(bool force = false)
        {
            await Task.Yield();

            try
            {
                using (var wc = new AwaredWebClient("", System.Text.Encoding.UTF8))
                {
                    wc.Timeout = 5000;
                    var new_meta = await wc.DownloadStringTaskAsync(UpdateSource);
                    var meta_exp = new_meta.Split(new[] { ';' }, 2);
                    var local_meta = Core.Configure.Read(LocalStorage + ".ver");

                    if (force)
                    {
                        force = true;
                    }
                    else if (local_meta == "")
                    {
                        force = true;
                    }
                    else if (local_meta.Split(new[] { ';' }, 2)[0] != meta_exp[0])
                    {
                        force = true;
                    }
                
                    if (force)
                    {
                        Core.Configure.Write(LocalStorage + ".ver", new_meta);
                        await wc.DownloadFileTaskAsync(meta_exp[1], Path.Combine(Core.Configure.Directory, LocalStorage));
                        Core.Logger.WriteLine("Hotfix", "Module successfully updated - " + LocalStorage);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Logger.WriteException(ex);
                Core.Configure.Write(LocalStorage + ".ver", "");
            }
        }

        /// <summary>
        /// 读取本地的数据。
        /// </summary>
        /// <returns>本地储存的数据。</returns>
        public string ReadContent()
        {
            var ans = Core.Configure.Read(LocalStorage);
            return ans == "" ? null : ans;
        }

        /// <summary>
        /// 读取本地的数据。
        /// </summary>
        /// <param name="obj">附加对象。</param>
        /// <returns>本地储存的数据。</returns>
        public static string ReadContent(object obj)
        {
            return obj.GetType().Get<HotfixAttribute>().ReadContent();
        }
    }
}
