using HandSchool.Internal;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace HandSchool.Services
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
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
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
                    var meta_exp = new_meta.Split(new char[] { ';' }, 2);
                    var local_meta = Core.ReadConfig(LocalStorage + ".ver");

                    if (force)
                    {
                        force = true;
                    }
                    else if (local_meta == "")
                    {
                        force = true;
                    }
                    else if (local_meta.Split(new char[] { ';' }, 2)[0] != meta_exp[0])
                    {
                        force = true;
                    }
                
                    if (force)
                    {
                        Core.WriteConfig(LocalStorage + ".ver", new_meta);
                        await wc.DownloadFileTaskAsync(meta_exp[1], Path.Combine(Core.ConfigDirectory, LocalStorage));
                        Core.Log("[Hotfix] Module successfully updated - " + LocalStorage);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Log(ex);
                Core.WriteConfig(LocalStorage + ".ver", "");
            }
        }

        /// <summary>
        /// 读取本地的数据。
        /// </summary>
        /// <returns>本地储存的数据。</returns>
        public string ReadContent()
        {
            return Core.ReadConfig(LocalStorage);
        }

        /// <summary>
        /// 读取本地的数据。
        /// </summary>
        /// <param name="obj">附加对象。</param>
        /// <returns>本地储存的数据。</returns>
        public static string ReadContent(object obj)
        {
            var hf = obj.GetType().GetCustomAttribute(typeof(HotfixAttribute)) as HotfixAttribute;
            Core.Assert(hf != null, "HotfixAttribute not attached.");
            return hf.ReadContent();
        }
    }
}
