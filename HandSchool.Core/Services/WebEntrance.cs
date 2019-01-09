using HandSchool.Models;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task Receive(string data);

        /// <summary>
        /// 页面响应
        /// </summary>
        IViewResponse View { get; }

        /// <summary>
        /// 执行JavaScript脚本的函数
        /// </summary>
        Action<string> Evaluate { get; set; }

        /// <summary>
        /// 可用菜单
        /// </summary>
        List<InfoEntranceMenu> Menu { get; }
    }
}
