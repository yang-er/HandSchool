using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.Models;
using System;
using System.Collections.Generic;

namespace HandSchool.Services
{
    /// <summary>
    /// 网页入口点
    /// </summary>
    public interface IWebEntrance
    {
        /// <summary>
        /// 接收invokeCSharpAction的信息
        /// </summary>
        /// <param name="data">字符串接收</param>
        void Receive(string data);

        /// <summary>
        /// 页面响应
        /// </summary>
        IViewResponse Binding { get; set; }

        /// <summary>
        /// 执行JavaScript脚本的函数
        /// </summary>
        Action<string> Evaluate { get; set; }

        /// <summary>
        /// 可用菜单
        /// </summary>
        List<InfoEntranceMenu> Menu { get; set; }
    }

    /// <summary>
    /// 链接入口点
    /// </summary>
    public interface IUrlEntrance : IWebEntrance
    {
        /// <summary>
        /// 使用的网址
        /// </summary>
        string HtmlUrl { get; set; }
    }

    /// <summary>
    /// 信息入口点
    /// </summary>
    public interface IInfoEntrance : ISystemEntrance, IWebEntrance
    {
        /// <summary>
        /// 使用的Bootstrap文档
        /// </summary>
        Bootstrap HtmlDocument { get; set; }
    }
}
