using HandSchool.Internals;

namespace HandSchool.Models
{
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
        public CommandAction Command;

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
        public InfoEntranceMenu(string name, CommandAction cmd, string ico)
        {
            Name = name;
            Command = cmd;
            Icon = ico;
        }
    }
}
