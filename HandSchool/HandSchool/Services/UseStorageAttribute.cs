using System;

namespace HandSchool.Services
{
    /// <summary>
    /// 类使用的文件的注册特性，用于清空设置文件。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UseStorageAttribute : Attribute
    {
        /// <summary>
        /// 指定的学校
        /// </summary>
        public string School { get; }

        /// <summary>
        /// 此类使用的所有文件
        /// </summary>
        public string[] Files { get; }

        /// <summary>
        /// 类使用的文件的注册特性，用于清空设置文件。
        /// </summary>
        /// <param name="school">指定的学校</param>
        /// <param name="files">此类使用的所有文件</param>
        public UseStorageAttribute(string school, params string[] files)
        {
            School = school;
            Files = files;
        }
    }
}
