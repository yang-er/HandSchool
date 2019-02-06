using HandSchool.Services;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 基本的控制器，实现了H5交互的基类。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    /// <inheritdoc cref="IWebEntrance" />
    public abstract class BaseController : BaseViewModel, IWebEntrance
    {
        /// <summary>
        /// 执行JavaScript操作的函数
        /// </summary>
        public Action<string> Evaluate { get; set; }

        /// <summary>
        /// 显示的菜单
        /// </summary>
        public List<MenuEntry> Menu { get; } = new List<MenuEntry>();

        /// <summary>
        /// 收到JavaScript消息
        /// </summary>
        /// <param name="data">消息</param>
        public abstract Task Receive(string data);
    }
}