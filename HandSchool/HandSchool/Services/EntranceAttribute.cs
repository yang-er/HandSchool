using System;

namespace HandSchool.Services
{
    /// <summary>
    /// 入口点信息特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class EntranceAttribute : Attribute
    {
        /// <summary>
        /// 入口点名称
        /// </summary>
        public string Title { get; }

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
        /// <param name="title">入口点名称</param>
        /// <param name="describe">入口点描述</param>
        /// <param name="type">入口点类型</param>
        public EntranceAttribute(string title, string describe = "", EntranceType type = EntranceType.NormalEntrance)
        {
            Title = title;
            Description = describe;
            Type = type;
        }
    }

    /// <summary>
    /// 入口点类型枚举
    /// </summary>
    public enum EntranceType
    {
        /// <summary>
        /// 普通入口点
        /// </summary>
        NormalEntrance,

        /// <summary>
        /// 信息入口点
        /// </summary>
        InfoEntrance,

        /// <summary>
        /// 学校服务入口点
        /// </summary>
        SchoolEntrance,
    }
}
