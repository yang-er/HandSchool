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
        /// 子入口点请求
        /// </summary>
        public event Action<IWebEntrance> SubEntranceRequested;

        /// <summary>
        /// 收到JavaScript消息
        /// </summary>
        /// <param name="data">消息</param>
        public abstract Task Receive(string data);

        /// <summary>
        /// 发送子入口点。
        /// </summary>
        /// <param name="webEntrance">入口点</param>
        protected void SendSubEntrance(IWebEntrance webEntrance)
        {
            SubEntranceRequested?.Invoke(webEntrance);
        }
    }
}