using HandSchool.Internal;
using HandSchool.Services;
using System;

namespace HandSchool.Models
{
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
        public Func<IWebEntrance> Load { get; }

        /// <summary>
        /// 创建信息查询的入口点包装。
        /// </summary>
        /// <param name="type">入口点的参数类型。</param>
        public InfoEntranceWrapper(Type type)
        {
            var ent = type.Get<EntranceAttribute>();
            Name = ent.Title;
            Description = ent.Description;
            if (type.Has<HotfixAttribute>())
                type.Get<HotfixAttribute>().CheckUpdate(false);
            Load = () => Core.Reflection.CreateInstance<IWebEntrance>(type);
        }
    }
}
