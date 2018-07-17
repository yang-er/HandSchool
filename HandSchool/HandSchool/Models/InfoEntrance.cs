using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using HFAttr = HandSchool.Services.HotfixAttribute;

namespace HandSchool.Models
{
    /// <summary>
    /// 创建入口点的函数
    /// </summary>
    /// <returns>新的入口点</returns>
    public delegate IInfoEntrance EntranceCreator();

    /// <summary>
    /// 入口点组
    /// </summary>
    public class InfoEntranceGroup : List<InfoEntranceWrapper>
    {
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
    /// 入口点包装
    /// </summary>
    public class InfoEntranceWrapper
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
        /// 入口点包装
        /// </summary>
        /// <param name="name">入口点名称</param>
        /// <param name="description">入口点描述</param>
        /// <param name="type">入口点加载委托</param>
        public InfoEntranceWrapper(string name, string description, Type type)
        {
            Name = name;
            Description = description;
            if (type.GetCustomAttribute(typeof(HFAttr)) is HFAttr hfattr)
                Task.Run(() => hfattr.CheckUpdate(false));
            Load = () => Activator.CreateInstance(type) as IInfoEntrance;
        }
    }

    /// <summary>
    /// 入口点菜单
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
        /// 入口点菜单
        /// </summary>
        /// <param name="name">菜单名称</param>
        /// <param name="cmd">菜单执行命令</param>
        /// <param name="ico">菜单图标</param>
        public InfoEntranceMenu(string name, Command cmd, string ico)
        {
            Name = name;
            Command = cmd;
            Icon = ico;
        }
    }
}
