using HandSchool.Views;
using System;
using System.Threading.Tasks;

namespace HandSchool.Models
{
    /// <summary>
    /// 单击进入的入口点包装，通常传递一个 <see cref="INavigate"/> 对象来帮助界面访问。
    /// </summary>
    public sealed class TapEntranceWrapper : EntranceWrapperBase
    {
        private readonly Func<INavigate, Task> internal_action;
        
        /// <summary>
        /// 入口点被通知，然后执行内部的动作，并传递参数。
        /// </summary>
        public Task Activate(INavigate nav)
        {
            return internal_action?.Invoke(nav);
        }

        /// <summary>
        /// 创建一个新的单击入口点包装。
        /// </summary>
        /// <param name="name">入口点的名称。</param>
        /// <param name="desc">入口点的文字描述。</param>
        /// <param name="action">入口点的异步动作函数。</param>
        public TapEntranceWrapper(string name, string desc, Func<INavigate, Task> action)
        {
            Title = name;
            Detail = desc;
            internal_action = action;
        }
    }
}
