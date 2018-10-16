using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using EntAttr = HandSchool.Services.EntranceAttribute;
using HFAttr = HandSchool.Services.HotfixAttribute;

namespace HandSchool.Models
{
    /// <summary>
    /// 创建信息查询入口点的函数。
    /// </summary>
    /// <returns>新的入口点，用于实际使用。</returns>
    public delegate IWebEntrance EntranceCreator();

    /// <summary>
    /// 实现了 <see cref="List{IEntranceWrapper}"/> 的带标题的入口点信息组。
    /// </summary>
    public class InfoEntranceGroup : List<IEntranceWrapper>
    {
        /// <summary>
        /// 创建一个带标题的入口点信息组。
        /// </summary>
        /// <param name="tit">组的标题名称，用于在ListView中显示。</param>
        public InfoEntranceGroup(string tit = "")
        {
            GroupTitle = tit;
        }

        /// <summary>
        /// 组标题
        /// </summary>
        public string GroupTitle { get; set; }

        public override string ToString()
        {
            return GroupTitle;
        }
    }

    /// <summary>
    /// 入口点包装的基本接口。
    /// </summary>
    public interface IEntranceWrapper
    {
        /// <summary>
        /// 入口点名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 入口点描述
        /// </summary>
        string Description { get; }
    }

    /// <summary>
    /// 单击进入的入口点包装，通常传递一个 <see cref="INavigation"/> 对象来帮助界面访问。
    /// </summary>
    public class TapEntranceWrapper : IEntranceWrapper
    {
        private readonly Func<INavigation, Task> internal_action;

        /// <summary>
        /// 入口点名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 入口点描述
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 入口点被通知，然后执行内部的动作，并传递参数。
        /// </summary>
        public Task Activate(INavigation nav)
        {
            return internal_action?.Invoke(nav);
        }

        /// <summary>
        /// 创建一个新的单击入口点包装。
        /// </summary>
        /// <param name="name">入口点的名称。</param>
        /// <param name="desc">入口点的文字描述。</param>
        /// <param name="action">入口点的异步动作函数。</param>
        public TapEntranceWrapper(string name, string desc, Func<INavigation, Task> action)
        {
            Name = name;
            Description = desc;
            internal_action = action;
        }
    }

    /// <summary>
    /// 信息查询入口点包装，通常会用 <see cref="IWebEntrance"/> 来进行信息查询。
    /// </summary>
    public class InfoEntranceWrapper : IEntranceWrapper
    {
        /// <summary>
        /// 入口点名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 入口点描述
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 入口点加载委托
        /// </summary>
        public EntranceCreator Load { get; }
        
        /// <summary>
        /// 创建信息查询的入口点包装。
        /// </summary>
        /// <param name="type">入口点的参数类型。</param>
        public InfoEntranceWrapper(Type type)
        {
            var ent = type.GetCustomAttribute(typeof(EntAttr)) as EntAttr;
            Name = ent.Title;
            Description = ent.Description;
            if (type.GetCustomAttribute(typeof(HFAttr)) is HFAttr hfattr)
                hfattr.CheckUpdate(false);
            Load = () => Activator.CreateInstance(type) as IWebEntrance;
        }
    }

    /// <summary>
    /// 信息查询所使用的菜单，用于添加本机的按钮来与HTML交互。
    /// </summary>
    public struct InfoEntranceMenu
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 菜单执行命令
        /// </summary>
        public Command Command;

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon;

        /// <summary>
        /// 创建信息查询所使用的菜单。
        /// </summary>
        /// <param name="name">菜单的名称。</param>
        /// <param name="cmd">菜单执行的命令。</param>
        /// <param name="ico">菜单在UWP上显示的图标。</param>
        public InfoEntranceMenu(string name, Command cmd, string ico)
        {
            Name = name;
            Command = cmd;
            Icon = ico;
        }
    }
}
