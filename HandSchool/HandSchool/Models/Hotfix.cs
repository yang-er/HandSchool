using HandSchool.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using static HandSchool.Internal.Helper;
namespace HandSchool.Models
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class HotfixAttribute :Attribute
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
        public void CheckUpdate(bool force = false)
        {
            try
            {
                using (var wc = new WebClient())
                {
                    var new_meta = wc.DownloadString(UpdateSource);
                    var meta_exp = new_meta.Split(new char[] { ';' }, 2);
                    var local_meta = ReadConfFile(LocalStorage + ".ver");

                    if (force)
                    {

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
                        WriteConfFile(LocalStorage + ".ver", local_meta);
                        wc.DownloadFile(meta_exp[1], Path.Combine(DataBaseDir, LocalStorage + ".content"));
                    }
                }
            }
            catch (WebException)
            {
                WriteConfFile(LocalStorage + ".ver", "");
            }
        }

        /// <summary>
        /// 读取本地的数据
        /// </summary>
        /// <returns>数据</returns>
        public string ReadContent()
        {
            var ret = ReadConfFile(LocalStorage + ".content");
            if (ret != "") return ret;
            else
            {
                CheckUpdate(true);
                return ReadConfFile(LocalStorage + ".content");
            }
        }
    }
}
