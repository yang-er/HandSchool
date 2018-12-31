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
        /// 开启页面自带的POST内容。如果为NULL则GET。
        /// </summary>
        byte[] OpenWithPost { get; }

        /// <summary>
        /// 页面使用的cookie
        /// </summary>
        List<string> Cookie { get; }

        /// <summary>
        /// 处理子网页的出现
        /// </summary>
        /// <param name="sub">子网页</param>
        /// <returns>新的Url入口点</returns>
        IUrlEntrance SubUrlRequested(string sub);
    }
}
