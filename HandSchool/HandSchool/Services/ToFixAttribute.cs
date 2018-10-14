using System;

namespace HandSchool.Services
{
    /// <summary>
    /// 等待修改的特性，用于标记下一个版本的任务和已知bug等。
    /// </summary>
    public class ToFixAttribute : Attribute
    {
        /// <summary>
        /// 等待修改的特性，用于标记下一个版本的任务和已知bug等。
        /// </summary>
        /// <param name="description">下一个版本需要完成的事情。</param>
        public ToFixAttribute(string description) { }
    }
}
