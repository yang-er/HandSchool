using System.Collections.Generic;

namespace HandSchool.Services
{
    /// <summary>
    /// 链接入口点
    /// </summary>
    /// <inheritdoc cref="IWebEntrance"/>
    public interface IUrlEntrance : IWebEntrance
    {
        /// <summary>
        /// 使用的网址
        /// </summary>
        string HtmlUrl { get; set; }
        
        /// <summary>
        /// 处理子网页的出现
        /// </summary>
        /// <param name="sub">子网页</param>
        /// <returns>新的Url入口点</returns>
        IUrlEntrance SubUrlRequested(string sub);
    }
}
