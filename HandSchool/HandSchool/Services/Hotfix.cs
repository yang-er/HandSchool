using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace HandSchool.Services
{
    /// <summary>
    /// 模块热更新
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class HotfixAttribute : Attribute
    {
        /**
         *  Hotfix Module
         *  - Check File content:
         *      (ver)1;url=balabala...
         *  - Save file
         */

        /// <summary>
        /// 更新源
        /// </summary>
        public string UpdateSource { get; }

        /// <summary>
        /// 本地存储内容
        /// </summary>
        public string LocalStorage { get; }

        /// <summary>
        /// 提供热更新的元数据
        /// </summary>
        /// <param name="uri">更新源</param>
        /// <param name="prefix">本地存储文件前缀</param>
        public HotfixAttribute(string uri, string prefix)
        {
            UpdateSource = uri;
            LocalStorage = prefix;
        }

        /// <summary>
        /// 检查更新并存储结果
        /// </summary>
        /// <param name="force">是否强制更新</param>
        public async void CheckUpdate(bool force = false)
        {
            try
            {
                using (var wc = new WebClient())
                {
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
                        Core.WriteConfig(LocalStorage + ".ver", local_meta);
                        await wc.DownloadFileTaskAsync(meta_exp[1], Path.Combine(Core.ConfigDirectory, LocalStorage));
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
        /// 读取本地的数据
        /// </summary>
        /// <returns>数据</returns>
        public string ReadContent()
        {
            return Core.ReadConfig(LocalStorage);
        }

        /// <summary>
        /// 读取本地的数据
        /// </summary>
        /// <param name="obj">附加对象</param>
        /// <returns>数据</returns>
        public static string ReadContent(object obj)
        {
            var hf = obj.GetType().GetCustomAttribute(typeof(HotfixAttribute)) as HotfixAttribute;
            System.Diagnostics.Debug.Assert(hf != null, "HotfixAttribute not attached.");
            return hf.ReadContent();
        }
    }
}
