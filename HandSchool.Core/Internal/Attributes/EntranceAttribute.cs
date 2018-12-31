using System;
using HandSchool.Models;

namespace HandSchool.Internal
{
    /// <summary>
    /// 入口点信息特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class EntranceAttribute : Attribute
    {
        /// <summary>
        /// 入口点名称
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// 学校名称
        /// </summary>
        public string School { get; }

        /// <summary>
        /// 入口点描述
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 入口点类型
        /// </summary>
        public EntranceType Type { get; }

        /// <summary>
        /// 入口点信息特性
        /// </summary>
        /// <param name="school">学校名称</param>
        /// <param name="title">入口点名称</param>
        /// <param name="describe">入口点描述</param>
        /// <param name="type">入口点类型</param>
        public EntranceAttribute(string school, string title, string describe = "", EntranceType type = EntranceType.NormalEntrance)
        {
            School = school;
            Title = title;
            Description = describe;
            Type = type;
        }
    }
}
