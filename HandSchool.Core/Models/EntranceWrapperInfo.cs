using HandSchool.Internal;
using HandSchool.Services;
using System;

namespace HandSchool.Models
{
    /// <summary>
    /// 信息查询入口点包装，通常会用 <see cref="IWebEntrance"/> 来进行信息查询。
    /// </summary>
    public sealed class InfoEntranceWrapper : IEntranceWrapper
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
        public InfoEntranceWrapper(Type type) : this(type, () => Core.Reflection.CreateInstance<IWebEntrance>(type)) { }

        /// <summary>
        /// 从 <typeparamref name="T"/> 创建一个入口点包装。
        /// </summary>
        /// <typeparam name="T">入口点类型</typeparam>
        /// <returns>创建好的入口点包装</returns>
        public static InfoEntranceWrapper From<T>()
            where T : IWebEntrance, new()
        {
            return new InfoEntranceWrapper(typeof(T), () => new T());
        }

        /// <summary>
        /// 创建信息查询的入口点包装。
        /// </summary>
        /// <param name="type">入口点的参数类型。</param>
        /// <param name="loader">入口点加载方法。</param>
        private InfoEntranceWrapper(Type type, Func<IWebEntrance> loader)
        {
            var ent = type.Get<EntranceAttribute>();
            Name = ent.Title;
            Description = ent.Description;
            if (type.Has<HotfixAttribute>())
                type.Get<HotfixAttribute>().CheckUpdate(false);
            Load = loader;
        }
    }
}