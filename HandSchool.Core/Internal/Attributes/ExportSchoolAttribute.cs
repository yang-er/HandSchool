using Autofac;
using HandSchool.Design;
using System;

namespace HandSchool.Internals
{
    /// <summary>
    /// 学校程序集导出学校特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ExportSchoolAttribute : Attribute
    {
        /// <summary>
        /// 注册的学校类型
        /// </summary>
        public Type RegisteredType { get; }

        /// <summary>
        /// 欢迎页面
        /// </summary>
        public Type HelloPage { get; }

        /// <summary>
        /// 学校的名称
        /// </summary>
        public string SchoolName { get; }

        /// <summary>
        /// 学校的简写字母
        /// </summary>
        public string SchoolId { get; }
        
        /// <summary>
        /// 注册学校加载器，表示此学校已可用。
        /// </summary>
        /// <param name="registerType">注册的学校加载器。</param>
        public ExportSchoolAttribute(
            string id, string name,
            Type registerType, Type page)
        {
            SchoolId = id;
            SchoolName = name;
            RegisteredType = registerType;
            HelloPage = page;
        }

        /// <summary>
        /// 创建学校构造器。
        /// </summary>
        /// <param name="container">依赖注入容器</param>
        /// <returns>学校构建器</returns>
        public SchoolBuilder CreateBuilder(ILifetimeScope container)
        {
            if (!typeof(SchoolBuilder).IsAssignableFrom(RegisteredType))
                throw new InvalidOperationException(RegisteredType.AssemblyQualifiedName + " is unacceptable");
            if (container is null)
                throw new ArgumentNullException(nameof(container));

            using (var sc = container.BeginLifetimeScope(s => s.RegisterType(RegisteredType)))
            {
                return sc.Resolve(RegisteredType) as SchoolBuilder;
            }
        }
    }
}