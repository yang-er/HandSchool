using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 基本的控制器，实现了H5交互的基类。
    /// </summary>
    public abstract class BaseController : BaseViewModel, IWebEntrance
    {
        /// <summary>
        /// HTML浏览器的执行入口函数
        /// </summary>
        public Action<string> Evaluate { get; set; }

        /// <summary>
        /// 提供的菜单按钮
        /// </summary>
        public List<InfoEntranceMenu> Menu { get; } = new List<InfoEntranceMenu>();

        /// <summary>
        /// 接收 JavaScript 的消息并处理。
        /// </summary>
        /// <param name="data">消息</param>
        public abstract Task Receive(string data);
    }
}
